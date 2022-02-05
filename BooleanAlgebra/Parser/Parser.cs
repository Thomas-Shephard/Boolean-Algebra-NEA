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
    
    private string RawText { get; }
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
    public Parser(IEnumerable<Lexeme> lexemes, string rawText, bool useGenericOperands = false) {
        //Creates a new array of lexemes from the collection of lexemes provided.
        Lexemes = lexemes.ToArray() ?? throw new ArgumentNullException(nameof(lexemes));
        RawText = rawText;
        //Sets up the lexeme queue with the given lexemes.
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        UseGenericOperands = useGenericOperands;
    }
    
    public ISyntaxItem Parse() {
        //Sets up the lexeme queue with the given lexemes.
        LexemesQueue = new Queue<Lexeme>(Lexemes);
        //If the generated syntax tree is null, then throw an exception.
        ISyntaxItem generatedSyntaxItem = InternalParse()?? throw new InvalidOperationException("An empty string was mistakenly parsed.");
        generatedSyntaxItem.Compress();
        return generatedSyntaxItem;
    }

    private ISyntaxItem? InternalParse(int currentPrecedence = 0, ISyntaxItem? previousSyntaxItem = null, IdentifierType? endIdentifierType = null) {
        //If the current precedence is less than the maximum precedence of an identifier, recursively parse the expression from an incremented precedence.
        if (currentPrecedence < IdentifierUtils.GetMaximumPrecedence())
            //The new syntax item gained from parsing at an incremented precedence should have the same parameters as that of the currently parsed syntax item.
            //However, the newly parsed syntax item should have an incremented precedence.
            previousSyntaxItem = InternalParse(currentPrecedence + 1, previousSyntaxItem, endIdentifierType);
        //Continue to generate syntax items from the next lexeme in the queue if:
        //  1. A lexeme exists in the queue.
        //  2. The current precedence is equal to the precedence of the found lexeme.
        //  3. The found lexeme is not of the same type as the end identifier type.
        while (LexemesQueue.TryPeek(out Lexeme? currentLexeme) 
               && currentLexeme.Identifier.Precedence == currentPrecedence 
               && currentLexeme.Identifier.IdentifierType != endIdentifierType) {
            //Generate a syntax item from the first lexeme in the queue with the previously parsed syntax item.
            //Keeping the same end identifier type also means that the syntax item will be generated with the same end identifier type.
            previousSyntaxItem = GenerateSyntaxItemFromLexeme(LexemesQueue.Dequeue(), previousSyntaxItem, endIdentifierType);
        }
 
        return previousSyntaxItem;
    }

    private ISyntaxItem GenerateSyntaxItemFromLexeme(Lexeme currentLexeme, ISyntaxItem? previousSyntaxItem, IdentifierType? endIdentifierType = null) {
        ISyntaxItem nextSyntaxItem;
        //Depending on the type of the current lexemes identifier, a different syntax item will be generated.
        switch (currentLexeme.Identifier.IdentifierType) {
            //If the current lexeme is an operand and if the current lexeme has context associated with it, then create a new operand.
            case IdentifierType.OPERAND when currentLexeme is ContextualLexeme contextualLexeme:
                //If a previous syntax item was provided, an error will be thrown.
                //An operand should only have an operator before it and that would be generated at a lower precedence.
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the operand", RawText);
                //If generic operands are being used and if the current lexeme is a variable, create a generic operand.
                nextSyntaxItem = UseGenericOperands && currentLexeme.Identifier.Name == "VARIABLE"
                    //A generic operand has the property is repeating, when it begins with the string 'Items' whereas non-repeating generic operands begin with 'Item'.
                    ? new GenericOperand(currentLexeme.Identifier, contextualLexeme.LexemeValue, contextualLexeme.LexemeValue.StartsWith("Items"))
                    //Otherwise create a standard operand with the identifier and the lexeme value.
                    : new Operand(currentLexeme.Identifier, contextualLexeme.LexemeValue);
                break;
            //All of the operands (variables and literals) require context.
            //Otherwise, it would be impossible to tell what the value of a variable or literal was. Therefore, an error will be thrown.
            case IdentifierType.OPERAND:
                throw new InvalidOperationException("The lexeme was not a contextual lexeme");
            //If the current lexeme is a unary operator, then a syntax item with a child node will be generated.
            case IdentifierType.UNARY_OPERATOR:
                //If a previous syntax item was provided, an error will be thrown.
                //A unary operator should only be nested inside of an existing operator and that would be generated without a previous syntax item.
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the unary operator", RawText);
                //Create a new syntax item at the current precedence and with the same end identifier type.
                //If a new syntax item is not created, an error will be thrown as a unary operator always needs a child node.
                nextSyntaxItem = InternalParse(currentLexeme.Identifier.Precedence, endIdentifierType: endIdentifierType)
                                 ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the unary operator", RawText);
                //Create a new unary operator with the identifier and the previously generated syntax item as a child node.
                nextSyntaxItem = new UnaryOperator(currentLexeme.Identifier, nextSyntaxItem);
                break;
            //If the current lexeme is a binary operator, then a syntax item with two or more child nodes will be generated.
            case IdentifierType.BINARY_OPERATOR:
                //If a previous syntax item was not provided, an error will be thrown.
                //A binary operator is required to have two or more nested syntax items, one of which is required to come before the binary operator.
                //e.g. A + A, the first A comes before the + operator.
                if (previousSyntaxItem is null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression before the binary operator", RawText);
                //All of the child syntax items are added to a list with the previous syntax item as the first item.
                List<ISyntaxItem> childSyntaxItems = new() { previousSyntaxItem };
                //A do while loop is used as there must be at least one more child syntax items for the binary operator to be successfully parsed.
                do {
                    //Add a newly parsed child syntax item to the list with a new precedence of 1 more than the current precedence and the same end identifier type.
                    //The increased precedence is required to ensure that the child syntax items are parsed in the correct order.
                    //If the newly generated child syntax item is null, an error will be thrown.
                    childSyntaxItems.Add(InternalParse(currentLexeme.Identifier.Precedence + 1, endIdentifierType: endIdentifierType)
                        ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the binary operator", RawText));
                //Continue to add new child syntax items whilst the next identifier is the same as the current identifier.
                //A + A + A would have the same identifier as the first is '+' and the second is also '+'.
                } while (IsNextSyntaxIdentifierOfSameLexemeType(currentLexeme.Identifier));
                //Create a new binary operator with the identifier and the child syntax items as child nodes.
                nextSyntaxItem = new BinaryOperator(currentLexeme.Identifier, childSyntaxItems.ToArray());
                break;
            //If the current lexeme is the start of a grouping operator, then group the encapsulated expression together.
            case IdentifierType.GROUPING_OPERATOR_START:
                //If a previous syntax item was provided, an error will be thrown.
                //A grouping operator should only be nested inside of an existing operator and that would be generated without a previous syntax item.
                if (previousSyntaxItem is not null)
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected an operator before the opening parenthesis", RawText);
                //Store the current lexeme's name to determine the type of it later.
                string initialGroupingOperatorName = currentLexeme.Identifier.Name;
                //Create a new syntax item at the initial precedence and with the grouping operator end as the identifier type.
                //The initial precedence is used so that another tree can be created with all the available operators.
                //If a new syntax item is not created, an error will be thrown as a unary operator always needs a child node.
                nextSyntaxItem = InternalParse(endIdentifierType: IdentifierType.GROUPING_OPERATOR_END)
                    ?? throw new ParserException(currentLexeme.LexemePosition, "The parser expected an expression after the opening parenthesis", RawText);
                //If the lexeme after the child node is generated is not a grouping operator end with the initial grouping operator's name, an error will be thrown.
                if(!IsNextIdentifierOfSameIdentifierTypeAndName(IdentifierType.GROUPING_OPERATOR_END, initialGroupingOperatorName))
                    throw new ParserException(currentLexeme.LexemePosition, "The parser expected a closing parenthesis", RawText);
                //If the grouping operator was a repeating operator, then the child node is added as the child node of the new repeating operator.
                //Else, it is left as is.
                if (initialGroupingOperatorName == "REPEATING") {
                    nextSyntaxItem = new RepeatingOperator(currentLexeme.Identifier, nextSyntaxItem);
                }
                break;
            //If the current lexeme is the end of a grouping operator, then an error has occurred as the parser should have already found the start of the operator and set that as the end identifier type.
            case IdentifierType.GROUPING_OPERATOR_END:
                throw new ParserException(currentLexeme.LexemePosition, "The parser did not expect a closing parenthesis", RawText);
            //If the current lexeme is none of the above, then an error has occurred as the lexer has mistakenly produced a lexeme that the parser does not expect.
            default:
                throw new InvalidOperationException("An unknown syntax identifier was mistakenly lexed");
        }

        return nextSyntaxItem;
    }

    private bool IsNextSyntaxIdentifierOfSameLexemeType(Identifier identifier) {
        //The next lexeme only has an identifier equal to the current identifier if:
        //  1. Another lexeme exists in the queue.
        //  2. The next lexeme has an identifier that satisfies the Equals() method of the current identifier.
        //The next lexeme in the queue is then discarded if this is true.
        return LexemesQueue.Count > 0
            && LexemesQueue.Peek().Identifier.Equals(identifier)
            && LexemesQueue.TryDequeue(out _);
    }

    private bool IsNextIdentifierOfSameIdentifierTypeAndName(IdentifierType identifierType, string name) {
        //The next lexeme only has an identifier equal to the current identifier if:
        //  1. Another lexeme exists in the queue.
        //  2. The next lexeme has an identifier that satisfies the Equals() method of the current identifier.
        //The next lexeme in the queue is then discarded if this is true.
        return LexemesQueue.Count > 0
            && LexemesQueue.TryPeek(out Lexeme? currentLexeme)
            && currentLexeme.Identifier.IdentifierType == identifierType
            && currentLexeme.Identifier.Name == name
            && LexemesQueue.TryDequeue(out _);
    }
}