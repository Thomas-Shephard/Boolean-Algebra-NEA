namespace BooleanAlgebra.Lexer;
/// <summary>
/// Provides a method to produce a collection of lexemes from a given string.
/// </summary>
public sealed class Lexer {
    /// <summary>
    /// The input string that the lexer will produce a collection of lexemes from.
    /// </summary>
    private string BooleanExpression { get; }
    /// <summary>
    /// Determines whether the lexer should recognise identifiers that can only be used while generating simplification rules.
    /// </summary>
    private bool IsBuildingSimplificationRule { get; }

    /// <summary>
    /// Instantiates a new lexer that will be able to produce a collection of lexemes from a given input string.
    /// </summary>
    /// <param name="booleanExpression">The input string that the lexer will produce a collection of lexemes from.</param>
    /// <param name="isBuildingSimplificationRule">Determines whether the lexer should recognise identifiers that can only be used while generating simplification rules.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="booleanExpression"/> is null.</exception>
    public Lexer(string booleanExpression, bool isBuildingSimplificationRule = false) {
        BooleanExpression = booleanExpression ?? throw new ArgumentNullException(nameof(booleanExpression));
        IsBuildingSimplificationRule = isBuildingSimplificationRule;
    }

    /// <summary>
    /// Produces a collection of lexemes from the input string.
    /// </summary>
    /// <returns>A collection of lexemes produced from the input string.</returns>
    public List<Lexeme> Lex() {
        List<Lexeme> lexemes = new();
        //The start position in the input string is the zeroth index.
        int currentPosition = 0;

        //While a character exists at the current position in the input string, attempt to produce a lexeme from it.
        while (TryGetCharacterAtPosition(currentPosition, out char currentCharacter)) {
            //Skip over whitespace characters as they are not the constituents of any lexemes.
            if (char.IsWhiteSpace(currentCharacter)) {
                currentPosition++;
                continue;
            }

            //The start position of a lexeme will be equal to the position within the string before the symbol is generated.
            int lexemeStartPosition = currentPosition;
            
            //Finds the first lexeme pattern that matches the current character, defaults to null if none found.
            LexemePattern? lexemePattern = LexemePattern.GetLexemePatternFromCharacterLexemePattern(currentCharacter);
            
            //Generates a symbol from the found lexeme pattern.
            //If no lexeme pattern was found, the generated symbol will be the current character.
            string symbol = GenerateSymbolFromLexemePattern(lexemePattern, ref currentPosition);

            //The end position of the lexeme will be equal to the position within the string after the symbol is generated.
            LexemePosition lexemePosition = new(lexemeStartPosition, currentPosition);
            
            //Attempt to find an identifier that matches the generated symbol.
            //If no identifier is found, an exception will be thrown as an unrecognised symbol was entered by the user.
            if(!IdentifierUtils.TryGetIdentifierFromSymbol(symbol, IsBuildingSimplificationRule, out Identifier? matchedIdentifier))
                throw new UnknownLexemeException(lexemePosition, BooleanExpression);

            //Add a new lexeme to the collection of lexemes.
            lexemes.Add(matchedIdentifier.IsContextRequired
                //Only provide contextual information if the identifier requires it.
                ? new ContextualLexeme(matchedIdentifier, lexemePosition, symbol)
                : new Lexeme(matchedIdentifier, lexemePosition));
        }
        
        return lexemes;
    }

    /// <summary>
    /// Generates a symbol from the characters after the given position that match the given lexeme pattern.
    /// </summary>
    /// <param name="lexemePattern">The pattern that the input text should be matched against.</param>
    /// <param name="currentPosition">The position that the lexer is at within the input text.</param>
    /// <returns>The symbol generated from the characters after the given position that match the given lexeme pattern.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="currentPosition"/> is outside of the range of the input text.</exception>
    private string GenerateSymbolFromLexemePattern(LexemePattern? lexemePattern, ref int currentPosition) {
        StringBuilder generatedSymbol = new();

        //Ensure that a character exists at the specified position within the input text.
        if (!TryGetCharacterAtPosition(currentPosition, out char currentCharacter))
            throw new ArgumentException($"The input string has no characters at the specified position", nameof(currentPosition));

        //Even if the lexeme pattern is null, the initial character is still added to the generated symbol.
        generatedSymbol.Append(currentCharacter);
        currentPosition++;
        if (lexemePattern is null)
            return generatedSymbol.ToString();
        //If the lexeme pattern is not null, continue to add characters to the generated symbol
        //until either the end of the input text is reached or the next character no longer matches the lexeme pattern.
        while (TryGetCharacterAtPosition(currentPosition, out currentCharacter) && lexemePattern.IsCharacterMatch(currentCharacter)) {
            generatedSymbol.Append(currentCharacter);
            currentPosition++;
        }
        return generatedSymbol.ToString();
    }

    /// <summary>
    /// Indicates whether the a given character exists at a given position.
    /// Provides the character at the given position if a character does exist at the position.
    /// </summary>
    /// <param name="currentPosition">The position within the input string that should be checked.</param>
    /// <param name="currentCharacter">The character at the specified position, '\0' when no character exists at the position.</param>
    /// <returns>True when the specified position is within the bounds of the input text.</returns>
    private bool TryGetCharacterAtPosition(int currentPosition, out char currentCharacter) {
        if (currentPosition >= 0 && currentPosition < BooleanExpression.Length) {
            //If the currentPosition is within the bounds of rawText, return true and output the character at the requested position.
            currentCharacter = BooleanExpression[currentPosition];
            return true;
        }

        //If the currentPosition is not within the bounds of rawText, return false and output the default character value.
        currentCharacter = default;
        return false;
    }
}