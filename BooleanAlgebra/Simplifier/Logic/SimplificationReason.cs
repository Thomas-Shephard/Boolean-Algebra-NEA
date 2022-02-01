namespace BooleanAlgebra.Simplifier.Logic;
/// <summary>
/// 
/// </summary>
/// <param name="SimplifiedSyntaxItem"></param>
/// <param name="Reason"></param>
public record SimplificationReason(ISyntaxItem SimplifiedSyntaxItem, string Reason) {
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem SimplifiedSyntaxItem { get; } = SimplifiedSyntaxItem;
    /// <summary>
    /// 
    /// </summary>
    public string Reason { get; } = Reason;
}