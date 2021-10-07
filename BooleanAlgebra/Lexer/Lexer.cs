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
            List<ILexeme> lexemeList = new();
            Func<char, bool>[] permittedCharacters = { CharUtils.IsDigit, CharUtils.IsVariable };
            uint currentPosition = 0;

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                if (char.IsWhiteSpace(currentCharacter)) {
                    currentPosition++;
                    continue;
                }

                uint startPosition = currentPosition;
                string lexemeValue = GenerateStringFromPattern(rawText, permittedCharacters
                    .FirstOrDefault(permittedCharacter => permittedCharacter(currentCharacter)) ?? CharUtils.IsOtherCharacter, ref currentPosition);

                LexemePosition lexemePosition = new(startPosition, currentPosition);
                LexemeIdentifier lexemeIdentifier = IdentifierUtils.GetLexemeIdentifierFromString(lexemeValue);

                lexemeList.Add(lexemeIdentifier.IsContextRequired 
                    ? new ContextualLexeme(lexemeIdentifier, lexemePosition, lexemeValue)
                    : new ContextFreeLexeme(lexemeIdentifier, lexemePosition));
            }

            return lexemeList;
        }

        private static string GenerateStringFromPattern(string rawText, Func<char, bool> permittedCharacters, ref uint currentPosition) {
            StringBuilder outputString = new();

            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter) && permittedCharacters(currentCharacter)) {
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