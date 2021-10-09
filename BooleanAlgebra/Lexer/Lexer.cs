using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooleanAlgebra.Identifiers;
using BooleanAlgebra.Lexer.Lexemes;

namespace BooleanAlgebra.Lexer {
    public static class Lexer {
        public static bool Lex(string rawText, out List<Lexeme> lexemes) {
            if (rawText is null) throw new ArgumentNullException(nameof(rawText));
            lexemes = new List<Lexeme>();
            uint currentPosition = 0;

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if (char.IsWhiteSpace(currentCharacter)) {
                    currentPosition++;
                    continue;
                }

                uint startPosition = currentPosition;
                
                LexemePattern? characterPattern = LexemePattern.GetLexemePatterns().FirstOrDefault(x => x.IsCharacterMatch(currentCharacter));

                string lexemeValue;
                if (characterPattern is null) {
                    currentPosition++;
                    lexemeValue = currentCharacter.ToString();
                } else {
                    lexemeValue = GenerateStringFromPattern(rawText, characterPattern, ref currentPosition);
                }

                LexemePosition lexemePosition = new(startPosition, currentPosition);
                LexemeIdentifier lexemeIdentifier = IdentifierUtils.GetLexemeIdentifierFromString(lexemeValue);
                
                lexemes.Add(lexemeIdentifier.IsContextRequired 
                    ? new ContextualLexeme(lexemeIdentifier, lexemePosition, lexemeValue)
                    : new Lexeme(lexemeIdentifier, lexemePosition));
            }

            return !lexemes.Any(lexeme => lexeme.LexemeIdentifier.Equals(IdentifierUtils.LEXEME_UNKNOWN));
        }

        private static string GenerateStringFromPattern(string rawText, LexemePattern lexemePattern, ref uint currentPosition) {
            StringBuilder outputString = new();

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if(!lexemePattern.IsCharacterMatch(currentCharacter)) break;
                outputString.Append(currentCharacter);
                currentPosition++;
            }

            return outputString.ToString();
        }

        private static bool TryGetCharacterAtPosition(string rawText, uint currentPosition, out char currentCharacter) {
            if (currentPosition < rawText.Length) {
                currentCharacter = rawText[(int) currentPosition];
                return true;
            }

            currentCharacter = default;
            return false;
        }
    }
}