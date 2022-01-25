using BooleanAlgebra.Simplifier.Logic;
namespace BooleanAlgebra.Simplifier;
public class SimplifierItem {
    public SimplifierItem? ParentItem { get; }
    public ISyntaxItem SyntaxTree { get; }
    public string SimplificationReason { get; }
    public bool IsInAfterStage { get; }
    public SimplifierItem? RootNode { get; }
    public List<SimplifierItem> Children { get; }
    public SimplifierItem(ISyntaxItem syntaxTree, string simplificationReason, SimplifierItem? parentItem = null, bool isInAfterStage = false, SimplifierItem? rootNode = null) {
        SyntaxTree = syntaxTree;
        SimplificationReason = simplificationReason;
        ParentItem = parentItem;
        IsInAfterStage = isInAfterStage;
        RootNode = rootNode;
        Children = new List<SimplifierItem>();
    }

    public void Simplify(SimplifierItem? startItem = null) {
        startItem ??= this;
        if (!IsInAfterStage) {
            IEnumerable<Tuple<ISyntaxItem, string>> lowestCostNextItem = SyntaxTree.SimplifySyntaxTree(SimplificationOrder.PRE)
                .Where(x => startItem.PreviousSyntaxItems().All(y => !y.Equals(x.Item1)));
            foreach ((ISyntaxItem? item1, string? item2) in lowestCostNextItem) {
                if(item1.GetCost() > Math.Max(startItem.SyntaxTree.GetCost() * 2.5, startItem.SyntaxTree.GetCost() + 50))
                    continue;
                Children.Add(new SimplifierItem(item1, item2, this, rootNode: RootNode ?? this));
                Children.Last().Simplify(startItem);
            }
        }
        
        foreach ((ISyntaxItem? item1, string? item2) in SyntaxTree.SimplifySyntaxTree(SimplificationOrder.POST).Where(x => startItem.PreviousSyntaxItems().All(y => !y.Equals(x.Item1)))) {
            if(item1.GetCost() > Math.Max(startItem.SyntaxTree.GetCost() * 2.5, startItem.SyntaxTree.GetCost() + 50))
                continue;
            Children.Add(new SimplifierItem(item1, item2, this, true, RootNode ?? this));
            Children.Last().Simplify(startItem);
        }
    }

    private List<ISyntaxItem> PreviousSyntaxItems() {
        List<ISyntaxItem> returnValue = new() {SyntaxTree};
        foreach(SimplifierItem child in Children) {
            returnValue.AddRange(child.PreviousSyntaxItems());
        }
        return returnValue;
    }

    public static SimplifierItem GetSmallestCostSyntaxItem(SimplifierItem rootNode) {
        SimplifierItem lowestCostItem = rootNode;
        
        Queue<SimplifierItem> queue = new();
        queue.Enqueue(rootNode);
        while (queue.Count > 0) {
            SimplifierItem currentItem = queue.Dequeue();
            if (currentItem.SyntaxTree.GetCost() < lowestCostItem.SyntaxTree.GetCost()) {
                lowestCostItem = currentItem;
            }
            foreach (SimplifierItem child in currentItem.Children) {
                queue.Enqueue(child);
            }
        }

        return lowestCostItem;
    }

    
    public List<Tuple<ISyntaxItem, string>> GetSimplifiedSyntaxTree() {
        List<Tuple<ISyntaxItem, string>> returnValue = new();
        if(ParentItem is not null)
            returnValue.AddRange(ParentItem.GetSimplifiedSyntaxTree());
        returnValue.Add(new Tuple<ISyntaxItem, string>(SyntaxTree, SimplificationReason));
        return returnValue;
    }
}

public class Simplifier {
    private const string StartMessage = "Initial boolean expression";
    private ISyntaxItem StartSyntaxItem { get; }

    public Simplifier(ISyntaxItem startSyntaxItem) {
        StartSyntaxItem = startSyntaxItem;
    }

    public List<Tuple<ISyntaxItem, string>> Simplify() {
        SimplifierItem simplifierTree = new(StartSyntaxItem, StartMessage);
        simplifierTree.Simplify();
        
        return SimplifierItem.GetSmallestCostSyntaxItem(simplifierTree).GetSimplifiedSyntaxTree();
    }
}