namespace BooleanAlgebra.Identifiers;
public class Identifier : IEquatable<Identifier> {
    public IdentifierType IdentifierType { get; }
    public uint Precedence { get; }
    public string Name { get; }
    public Regex Regex { get; }
    private string RegexPattern { get; }
    public bool IsContextRequired { get; }

    public Identifier(IdentifierType identifierType, uint precedence, string name, string regexPattern, bool isContextRequired) {
        IdentifierType = identifierType;
        Precedence = precedence;
        Name = name;
        RegexPattern = regexPattern;
        Regex = new Regex(regexPattern);
        IsContextRequired = isContextRequired;
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
               && IsContextRequired == other.IsContextRequired;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return obj.GetType() == GetType() 
               && Equals((Identifier) obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int) IdentifierType, Precedence, Name, RegexPattern, IsContextRequired);
    }
}