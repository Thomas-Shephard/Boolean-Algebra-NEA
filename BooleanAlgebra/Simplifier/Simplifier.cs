namespace BooleanAlgebra.Simplifier;
/// <summary>
/// 
/// </summary>
public class Simplifier {
    /// <summary>
    /// 
    /// </summary>
    private const string StartMessage = "Initial boolean expression";
    /// <summary>
    /// 
    /// </summary>
    private ISyntaxItem StartSyntaxItem { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startSyntaxItem"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Simplifier(ISyntaxItem startSyntaxItem) {
        StartSyntaxItem = startSyntaxItem ?? throw new ArgumentNullException(nameof(startSyntaxItem));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<SimplificationReason> Simplify() {
        SimplificationItem simplifierTree = new(StartSyntaxItem, StartMessage);
        simplifierTree.Simplify();
        
        return SimplificationItem.GetSmallestCostSyntaxItem(simplifierTree).GetSimplifiedSyntaxTree();
    }
}