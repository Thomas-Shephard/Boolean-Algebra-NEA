using BooleanAlgebra.Simplification.Logic;

namespace BooleanAlgebra.Simplification;
public class Simplification {
    private SyntaxItem StartSyntaxItem { get; }
    public List<Tuple<SyntaxItem, string>> Simplifications { get; }

    public Simplification(SyntaxItem startSyntaxItem) {
        StartSyntaxItem = startSyntaxItem;
        Simplifications = new List<Tuple<SyntaxItem, string>>();

        int currentIndex;
        uint currentCost;

        Dictionary<int, List<Tuple<SyntaxItem, string>>> simplificationsDictionary = new();

        for (int i = 0; i < 2; i++) {
            List<Tuple<SyntaxItem, string>> tupleList = new();
            Tuple<SyntaxItem, string> simplification = new(startSyntaxItem, "");

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

        Simplifications = simplificationsDictionary.ElementAt(currentIndex).Value;
    }

    public override string ToString() {
        StringBuilder returnValue = new($"[Start ----------, {StartSyntaxItem}]");
        for (int i = 1; i < Simplifications.Count; i++) {
            returnValue.Append($"{Environment.NewLine}[Simplification {i}, {Simplifications[i].Item1}, {Simplifications[i].Item2}]");
        }

        return returnValue.ToString();
    }
}