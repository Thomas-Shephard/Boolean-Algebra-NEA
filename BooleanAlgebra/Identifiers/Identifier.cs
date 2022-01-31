namespace BooleanAlgebra.Identifiers;
/// <summary>
/// Holds the information about an identifier so that it can be used by the lexer and parser.
/// </summary>
public class Identifier : IEquatable<Identifier> {
    /// <summary>
    /// The type of an identifier. Used to determine how to build the abstract syntax tree.
    /// </summary>
    public IdentifierType IdentifierType { get; }
    /// <summary>
    /// Determines the precedence at which matched lexemes are parsed into the abstract syntax tree.
    /// </summary>
    public int Precedence { get; }
    /// <summary>
    /// The name of the identifier.
    /// Used as a string representation of the identifier.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The regular expression pattern that is used to generate the regular expression.
    /// This is stored separately from the regular expression itself as it is used for equality comparison.
    /// </summary>
    private string RegexPattern { get; }
    /// <summary>
    /// The regular expression that a given lexeme value will be matched against.
    /// </summary>
    public Regex Regex { get; }
    /// <summary>
    /// Determines whether the lexer will provide contextual information about the lexeme.
    /// </summary>
    public bool IsContextRequired { get; }
    /// <summary>
    /// Determines whether the identifier can only be used while generating simplification rules.
    /// </summary>
    public bool IsSimplificationRuleSpecific { get; }
    
    /// <summary>
    /// Instantiates a new identifier with the properties set to the given parameters.
    /// </summary>
    /// <param name="identifierType">The type that an identifier is.</param>
    /// <param name="precedence">The precedence at which lexemes that match to the identifier will be parsed into the abstract syntax tree.</param>
    /// <param name="name">The name of the identifier.</param>
    /// <param name="regexPattern">The regular expression pattern that a given lexeme value will be matched against.</param>
    /// <param name="isContextRequired">Determines whether the lexer will provide contextual information about the lexeme.</param>
    /// <param name="isSimplificationRuleSpecific">Determines whether identifier can only be used while generating simplification rules.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="name"/> or <paramref name="regexPattern"/> is null.</exception>
    public Identifier(IdentifierType identifierType, int precedence, string name, string regexPattern, bool isContextRequired, bool isSimplificationRuleSpecific) {
        IdentifierType = identifierType;
        Precedence = precedence;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(regexPattern));
        //Initialises a new instance of the Regex class with the specified regular expression pattern.
        Regex = new Regex(regexPattern);
        IsContextRequired = isContextRequired;
        IsSimplificationRuleSpecific = isSimplificationRuleSpecific;
    }

    public bool Equals(Identifier? otherIdentifier) {
        if (ReferenceEquals(null, otherIdentifier)) return false; //If the other identifier is null, it cannot be equal to this identifier.
        if (ReferenceEquals(this, otherIdentifier)) return true;  //If the other identifier is this identifier, it is equal to this identifier.
        
        //The identifiers are only equal if their properties are equal to each other.
        //The string representation of the regular expression is used instead of the regular expression itself as Regex objects do not implement the IEquatable interface.
        return IdentifierType == otherIdentifier.IdentifierType 
               && Precedence == otherIdentifier.Precedence 
               && Name == otherIdentifier.Name 
               && RegexPattern == otherIdentifier.RegexPattern 
               && IsContextRequired == otherIdentifier.IsContextRequired
               && IsSimplificationRuleSpecific == otherIdentifier.IsSimplificationRuleSpecific;
    }

    public override bool Equals(object? obj) {
        //The other object is only equal to this identifier:
        //  1. If the other object is an identifier.
        //  2. If the other identifier satisfies the equality comparison.
        return obj is Identifier otherIdentifier && Equals(otherIdentifier);
    }
    
    public override int GetHashCode() {
        //Provides a hash code for the identifier that takes the properties from the current object.
        //The underlying integer is used instead of the identifier type as, under the hood, an enum is an integer.
        //The string representation of the regular expression is used instead of the regular expression itself as Regex objects do not implement the GetHashCode method.
        return HashCode.Combine((int) IdentifierType, Precedence, Name, RegexPattern, IsContextRequired, IsSimplificationRuleSpecific);
    }
}