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
    /// Determines whether the lexer will provide context to the lexeme.
    /// </summary>
    public bool IsContextRequired { get; }
    /// <summary>
    /// An identifier with this property set to true should only be matched when the lexer is assembling a syntax tree for the simplification algorithm.
    /// </summary>
    public bool IsSimplificationRuleSpecific { get; }
    
    /// <summary>
    /// Instantiates a new identifier with the properties set to the given parameters.
    /// </summary>
    /// <param name="identifierType">The type that an identifier is.</param>
    /// <param name="precedence">The precedence at which lexemes that match to the identifier will be parsed into the abstract syntax tree.</param>
    /// <param name="name">The name of the identifier.</param>
    /// <param name="regexPattern">The regular expression pattern that a given lexeme value will be matched against.</param>
    /// <param name="isContextRequired">Determines whether the lexer will provide context to the lexeme.</param>
    /// <param name="isSimplificationRuleSpecific">Determines whether the lexer and parser will only recognise the identifier when assembling a syntax tree for the simplification algorithm.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="name"/> or <paramref name="regexPattern"/> is null.</exception>
    public Identifier(IdentifierType identifierType, int precedence, string name, string regexPattern, bool isContextRequired, bool isSimplificationRuleSpecific) {
        //Initializes the properties of the identifier based on the parameters from the constructor.
        IdentifierType = identifierType;
        Precedence = precedence;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(regexPattern));
        //Initializes a new instance of the Regex class with the specified regular expression pattern.
        Regex = new Regex(regexPattern);
        IsContextRequired = isContextRequired;
        IsSimplificationRuleSpecific = isSimplificationRuleSpecific;
    }

    public bool Equals(Identifier? other) {
        if (ReferenceEquals(null, other)) return false; //If the other identifier is null, it cannot be equal to this identifier.
        if (ReferenceEquals(this, other)) return true;  //If the other identifier is this identifier, it is equal to this identifier.
        
        //The identifiers are only equal if their properties are equal to each other.
        //The string representation of the regular expression is used instead of the regular expression itself as Regex objects do not implement the Equals method.
        return IdentifierType == other.IdentifierType 
               && Precedence == other.Precedence 
               && Name == other.Name 
               && RegexPattern == other.RegexPattern 
               && IsContextRequired == other.IsContextRequired
               && IsSimplificationRuleSpecific == other.IsSimplificationRuleSpecific;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;   //If the object is null, it cannot be equal to this object.
        if (ReferenceEquals(this, obj)) return true;    //If the object is this object, it is equal to this object.
        
        //Compare the other object as an identifier with a more specific comparison.
        //If the other object is not an identifier, the comparison will fail.
        return Equals(obj as Identifier);
    }
    
    public override int GetHashCode() {
        //Provides a hash code for the identifier that takes the properties from the current object.
        //The underlying integer is used instead of the identifier type as, under the hood, an enum is an integer.
        //The string representation of the regular expression is used instead of the regular expression itself as Regex objects do not implement the GetHashCode method.
        return HashCode.Combine((int) IdentifierType, Precedence, Name, RegexPattern, IsContextRequired, IsSimplificationRuleSpecific);
    }
}