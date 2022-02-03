namespace BooleanAlgebra.Parser;
/// <summary>
/// Provides a method to produce a syntax tree from a collection of lexemes.
/// </summary>
public sealed class Parser {
    /// <summary>
    /// The collection of lexemes that the syntax tree will be created from.
    /// </summary>
    private IEnumerable<Lexeme> Lexemes { get; }
    /// <summary>
    /// The current queue of lexemes that are being processed so that they can be used to create the syntax tree.
    /// </summary>
    private Queue<Lexeme> LexemesQueue { get; set; }
    /// <summary>
    /// Whether variable operands should be created as substitutable generic operands.
    /// This is used during the creation of simplification rules.
    /// </summary>
    private bool UseGenericOperands { get; }

    /// <summary>
    /// Instantiates a new parser that will be able to produce a syntax tree from a given collection of lexemes.
    /// </summary>
    /// <param name="lexemes">The collection of lexemes that the syntax tree will be created from.</param>
    /// <param name="useGenericOperands">Whether variable operands should be created as substitutable generic operands.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lexemes"/> is null.</exception>
    public Parser(IEnumerable<Lexeme> lexemes, bool useGenericOperands = false) {
        //Creates a new array of lexemes from the collection of lexemes provided.
        Lexemes = lexemes.ToArray() ?? throw new ArgumentNullException(nameof(lexemes));
        //Sets up the lexeme queue with the given lexemes.
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        UseGenericOperands = useGenericOperands;
    }
    
    public ISyntaxItem Parse() {
        //Sets up the lexeme queue with the given lexemes.
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        return InternalParse() ?? throw new ParserException(new LexemePosition(0,0), "The boolean expression must not be empty");
    }

    private ISyntaxItem InternalParse(int currentPrecedence = 0, ISyntaxItem? previousSyntaxItem = null, IdentifierType? endSyntaxIdentifierType = null) {
        //
        if (currentPrecedence < IdentifierUtils.GetMaximumPrecedence())
            previousSyntaxItem = InternalParse(currentPrecedence + 1, previousSyntaxItem, endSyntaxIdentifierType);
        while (LexemesQueue.TryPeek(out Lexeme? currentLexeme) && currentLexeme.Identifier.IdentifierType != endSyntaxIdentifierType && currentLexeme.Identifier.Precedence == currentPrecedence) {
            previousSyntaxItem = GenerateSyntaxItemFromSyntaxIdentifier(LexemesQueue.Dequeue(), previousSyntaxItem, endSyntaxIdentifierType);
        }
        
        return previousSyntaxItem;
    }

    private ISyntaxItem GenerateSyntaxItemFromSyntaxIdentifier(Lexeme currentLexeme, ISyntaxItem? previousSyntaxItem, IdentifierType? endSyntaxIdentifierType = null) {
        ISyntaxItem nextSyntaxItem;
        switch (currentLexeme.Identifier.IdentifierType) {
            case IdentifierType.OPERAND when currentLexeme is ContextualLexeme contextualLexeme:
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the operand");
                nextSyntaxItem = UseGenericOperands && currentLexeme.Identifier.Name != "LITERAL"
                    ? new GenericOperand(currentLexeme.Identifier, contextualLexeme.LexemeValue, contextualLexeme.LexemeValue.StartsWith("Items"))
                    : new Operand(currentLexeme.Identifier, contextualLexeme.LexemeValue);
                break;
            //All of the operands (variables and literals) require context.
            //Otherwise, it would be impossible to tell what the value of a variable or literal was.
            case IdentifierType.OPERAND:
                //If this does occur, it means that there is an issue with my code as it should be impossible to get here.
                throw new InvalidOperationException("The lexeme was not a contextual lexeme");
            case IdentifierType.UNARY_OPERATOR:
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the unary operator");
                nextSyntaxItem = InternalParse(currentLexeme.Identifier.Precedence, endSyntaxIdentifierType: endSyntaxIdentifierType)
                                 ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the unary operator");
                nextSyntaxItem = new UnaryOperator(currentLexeme.Identifier, nextSyntaxItem);
                break;
            case IdentifierType.BINARY_OPERATOR:
                if (previousSyntaxItem is null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression before the binary operator");
                List<ISyntaxItem> childSyntaxItems = new() { previousSyntaxItem };
                do {
                    childSyntaxItems.Add(InternalParse(currentLexeme.Identifier.Precedence + 1, endSyntaxIdentifierType: endSyntaxIdentifierType)
                        ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the binary operator"));
                } while (IsNextSyntaxIdentifierOfSameLexemeType(currentLexeme.Identifier));
                nextSyntaxItem = new BinaryOperator(currentLexeme.Identifier, childSyntaxItems.ToArray());
                nextSyntaxItem.Compress();
                break;
            case IdentifierType.GROUPING_OPERATOR_START:
                string initialGroupingOperatorName = currentLexeme.Identifier.Name;
                if (previousSyntaxItem is not null && initialGroupingOperatorName == "PARENTHESIS")
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the opening parenthesis");
                nextSyntaxItem = InternalParse(endSyntaxIdentifierType: IdentifierType.GROUPING_OPERATOR_END)
                    ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the opening parenthesis");
                if(!IsNextIdentifierOfIdentifierTypeAndName(IdentifierType.GROUPING_OPERATOR_END, initialGroupingOperatorName))
                    throw new ParserException(currentLexeme.LexemePosition, $"The parser expected a closing parenthesis");
                if (initialGroupingOperatorName == "REPEATING") {
                    nextSyntaxItem = new RepeatingOperator(currentLexeme.Identifier, nextSyntaxItem);
                }
                break;
            case IdentifierType.GROUPING_OPERATOR_END:
                throw new ParserException(currentLexeme.LexemePosition, "The parser did not expect a closing parenthesis");
            default:
                throw new InvalidOperationException("An unknown syntax identifier was mistakenly lexed");
        }

        return nextSyntaxItem;
    }

    private bool IsNextSyntaxIdentifierOfSameLexemeType(Identifier identifier) {
        return LexemesQueue.Count > 0
            && LexemesQueue.Peek().Identifier.Equals(identifier)
            && LexemesQueue.TryDequeue(out _);
    }

    private bool IsNextIdentifierOfIdentifierTypeAndName(IdentifierType identifierType, string name) {
        return LexemesQueue.Count > 0
            && LexemesQueue.TryPeek(out Lexeme? currentLexeme)
            && currentLexeme.Identifier.IdentifierType == identifierType
            && currentLexeme.Identifier.Name == name
            && LexemesQueue.TryDequeue(out _);
    }
}