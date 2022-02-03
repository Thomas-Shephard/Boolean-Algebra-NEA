namespace BooleanAlgebra.Simplifier; 
/// <summary>
/// 
/// </summary>
public class SimplificationItem {
    private SimplificationItem? ParentNode { get; }
    
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
    private SimplificationItem RootNode { get; }
    /// <summary>
    /// 
    /// </summary>
    private List<SimplificationItem> Children { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="simplificationReason"></param>
    /// <param name="isInAfterStage"></param>
    /// <param name="rootNode"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public SimplificationItem(ISyntaxItem syntaxTree, string simplificationReason, SimplificationItem? parentNode = null, bool isInAfterStage = false, SimplificationItem? rootNode = null) {
        SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
        SimplificationReason = simplificationReason ?? throw new ArgumentNullException(nameof(simplificationReason));
        ParentNode = parentNode;
        IsInAfterStage = isInAfterStage;
        RootNode = rootNode ?? this;
        Children = new List<SimplificationItem>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startItem"></param>
    public void Simplify(SimplificationItem? startItem = null) {
        startItem ??= this;
        List<ISyntaxItem> previousSyntaxItems = new();
        startItem.GetSyntaxItemsInChildNodes(previousSyntaxItems);
        if (!IsInAfterStage) {
            InternalSimplify(previousSyntaxItems, startItem, false);
        }
        
        InternalSimplify(previousSyntaxItems, startItem, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startItem"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static int MaxAllowedCost(SimplificationItem startItem) {
        if (startItem is null) throw new ArgumentNullException(nameof(startItem));
        //The maximum allowed cost is somewhat arbitrary. It is assumed that if the cost of a tree is greater than the following value, there would be no benefit to simplifying it.
        return (int) Math.Max(startItem.SyntaxTree.GetCost() * 2.5, startItem.SyntaxTree.GetCost() + 50);
    }

    private void InternalSimplify(List<ISyntaxItem> previousSyntaxItems, SimplificationItem startItem, bool isPostSimplification) {
        if (startItem is null) throw new ArgumentNullException(nameof(startItem));

        foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in SyntaxTree.SimplifySyntaxTree(isPostSimplification)
                     .Where(x => previousSyntaxItems.All(y => !y.Equals(x.SimplifiedSyntaxItem)))) {
            if(simplifiedSyntaxItem.GetCost() > MaxAllowedCost(startItem))
                continue;
            Children.Add(new SimplificationItem(simplifiedSyntaxItem, reason, this, isPostSimplification, RootNode));
            Children.Last().Simplify(startItem);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void GetSyntaxItemsInChildNodes(List<ISyntaxItem> foundSyntaxItems) {
        if (foundSyntaxItems is null) throw new ArgumentNullException(nameof(foundSyntaxItems));
        foundSyntaxItems.Add(SyntaxTree);
        foreach (SimplificationItem simplificationItem in Children) {
            simplificationItem.GetSyntaxItemsInChildNodes(foundSyntaxItems);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void GetSimplificationSteps(List<SimplificationRulePair> simplificationSteps) {
        if (simplificationSteps is null) throw new ArgumentNullException(nameof(simplificationSteps));
        ParentNode?.GetSimplificationSteps(simplificationSteps);
        simplificationSteps.Add(new SimplificationRulePair(SyntaxTree, SimplificationReason));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootNode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SimplificationItem GetSmallestCostSyntaxItemWithSmallestDepth(SimplificationItem rootNode) {
        if (rootNode is null) throw new ArgumentNullException(nameof(rootNode));
        SimplificationItem lowestCostItem = rootNode;
        int currentLowestCost = int.MaxValue;

        Queue<SimplificationItem> queue = new();
        queue.Enqueue(rootNode);
        while (queue.Count > 0) {
            SimplificationItem currentItem = queue.Dequeue();
            if (currentItem.SyntaxTree.GetCost() < currentLowestCost) {
                lowestCostItem = currentItem;
                currentLowestCost = lowestCostItem.SyntaxTree.GetCost();
            }
            foreach (SimplificationItem child in currentItem.Children) {
                queue.Enqueue(child);
            }
        }

        return lowestCostItem;
    }
}