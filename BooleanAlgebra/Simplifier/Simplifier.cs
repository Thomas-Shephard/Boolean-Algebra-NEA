using BooleanAlgebra.Simplifier.Logic;

namespace BooleanAlgebra.Simplifier;

public class Simplifier {
    private SyntaxItem StartSyntaxItem { get; }

    public Simplifier(SyntaxItem startSyntaxItem) {
        StartSyntaxItem = startSyntaxItem;
    }
    
    public List<Tuple<SyntaxItem, string>> Simplify() {
        int currentIndex;
        uint currentCost;

        Dictionary<int, List<Tuple<SyntaxItem, string>>> simplificationsDictionary = new();

        for (int i = 0; i < 2; i++) {
            List<Tuple<SyntaxItem, string>> tupleList = new();
            Tuple<SyntaxItem, string> simplification = new(StartSyntaxItem, "Initial boolean expression");

            bool didSimplify;
            do {
                if (tupleList.Any(singleSimplification => singleSimplification.Item1.Equals(simplification.Item1)))
                    break;
                if (simplification.Item1.GetCost() > Math.Max(StartSyntaxItem.GetCost() * 2.5, StartSyntaxItem.GetCost() + 50))
                    break;

                tupleList.Add(simplification);
                didSimplify = simplification.Item1.TrySimplifySyntaxTree(i, tupleList.Select(x => x.Item1).ToList(), SimplificationPost.BEFORE, out simplification);
            } while (didSimplify);

            List<List<Tuple<SyntaxItem, string>>> listOfSimplifications = new();
            for (int i1 = 0; i1 < tupleList.Count; i1++) {
                List<Tuple<SyntaxItem, string>> list = tupleList.Take(i1 + 1).ToList();
                bool isFirst = true;
                do {
                    if (!isFirst && tupleList.Any(singleSimplification => singleSimplification.Item1.Equals(simplification.Item1)))
                        break;

                    if (!isFirst)
                        list.Add(simplification);

                    didSimplify = list.Last().Item1.TrySimplifySyntaxTree(i, list.Select(x => x.Item1).ToList(), SimplificationPost.AFTER, out simplification);
                    isFirst = false;
                } while (didSimplify);

                int currentIndexJ = -1;
                uint currentCostJ = uint.MaxValue;
                for (int j = 0; j < list.Count; j++) {
                    uint tempCost = list.ElementAt(j).Item1.GetCost();
                    if (tempCost >= currentCostJ) continue;
                    currentCostJ = tempCost;
                    currentIndexJ = j;
                }

                listOfSimplifications.Add(list.Take(currentIndexJ + 1).ToList());
            }

            currentIndex = -1;
            currentCost = uint.MaxValue;
            for (int i1 = 0; i1 < listOfSimplifications.Count; i1++) {
                uint tempCost = listOfSimplifications[i1].Last().Item1.GetCost();
                if (tempCost >= currentCost) continue;
                currentCost = tempCost;
                currentIndex = i1;
            }

            simplificationsDictionary.Add(i, listOfSimplifications[currentIndex]);
        }

        currentIndex = -1;
        currentCost = uint.MaxValue;
        for (int i = 0; i < simplificationsDictionary.Count; i++) {
            uint tempCost = simplificationsDictionary.ElementAt(i).Value.Last().Item1.GetCost();
            if (tempCost >= currentCost) continue;
            currentCost = tempCost;
            currentIndex = i;
        }

        return simplificationsDictionary.ElementAt(currentIndex).Value;
    }
}