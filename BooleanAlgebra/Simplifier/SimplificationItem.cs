namespace BooleanAlgebra.Simplifier; 
/// <summary>
/// Holds information about a node within the simplification tree.
/// Allows for the nodes to be traversed in a depth-first manner.
/// </summary>
public class SimplificationItem {
    /// <summary>
    /// Holds the simplified original syntax tree and the rule that was used to simplify it.
    /// </summary>
    private SimplificationRulePair SimplificationRulePair { get; }
    /// <summary>
    /// The root node of the simplification tree.
    /// </summary>
    private SimplificationItem RootNode { get; }
    /// <summary>
    /// The parent node of the current node in the simplification tree.
    /// Is null when the current node is the root node.
    /// </summary>
    private SimplificationItem? ParentNode { get; }
    /// <summary>
    /// All of the child nodes of the current node that have been created by simplifying the current node.
    /// </summary>
    private List<SimplificationItem> ChildNodes { get; }
    /// <summary>
    /// Whether the simplifier should only apply post simplification simplification rules.
    /// </summary>
    private bool IsInPostSimplificationStage { get; }

    /// <summary>
    /// Initialises a new simplification node with the specified simplification rule pair.
    /// </summary>
    /// <param name="simplificationRulePair">A record containing the simplified syntax item and the rule that was applied.</param>
    /// <param name="rootNode">The root node in the simplification tree.</param>
    /// <param name="parentNode">The parent node of the newly created simplification node (null if root node)</param>
    /// <param name="isInPostSimplificationStage">Whether the simplifier should only apply post simplification simplification rule.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="simplificationRulePair"/> is null.</exception>
    public SimplificationItem(SimplificationRulePair simplificationRulePair, SimplificationItem? rootNode = null, SimplificationItem? parentNode = null, bool isInPostSimplificationStage = false) {
        SimplificationRulePair = simplificationRulePair ?? throw new ArgumentNullException(nameof(simplificationRulePair));
        RootNode = rootNode ?? this;
        ParentNode = parentNode;
        ChildNodes = new List<SimplificationItem>();
        IsInPostSimplificationStage = isInPostSimplificationStage;
    }

    /// <summary>
    /// Simplifies the current node of the simplification tree and creates new child nodes for each rule that is applied.
    /// </summary>
    public void Simplify() {
        //Storing the list of previously found syntax items boosts efficiency as otherwise, it would need to be determined in each call to InternalSimplify().
        List<ISyntaxItem> previousSyntaxItems = new();
        RootNode.GetSyntaxItemsInChildNodes(previousSyntaxItems);
        //If the current node is not in the post simplification stage, then we should apply both the pre and post simplification rules.
        //If the current node is in the post simplification stage, then we should only apply the post simplification rules.
        if (!IsInPostSimplificationStage) {
            InternalSimplify(previousSyntaxItems, false);
        }
        InternalSimplify(previousSyntaxItems, true);
    }

    /// <summary>
    /// Returns an integer representing the maximum cost that a syntax tree can have.
    /// This is done to prevent the algorithm from continually expanding the tree when it should instead be decreasing the cost.
    /// </summary>
    /// <param name="rootNode">The root node of the simplification tree that the maximum cost should be based upon.</param>
    /// <returns>An integer representing the maximum cost that a syntax tree can have.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rootNode"/> is null.</exception>
    private static int MaxAllowedCost(SimplificationItem rootNode) {
        if (rootNode is null) throw new ArgumentNullException(nameof(rootNode));
        //The maximum allowed cost is somewhat arbitrary.
        //It is assumed that if the cost of a tree is greater than the calculated value, there would be no benefit to simplifying it.
        return (int) Math.Max(rootNode.SimplificationRulePair.SimplifiedSyntaxItem.GetCost() * 2.5, rootNode.SimplificationRulePair.SimplifiedSyntaxItem.GetCost() + 50);
    }

    /// <summary>
    /// Gets all of the possible simplifications from the current node and creates a new child node for each one that is then further simplified.
    /// </summary>
    /// <param name="previousSyntaxItems">All of the previous simplifications of the root node in the simplification tree.</param>
    /// <param name="isPostSimplification">Whether the method should be applying pre or post simplification rule.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="previousSyntaxItems"/> is null.</exception>
    private void InternalSimplify(List<ISyntaxItem> previousSyntaxItems, bool isPostSimplification) {
        if (previousSyntaxItems is null) throw new ArgumentNullException(nameof(previousSyntaxItems));
        //Iterate over all of the simplifications that can be performed on the current node.
        foreach ((ISyntaxItem simplifiedSyntaxItem, string reason) in SimplificationRulePair.SimplifiedSyntaxItem.SimplifySyntaxTree(isPostSimplification)) {
            //If the simplified syntax item is the same as any of the previous items, then simplifying it would potentially cause an infinite loop.
            //As the simplifier may expand and then re-simplify the same item perpetually.
            if (previousSyntaxItems.Any(previousSyntaxItem => previousSyntaxItem.Equals(simplifiedSyntaxItem)))
                continue;
            //If the cost of the new tree is more than the maximum allowed cost, then the tree is not simplified.
            if(simplifiedSyntaxItem.GetCost() > MaxAllowedCost(RootNode))
                continue;
            //Add the new simplified form of the original syntax tree to the simplification tree.
            ChildNodes.Add(new SimplificationItem(new SimplificationRulePair(simplifiedSyntaxItem, reason), RootNode, this, isPostSimplification));
            //Simplify the newly created simplification node.
            ChildNodes.Last().Simplify();
        }
    }

