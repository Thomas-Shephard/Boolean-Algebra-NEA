using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanAlgebra.Utils {
    public static class CharUtils {
        public static IEnumerable<CharacterPattern> GetCharacterPatterns() {
            return new[] {
                new CharacterPattern(new[] { '.' }, new[] { new Tuple<int, int>('0', '9') }),
                new CharacterPattern(new[] { '_' }, new[] { new Tuple<int, int>('A', 'Z'), new Tuple<int, int>('a', 'z') }),
            };
        }
    }

    public class CharacterPattern {
        private readonly char[] _acceptedCharacters;
        private readonly Tuple<int, int>[] _acceptedCharacterBounds;

        public CharacterPattern(char[] acceptedCharacters, Tuple<int, int>[] acceptedCharacterBounds) {
            _acceptedCharacters = acceptedCharacters;
            _acceptedCharacterBounds = acceptedCharacterBounds;
        }

        public bool IsCharMatch(char character) {
            return _acceptedCharacters.Contains(character) || _acceptedCharacterBounds.Any(acceptedCharBound => character >= acceptedCharBound.Item1 && character <= acceptedCharBound.Item2);
        }
    }
}