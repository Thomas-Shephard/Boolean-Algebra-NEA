using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Syntax.Identifiers;
using BooleanAlgebra.Utils;

namespace BooleanAlgebra.Lexer {
    public static class Lexer {
        public static IEnumerable<ILexeme> Lex(string rawText) {
            if (rawText is null)
                throw new ArgumentNullException(nameof(rawText));
            uint currentPosition = 0;

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if (char.IsWhiteSpace(currentCharacter)) {
                    currentPosition++;
                    continue;
                }

                uint startPosition = currentPosition;
                string lexemeValue;

                if (currentCharacter.IsDigit()) {
                    lexemeValue = GenerateNumber(rawText, ref currentPosition);
                } else if (currentCharacter.IsVariable()) {
                    lexemeValue = GenerateVariable(rawText, ref currentPosition);
                } else {
                    lexemeValue = currentCharacter.ToString();
                    currentPosition++;
                }
                
                LexemePosition lexemePosition = new(startPosition, currentPosition);
                LexemeIdentifier lexemeIdentifierMatch = IdentifierUtils.GetLexemeIdentifiers()
                                                        .FirstOrDefault(lexemeIdentifier => lexemeIdentifier.Regex.IsMatch(lexemeValue))
                                                        ?? IdentifierUtils.LEXEME_ERROR;
                
                if (lexemeIdentifierMatch.IsContextRequired) {
                    yield return new ContextualLexeme(lexemeIdentifierMatch, lexemePosition, lexemeValue);
                } else {
                    yield return new ContextFreeLexeme(lexemeIdentifierMatch, lexemePosition);
                }
            }
        }

        private static string GenerateVariable(string rawText, ref uint currentPosition) {
            StringBuilder outputVariable = new();

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter) &&
                   currentCharacter.IsVariable()) {
                outputVariable.Append(currentCharacter);
                currentPosition++;
            }

            return outputVariable.ToString();
        }

        private static string GenerateNumber(string rawText, ref uint currentPosition) {
            StringBuilder outputNumber = new();

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter) &&
                   currentCharacter.IsDigit()) {
                outputNumber.Append(currentCharacter);
                currentPosition++;
            }

            return outputNumber.ToString();
        }

        private static bool TryGetCharacterAtPosition(string rawText, uint currentPosition, out char currentCharacter) {
            if (currentPosition < rawText.Length && currentPosition < int.MaxValue) {
                currentCharacter = rawText[(int) currentPosition];
                return true;
            }

            currentCharacter = default;
            return false;
        }
    }
}