    /// <summary>
    /// Updates the provided list to contain all the child nodes of the current node.
    /// </summary>
    /// <param name="foundSyntaxItems">The list that will be updated to contain all of the child nodes of the current node.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="foundSyntaxItems"/> is null.</exception>
    private void GetSyntaxItemsInChildNodes(List<ISyntaxItem> foundSyntaxItems) {
        if (foundSyntaxItems is null) throw new ArgumentNullException(nameof(foundSyntaxItems));
        //Add the current node's syntax tree to the list of found syntax items.
        foundSyntaxItems.Add(SimplificationRulePair.SimplifiedSyntaxItem);
        //Iterate over all of the child nodes of the current node and recursively call this method on them.
        //When the node is a leaf node, no child nodes exist and therefore no recursive call is made.
        foreach (SimplificationItem simplificationItem in ChildNodes) {
            simplificationItem.GetSyntaxItemsInChildNodes(foundSyntaxItems);
        }
    }
    
    /// <summary>
    /// Updates the provided list to contain the simplification steps that have taken place to go from the root node of the simplification tree to the current node.
    /// </summary>
    /// <param name="simplificationSteps">The list that will be updated to contain the simplification steps.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="simplificationSteps"/> is null.</exception>
    public void GetSimplificationSteps(List<SimplificationRulePair> simplificationSteps) {
        if (simplificationSteps is null) throw new ArgumentNullException(nameof(simplificationSteps));
        //If the parent node is not null, then recursively call the parent node's GetSimplificationSteps method.
        //When the parent node is null, then the current node is the root node and the recursion ends.
        ParentNode?.GetSimplificationSteps(simplificationSteps);
        //Add the current node's simplification rule to the list of simplification steps.
        //The simplification step is added after the recursive call to the parent node's GetSimplificationSteps method.
        //This is done to ensure that the simplification steps are added to the list in the correct order.
        simplificationSteps.Add(SimplificationRulePair);
    }

    /// <summary>
    /// Gets the smallest cost syntax tree in the simplification tree that took the shortest number of steps to simplify.
    /// Performs a breadth-first search to find the smallest cost syntax tree.
    /// </summary>
    /// <param name="rootNode">The node in the simplification tree that is the starting place for the simplification.</param>
    /// <returns>The node in the simplification tree that has the smallest cost and then is of the smallest depth.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rootNode"/> is null.</exception>
    public static SimplificationItem GetSmallestCostSyntaxTreeWithSmallestDepth(SimplificationItem rootNode) {
        if (rootNode is null) throw new ArgumentNullException(nameof(rootNode));
        //Start with the lowest cost item being the root node so that if no other item is found, the root node is returned.
        SimplificationItem lowestCostItem = rootNode;
        //The lowest cost is stored independently from the simplification item to avoid unnecessary checks on the cost of the item.
        int currentLowestCost = int.MaxValue;

        //The following is a breadth-first search, which is done so that the lowest cost syntax tree at the lowest depth is returned.
        //As a lower depth is preferred, the resulting syntax tree will be simplified in the shortest number of steps of those calculated by the simplifier.
        Queue<SimplificationItem> queue = new();
        queue.Enqueue(rootNode);
        //While the queue is not empty (and therefore non-searched child nodes remain) continue to find the lowest cost item.
        while (queue.Count > 0) {
            //Dequeue the next item in the queue and calculate its cost.
            SimplificationItem currentItem = queue.Dequeue();
            int currentItemCost = currentItem.SimplificationRulePair.SimplifiedSyntaxItem.GetCost();
            //If the dequeued item's cost is less than the current lowest cost, then the current lowest cost item is updated.
            if (currentItemCost < currentLowestCost) {
                lowestCostItem = currentItem;
                currentLowestCost = currentItemCost;
            }
            //Add all of the current item's child nodes to the queue so that they can also be searched.
            foreach (SimplificationItem child in currentItem.ChildNodes) {
                queue.Enqueue(child);
            }
        }
        //Return the simplification item with the lowest cost.
        return lowestCostItem;
    }
}