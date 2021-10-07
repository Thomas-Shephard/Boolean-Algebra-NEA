using System;
using System.Collections.Generic;
using System.Text;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Syntax.Identifiers;
using BooleanAlgebra.Utils;

namespace BooleanAlgebra.Lexer {
    public static class Lexer {
        public static IEnumerable<ILexeme> Lex(string rawText) {
            if (rawText is null)
                throw new ArgumentNullException(nameof(rawText));
            List<ILexeme> lexemeList = new();
            uint currentPosition = 0;

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if (char.IsWhiteSpace(currentCharacter)) {
                    currentPosition++;
                    continue;
                }

                uint startPosition = currentPosition;
                string lexemeValue;

                if (currentCharacter.IsDigit()) {
                    lexemeValue = GenerateStringFromPattern(rawText, CharUtils.IsDigit, ref currentPosition);
                } else if (currentCharacter.IsVariable()) {
                    lexemeValue = GenerateStringFromPattern(rawText, CharUtils.IsVariable, ref currentPosition);
                } else {
                    lexemeValue = currentCharacter.ToString();
                    currentPosition++;
                }
                
                LexemePosition lexemePosition = new(startPosition, currentPosition);
                LexemeIdentifier lexemeIdentifier = IdentifierUtils.GetLexemeIdentifierFromString(lexemeValue);

                if (lexemeIdentifier.IsContextRequired) {
                    lexemeList.Add(new ContextualLexeme(lexemeIdentifier, lexemePosition, lexemeValue));
                } else {
                    lexemeList.Add(new ContextFreeLexeme(lexemeIdentifier, lexemePosition));
                }
            }

            return lexemeList;
        }

        private static string GenerateStringFromPattern(string rawText, Func<char, bool> isCharacterMatch, ref uint currentPosition) {
            StringBuilder outputString = new();

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if (!isCharacterMatch(currentCharacter)) break;
                outputString.Append(currentCharacter);
                currentPosition++;
            }

            return outputString.ToString();
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