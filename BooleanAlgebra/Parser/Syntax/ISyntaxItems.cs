namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// 
/// </summary>
public interface ISyntaxItem : IEquatable<ISyntaxItem> {
    /// <summary>
    /// 
    /// </summary>
    public Identifier Identifier { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetCost();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="higherLevelPrecedence"></param>
    /// <returns></returns>
    public string GetStringRepresentation(int higherLevelPrecedence = 0);
}

/// <summary>
/// 
/// </summary>
public interface ISingleDaughterSyntaxItem : ISyntaxItem {
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem Daughter { get; }
}

/// <summary>
/// 
/// </summary>
public interface IMultipleDaughterSyntaxItem : ISyntaxItem {
    /// <summary>
    /// 
    /// </summary>
    public ISyntaxItem[] Daughters { get; }
}