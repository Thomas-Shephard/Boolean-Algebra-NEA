namespace BooleanAlgebra.Identifiers;
public class Identifier : IEquatable<Identifier> {
    public IdentifierType IdentifierType { get; }
    public int Precedence { get; }
    public string Name { get; }
    private string RegexPattern { get; }
    public Regex Regex { get; }
    public bool IsContextRequired { get; }
    public bool IsGenericOperand { get; }

    public Identifier(IdentifierType identifierType, int precedence, string name, string regexPattern, bool isContextRequired, bool isGenericOperand) {
        IdentifierType = identifierType;
        Precedence = precedence;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(regexPattern));
        Regex = new Regex(regexPattern);
        IsContextRequired = isContextRequired;
        IsGenericOperand = isGenericOperand;
    }

    public override string ToString() {
        return Name;
    }
    
    public bool Equals(Identifier? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return IdentifierType == other.IdentifierType 
               && Precedence == other.Precedence 
               && Name == other.Name 
               && RegexPattern == other.RegexPattern 
               && IsContextRequired == other.IsContextRequired
               && IsGenericOperand == other.IsGenericOperand;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return Equals(obj as Identifier);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int) IdentifierType, Precedence, Name, RegexPattern, IsContextRequired, IsGenericOperand);
    }
}