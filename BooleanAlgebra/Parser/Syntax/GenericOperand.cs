namespace BooleanAlgebra.Parser.Syntax; 
/// <summary>
/// 
/// </summary>
public class GenericOperand : Operand {
    /// <summary>
    /// 
    /// </summary>
    public bool IsRepeating { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isRepeating"></param>
    public GenericOperand(string value, Identifier identifier, bool isRepeating) : base(value, identifier) {
        IsRepeating = isRepeating;
    }

    private GenericOperand(GenericOperand genericOperand) : base(genericOperand) {
        IsRepeating = genericOperand.IsRepeating;
    }
    
    public override ISyntaxItem ShallowClone() {
        return new GenericOperand(this);
    }

    public override bool Equals(ISyntaxItem? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return other is GenericOperand otherGenericOperand
            && Value == otherGenericOperand.Value
            && IsRepeating == otherGenericOperand.IsRepeating
            && Identifier.Equals(otherGenericOperand.Identifier);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as GenericOperand);
    }
    
    public override int GetHashCode() {
        //The hash code is not supported for syntax items to ensure that equality is not based off the order of any child nodes.
        throw new NotSupportedException("GetHashCode() is not supported for syntax items.");
    }
}