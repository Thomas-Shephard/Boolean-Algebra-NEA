namespace BooleanAlgebra.Lexer.Lexemes;
/// <summary>
/// Holds the accepted characters and the bounds of the accepted characters corresponding values.
/// Contains the lexemePatterns that the lexer should compare against.
/// </summary>
public sealed class LexemePattern {
    /// <summary>
    /// An array of the accepted characters corresponding unicode values.
    /// </summary>
    private readonly int[] _acceptedCharacters;
    /// <summary>
    /// An array of the bounds of the accepted characters corresponding unicode values.
    /// </summary>
    private readonly Tuple<int, int>[] _acceptedCharacterBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="LexemePattern"/> class with an <param name="acceptedCharacters"/> array and an <param name="acceptedCharacterBounds"/> array.
    /// </summary>
    /// <param name="acceptedCharacters">An array of the accepted characters corresponding unicode values.</param>
    /// <param name="acceptedCharacterBounds">An array of the bounds of the accepted characters corresponding unicode values.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="acceptedCharacters"/> or <paramref name="acceptedCharacterBounds"/> is null.</exception>
    private LexemePattern(int[] acceptedCharacters, Tuple<int, int>[] acceptedCharacterBounds) {
        _acceptedCharacters = acceptedCharacters ?? throw new ArgumentNullException(nameof(acceptedCharacters));                //Ensure that the acceptedCharacters array is not null
        _acceptedCharacterBounds = acceptedCharacterBounds ?? throw new ArgumentNullException(nameof(acceptedCharacterBounds)); //Ensure that the acceptedCharacterBounds array is not null
    }

    /// <summary>
    /// Indicates whether the <paramref name="character"/> is a match for the current <see cref="LexemePattern"/>.
    /// </summary>
    /// <param name="character">The character to check against.</param>
    /// <returns>True when the <paramref name="character"/> is a match for the current <see cref="LexemePattern"/>.</returns>
    public bool IsCharacterMatch(char character) {
        //Check if acceptedCharacters array contains the currentCharacter
        return _acceptedCharacters.Contains(character)
            //Check if any item in the acceptedCharacterBounds array is in the range of the currentCharacter
            || _acceptedCharacterBounds.Any(acceptedCharBound => character >= acceptedCharBound.Item1 && character <= acceptedCharBound.Item2);
    }

    /// <summary>
    /// Returns an enumerable collection of the lexemePatterns that the lexer should compare against.
    /// </summary>
    /// <returns>An enumerable collection of the lexemePatterns that the lexer should compare against.</returns>
    public static IEnumerable<LexemePattern> GetLexemePatterns() {
        return new[] {
            //A pattern equal to [0-9]+
            new LexemePattern(Array.Empty<int>(),new[] { new Tuple<int, int>('0', '9') }),
            //A pattern equal to [A-Za-z_]+
            new LexemePattern(new int[] { '_' }, new[] { new Tuple<int, int>('A', 'Z'), new Tuple<int, int>('a', 'z') }),
        };
    }
}