namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds a collection of acceptable characters and character bounds.
/// The lexer uses the collection of acceptable characters to assemble a lexeme.
/// </summary>
public sealed class LexemePattern {
    /// <summary>
    /// An array of the accepted characters corresponding char values.
    /// </summary>
    private readonly int[] _acceptedCharacters;
    /// <summary>
    /// An array of the bounds of the accepted characters corresponding char values.
    /// </summary>
    private readonly Tuple<int, int>[] _acceptedCharacterBounds;

    /// <summary>
    /// Instantiates a new acceptable collection of characters.
    /// </summary>
    /// <param name="acceptedCharacters">An array of the accepted characters corresponding char values.</param>
    /// <param name="acceptedCharacterBounds">An array of the bounds of the accepted characters corresponding char values.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="acceptedCharacters"/> or <paramref name="acceptedCharacterBounds"/> is null.</exception>
    private LexemePattern(int[] acceptedCharacters, Tuple<int, int>[] acceptedCharacterBounds) {
        _acceptedCharacters = acceptedCharacters ?? throw new ArgumentNullException(nameof(acceptedCharacters));
        _acceptedCharacterBounds = acceptedCharacterBounds ?? throw new ArgumentNullException(nameof(acceptedCharacterBounds));
    }

    /// <summary>
    /// Indicates whether a given character is within the collection of acceptable characters.
    /// </summary>
    /// <param name="character">The character to check as to whether it is inside the collection of acceptable characters.</param>
    /// <returns>True when the <paramref name="character"/> is within the collection of acceptable characters.</returns>
    public bool IsCharacterMatch(char character) {
        //Checks if the array of acceptable characters contains the queried character.
        return _acceptedCharacters.Contains(character)
            //Checks if the array of acceptable character bounds contains an entry where the queried character is within the bounds.
            || _acceptedCharacterBounds.Any(acceptedCharBound => character >= acceptedCharBound.Item1 && character <= acceptedCharBound.Item2);
    }

    /// <summary>
    /// Returns a collection of the lexemePatterns that the lexer can use to assemble a lexeme.
    /// </summary>
    /// <returns>A collection of the lexemePatterns that the lexer can use to assemble a lexeme.</returns>
    private static IEnumerable<LexemePattern> GetLexemePatterns() {
        return new[] {
            //A pattern that matches a character from '0' to '9'.
            new LexemePattern(Array.Empty<int>(),new[] { new Tuple<int, int>('0', '9') }),
            //A pattern that matches a character from 'a' to 'z', 'A' to 'Z' and '_'.
            new LexemePattern(new int[] { '_' }, new[] { new Tuple<int, int>('A', 'Z'), new Tuple<int, int>('a', 'z') }),
        };
    }

    /// <summary>
    /// Attempts to find a lexeme pattern that matches a given character.
    /// </summary>
    /// <param name="character">The character to find a potential lexeme pattern match for.</param>
    /// <returns>Null if no lexeme pattern can be found that matches the <paramref name="character"/> else returns the first lexeme pattern that matches the <paramref name="character"/>.</returns>
    public static LexemePattern? GetLexemePatternFromCharacterLexemePattern(char character) {
        //Gets the first lexeme pattern that is a match for the specified character.
        //FirstOrDefault will return null if no lexeme pattern is found that matches the specified character.
        return GetLexemePatterns().FirstOrDefault(lexemePattern => lexemePattern.IsCharacterMatch(character));
    }
}