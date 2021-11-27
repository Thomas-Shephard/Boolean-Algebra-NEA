namespace BooleanAlgebra.Parser;
public sealed class Parser {
    private IEnumerable<Lexeme> Lexemes { get; }
    private Queue<Lexeme> LexemesQueue { get; set; }
    private bool UseGenericOperands { get; }

    public Parser(IEnumerable<Lexeme> lexemes, bool useGenericOperands = false) {
        Lexemes = lexemes.ToArray() ?? throw new ArgumentNullException(nameof(lexemes));
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        UseGenericOperands = useGenericOperands;
    }

    public SyntaxItem Parse() {
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        return InternalParse() ?? throw new ParserException(new LexemePosition(0,0), "The boolean expression must not be empty");
    }

    private SyntaxItem? InternalParse(int currentPrecedence = 0, SyntaxItem? previousSyntaxItem = null, IdentifierType endSyntaxIdentifierType = IdentifierType.UNKNOWN) {
        if (currentPrecedence < IdentifierUtils.GetMaximumPrecedence())
            previousSyntaxItem = InternalParse(currentPrecedence + 1, previousSyntaxItem, endSyntaxIdentifierType);
        while (LexemesQueue.TryPeek(out Lexeme? currentLexeme) && currentLexeme.Identifier.IdentifierType != endSyntaxIdentifierType && currentLexeme.Identifier.Precedence == currentPrecedence) {
            previousSyntaxItem = GenerateSyntaxItemFromSyntaxIdentifier(LexemesQueue.Dequeue(), previousSyntaxItem, endSyntaxIdentifierType);
        }
        return previousSyntaxItem;
    }

    private SyntaxItem GenerateSyntaxItemFromSyntaxIdentifier(Lexeme currentLexeme, SyntaxItem? previousSyntaxItem, IdentifierType endSyntaxIdentifierType) {
        SyntaxItem nextSyntaxItem;
        switch (currentLexeme.Identifier.IdentifierType) {
            case IdentifierType.OPERAND when currentLexeme is ContextualLexeme contextualLexeme:
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the operand");
                nextSyntaxItem = UseGenericOperands && currentLexeme.Identifier.Name != "LITERAL"
                    ? new GenericOperand(contextualLexeme.LexemeValue)
                    : new Operand(contextualLexeme.LexemeValue);
                break;
            case IdentifierType.OPERAND:
                throw new InvalidOperationException("The lexeme was not a contextual lexeme");
            case IdentifierType.UNARY_OPERATOR:
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the unary operator");
                nextSyntaxItem = InternalParse(currentLexeme.Identifier.Precedence, endSyntaxIdentifierType: endSyntaxIdentifierType)
                                 ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the unary operator");
                nextSyntaxItem = new UnaryOperator(currentLexeme.Identifier.Name, nextSyntaxItem);
                break;
            case IdentifierType.BINARY_OPERATOR:
                if (previousSyntaxItem is null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression before the binary operator");
                List<SyntaxItem> daughterSyntaxItems = new() { previousSyntaxItem };
                do {
                    daughterSyntaxItems.Add(InternalParse(currentLexeme.Identifier.Precedence + 1, endSyntaxIdentifierType: endSyntaxIdentifierType)
                        ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the binary operator"));
                } while (NextSyntaxIdentifierIsOfSameLexemeType(currentLexeme.Identifier));
                nextSyntaxItem = new BinaryOperator(currentLexeme.Identifier.Name, daughterSyntaxItems);
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
                    nextSyntaxItem = new RepeatingOperator("", nextSyntaxItem);
                }
                break;
            case IdentifierType.GROUPING_OPERATOR_END:
                throw new ParserException(currentLexeme.LexemePosition, "The parser did not expect a closing parenthesis");
            case IdentifierType.UNKNOWN:
            default:
                throw new InvalidOperationException("An unknown syntax identifier was mistakenly lexed");
        }

        return nextSyntaxItem;
    }

    private bool NextSyntaxIdentifierIsOfSameLexemeType(Identifier identifier) {
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