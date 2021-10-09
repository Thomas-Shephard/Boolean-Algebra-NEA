using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Lexer {
    public class LexemePattern {
        private readonly char[] _acceptedCharacters;
        private readonly Tuple<int, int>[] _acceptedCharacterBounds;

        private LexemePattern(char[] acceptedCharacters, Tuple<int, int>[] acceptedCharacterBounds) {
            _acceptedCharacters = acceptedCharacters;
            _acceptedCharacterBounds = acceptedCharacterBounds;
        }

        public bool IsCharacterMatch(char character) {
            return _acceptedCharacters.Contains(character) || _acceptedCharacterBounds.Any(acceptedCharBound => character >= acceptedCharBound.Item1 && character <= acceptedCharBound.Item2);
        }
        
        public static IEnumerable<LexemePattern> GetLexemePatterns() {
            return new[] {
                new LexemePattern(new[] { '.' }, new[] { new Tuple<int, int>('0', '9') }),
                new LexemePattern(new[] { '_' }, new[] { new Tuple<int, int>('A', 'Z'), new Tuple<int, int>('a', 'z') }),
            };
        }
    }
}