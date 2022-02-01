namespace BooleanAlgebra.Simplifier; 
/// <summary>
/// 
/// </summary>
public class SimplificationItem {
    /// <summary>
    /// 
    /// </summary>
    private SimplificationItem? ParentItem { get; }
    /// <summary>
    /// 
    /// </summary>
    private ISyntaxItem SyntaxTree { get; }
    /// <summary>
    /// 
    /// </summary>
    private string SimplificationReason { get; }
    /// <summary>
    /// 
    /// </summary>
    private bool IsInAfterStage { get; }
    /// <summary>
    /// 
    /// </summary>
    private SimplificationItem? RootNode { get; }
    /// <summary>
    /// 
    /// </summary>
    private List<SimplificationItem> Children { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="simplificationReason"></param>
    /// <param name="parentItem"></param>
    /// <param name="isInAfterStage"></param>
    /// <param name="rootNode"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public SimplificationItem(ISyntaxItem syntaxTree, string simplificationReason, SimplificationItem? parentItem = null, bool isInAfterStage = false, SimplificationItem? rootNode = null) {
        SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
        SimplificationReason = simplificationReason ?? throw new ArgumentNullException(nameof(simplificationReason));
        ParentItem = parentItem;
        IsInAfterStage = isInAfterStage;
        RootNode = rootNode;
        Children = new List<SimplificationItem>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startItem"></param>
    public void Simplify(SimplificationItem? startItem = null) {
        startItem ??= this;
        if (!IsInAfterStage) {
            IEnumerable<SimplificationReason> lowestCostNextItem = SyntaxTree.SimplifySyntaxTree(false)
                .Where(x => startItem.PreviousSyntaxItems().All(y => !y.Equals(x.SimplifiedSyntaxItem)));
            foreach ((ISyntaxItem? item1, string? item2) in lowestCostNextItem) {
                if(item1.GetCost() > Math.Max(startItem.SyntaxTree.GetCost() * 2.5, startItem.SyntaxTree.GetCost() + 50))
                    continue;
                Children.Add(new SimplificationItem(item1, item2, this, rootNode: RootNode ?? this));
                Children.Last().Simplify(startItem);
            }
        }
        
        foreach ((ISyntaxItem? item1, string? item2) in SyntaxTree.SimplifySyntaxTree(true).Where(x => startItem.PreviousSyntaxItems().All(y => !y.Equals(x.SimplifiedSyntaxItem)))) {
            if(item1.GetCost() > Math.Max(startItem.SyntaxTree.GetCost() * 2.5, startItem.SyntaxTree.GetCost() + 50))
                continue;
            Children.Add(new SimplificationItem(item1, item2, this, true, RootNode ?? this));
            Children.Last().Simplify(startItem);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private List<ISyntaxItem> PreviousSyntaxItems() {
        List<ISyntaxItem> returnValue = new() {SyntaxTree};
        foreach(SimplificationItem child in Children) {
            returnValue.AddRange(child.PreviousSyntaxItems());
        }
        return returnValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootNode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SimplificationItem GetSmallestCostSyntaxItem(SimplificationItem rootNode) {
        if (rootNode is null) throw new ArgumentNullException(nameof(rootNode));
        SimplificationItem lowestCostItem = rootNode;

        Queue<SimplificationItem> queue = new();
        queue.Enqueue(rootNode);
        while (queue.Count > 0) {
            SimplificationItem currentItem = queue.Dequeue();
            if (currentItem.SyntaxTree.GetCost() < lowestCostItem.SyntaxTree.GetCost()) {
                lowestCostItem = currentItem;
            }
            foreach (SimplificationItem child in currentItem.Children) {
                queue.Enqueue(child);
            }
        }

        return lowestCostItem;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<SimplificationReason> GetSimplifiedSyntaxTree() {
        List<SimplificationReason> returnValue = new();
        if(ParentItem is not null)
            returnValue.AddRange(ParentItem.GetSimplifiedSyntaxTree());
        returnValue.Add(new SimplificationReason(SyntaxTree, SimplificationReason));
        return returnValue;
    }
}