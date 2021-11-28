namespace BooleanAlgebra.Lexer;
/// <summary>
/// Provides a method to lex a given input string.
/// </summary>
public sealed class Lexer {
    /// <summary>
    /// The input string to be lexed by the lexer.
    /// </summary>
    private string BooleanExpression { get; }
    private bool UseGenericOperands { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexer"/> class with a given input string.
    /// </summary>
    /// <param name="booleanExpression">The input string to be lexed by the lexer.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rawText"/> is null.</exception>
    public Lexer(string booleanExpression, bool useGenericOperands = false) {
        BooleanExpression = booleanExpression ?? throw new ArgumentNullException(nameof(booleanExpression));  //Ensure that rawText is not null
        UseGenericOperands = useGenericOperands;
    }

    /// <summary>
    /// Indicates whether the input text has been entirely lexed into known lexemes.
    /// Provides the list of lexemes that the input text has been lexed into regardless of the return value.
    /// </summary>
    /// <returns>True when the input text has been entirely lexed into known lexemes.</returns>
    public List<Lexeme> Lex() {
        List<Lexeme> lexemes = new();   //Initialize out parameter lexemes to an empty list
        int currentPosition = 0;       //Set the currentPosition equal to the startPosition (i.e. 0)

        //Check if a character exists at the currentPosition
        while (TryGetCharacterAtPosition(currentPosition, out char currentCharacter)) {
            //Skip over whitespace characters as they provide no information to the lexer
            if (char.IsWhiteSpace(currentCharacter)) {
                currentPosition++;
                continue;
            }

            //Attempt to find the first lexemePattern that matches the currentCharacter, defaults to null if none found
            LexemePattern? lexemePattern = LexemePattern.GetLexemePatterns().FirstOrDefault(pattern => pattern.IsCharacterMatch(currentCharacter));

            //The startPosition will be equal to the currentPosition before the lexeme value is generated
            int startPosition = currentPosition;

            //Generate the lexeme value from the first pattern that matches the currentCharacter
            string lexemeValue = GenerateLexemeValueFromPattern(lexemePattern, ref currentPosition);

            //The endPosition will be equal to the currentPosition after the lexeme value is generated
            LexemePosition lexemePosition = new(startPosition, currentPosition);

            //Attempt to find an identifier that matches the lexemeValue
            if(!IdentifierUtils.TryGetIdentifierFromLexemeValue(lexemeValue, UseGenericOperands, out Identifier? matchedIdentifier))
                throw new UnknownLexemeException(lexemePosition, BooleanExpression);

            //Add a lexeme to the out parameter lexemes
            lexemes.Add(matchedIdentifier.IsContextRequired
                //Only provide the lexemeValue if context is required
                ? new ContextualLexeme(matchedIdentifier, lexemePosition, lexemeValue)
                : new Lexeme(matchedIdentifier, lexemePosition));
        }

        //If any lexeme within the out parameter lexemes is unknown then the function will return false
        return lexemes;
    }

    /// <summary>
    /// Produces a lexemeValue from the <paramref name="currentPosition"/> within the <paramref name="rawText"/> and the <paramref name="lexemePattern"/>.
    /// </summary>
    /// <param name="lexemePattern">The pattern that the input text should be matched against.</param>
    /// <param name="currentPosition">The position that the lexer is at within the input text.</param>
    /// <returns>The lexemeValue that comes from the <paramref name="currentPosition"/> within the input text and the <paramref name="lexemePattern"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when there are no characters in the input string after the <paramref name="currentPosition"/>.</exception>
    private string GenerateLexemeValueFromPattern(LexemePattern? lexemePattern, ref int currentPosition) {
        StringBuilder outputString = new();

        //Ensure that a character exists at the specified position
        if (!TryGetCharacterAtPosition(currentPosition, out char currentCharacter))
            throw new ArgumentException($"The property {nameof(BooleanExpression)} has no characters at the specified position");

        do {
            outputString.Append(currentCharacter);  //Add the currentCharacter to the output string
            currentPosition++;                      //Increment the currentPosition
            if (lexemePattern is null) break;       //If the lexemePattern is null only return the first character
        } while (TryGetCharacterAtPosition(currentPosition, out currentCharacter)   //Ensure that a character exists at the specified position
                 && lexemePattern.IsCharacterMatch(currentCharacter));              //Ensure that the currentCharacter matches the desired pattern

        return outputString.ToString();
    }

    /// <summary>
    /// Indicates whether the <paramref name="currentPosition"/> exists within the input text.
    /// Provides the character at the <paramref name="currentPosition"/> within the input text.
    /// </summary>
    /// <param name="currentPosition">The position that the lexer is at within the input text.</param>
    /// <param name="currentCharacter">The character at the specified position when it is within the bounds of the input text.</param>
    /// <returns>True when the specified position is within the bounds of the input text.</returns>
    private bool TryGetCharacterAtPosition(int currentPosition, out char currentCharacter) {
        if (currentPosition < BooleanExpression.Length) {
            //If the currentPosition is within the bounds of rawText, return true and output the character at the requested position
            currentCharacter = BooleanExpression[currentPosition];
            return true;
        }

        //If the currentPosition is not within the bounds of rawText, return false and output the default character value
        currentCharacter = default;
        return false;
    }
}