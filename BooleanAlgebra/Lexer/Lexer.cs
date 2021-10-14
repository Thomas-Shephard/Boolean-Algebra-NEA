using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooleanAlgebra.Identifiers;
using BooleanAlgebra.Lexer.Lexemes;

namespace BooleanAlgebra.Lexer {
    /// <summary>
    /// Provides a method to lex a given string.
    /// </summary>
    public static class Lexer {
        /// <summary>
        /// Indicates whether the <paramref name="rawText"/> has been entirely lexed into known lexemes.
        /// Provides the list of lexemes that the <paramref name="rawText"/> has been lexed into regardless of the return value.
        /// </summary>
        /// <param name="rawText">The string to be lexed by the lexer.</param>
        /// <param name="lexemes">The list of lexemes produced from the <paramref name="rawText"/>.</param>
        /// <returns>True when the <paramref name="rawText"/> has been entirely lexed into known lexemes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rawText"/> is null.</exception>
        public static bool Lex(string rawText, out List<Lexeme> lexemes) {
            if (rawText is null) throw new ArgumentNullException(nameof(rawText));  //Ensure that rawText is not null
            lexemes = new List<Lexeme>();   //Initialize out parameter lexemes to an empty list
            uint currentPosition = 0;       //Set the currentPosition equal to the startPosition (i.e. 0)

            //Check if a character exists within the rawText string at the currentPosition
            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                //Skip over whitespace characters as they provide no information
                if (char.IsWhiteSpace(currentCharacter)) {
                    currentPosition++;
                    continue;
                }

                //The currentPosition will be equal to the startPosition of the lexeme before the currentPosition is incremented
                uint startPosition = currentPosition;
                
                //Attempt to find the first lexemePattern that matches the currentCharacter
                //If no lexemePatterns match then lexemePattern is null
                LexemePattern? lexemePattern = LexemePattern.GetLexemePatterns().FirstOrDefault(pattern => pattern.IsCharacterMatch(currentCharacter));
                
                string lexemeValue;
                if (lexemePattern is null) {
                    //If no lexemePattern is found set the lexemeValue equal to the currentCharacter and increment the currentPosition
                    currentPosition++;  
                    lexemeValue = currentCharacter.ToString();
                } else {
                    //If a lexemePattern is found generate a string of concatenated characters that match the lexemePattern
                    lexemeValue = GenerateStringFromPattern(rawText, lexemePattern, ref currentPosition);
                }

                //Initialize the lexemePosition with the previously declared startPosition and the currentPosition which will
                //be equal to the endPosition as no more increments on the currentPosition will occur until the next iteration occurs
                LexemePosition lexemePosition = new(startPosition, currentPosition);
                LexemeIdentifier lexemeIdentifier = IdentifierUtils.GetLexemeIdentifierFromString(lexemeValue);
                
                //Add a lexeme to the out parameter lexemes
                lexemes.Add(lexemeIdentifier.IsContextRequired 
                    //Only provide the lexemeValue if context is required
                    ? new ContextualLexeme(lexemeIdentifier, lexemePosition, lexemeValue)
                    : new Lexeme(lexemeIdentifier, lexemePosition));
            }

            //If any lexeme within the out parameter lexemes is unknown then the function will return false
            return !lexemes.Any(lexeme => lexeme.LexemeIdentifier.Equals(IdentifierUtils.LEXEME_UNKNOWN));
        }

        /// <summary>
        /// Produces a lexemeValue from the <paramref name="currentPosition"/> within the <paramref name="rawText"/> and the <paramref name="lexemePattern"/>.
        /// </summary>
        /// <param name="rawText">The string to be lexed by the lexer.</param>
        /// <param name="lexemePattern">The pattern that the <paramref name="rawText"/> should be matched against.</param>
        /// <param name="currentPosition">The currentPosition that the lexer is at within <paramref name="rawText"/>.</param>
        /// <returns>The lexemeValue that comes from the <paramref name="currentPosition"/> within the <paramref name="rawText"/> and the <paramref name="lexemePattern"/>.</returns>
        private static string GenerateStringFromPattern(string rawText, LexemePattern lexemePattern, ref uint currentPosition) {
            StringBuilder outputString = new();

            //Check if a character exists within the rawText string at the currentPosition
            while (TryGetCharacterAtPosition(rawText, currentPosition, out char currentCharacter)) {
                //Check if the lexemePattern is a match for the currentCharacter
                //Append the current character if it is a match
                if(!lexemePattern.IsCharacterMatch(currentCharacter)) break;
                outputString.Append(currentCharacter);
                currentPosition++;
            }

            return outputString.ToString();
        }

        /// <summary>
        /// Indicates whether the <paramref name="currentPosition"/> exists within the <paramref name="rawText"/>.
        /// Provides the character at the <paramref name="currentPosition"/> within the <paramref name="rawText"/>.
        /// </summary>
        /// <param name="rawText">The string to be lexed by the lexer.</param>
        /// <param name="currentPosition">The currentPosition that the lexer is at within <paramref name="rawText"/>.</param>
        /// <param name="currentCharacter">The character at the <paramref name="currentPosition"/> when it is within the bounds of <paramref name="rawText"/> else the default character value.</param>
        /// <returns>True when the <paramref name="currentPosition"/> is within the bounds of <paramref name="rawText"/>.</returns>
        private static bool TryGetCharacterAtPosition(string rawText, uint currentPosition, out char currentCharacter) {
            if (currentPosition < rawText.Length) {
                //If the currentPosition is within the bounds of rawText, return true and output the character at the requested position
                currentCharacter = rawText[(int) currentPosition];
                return true;
            }

            //If the currentPosition is not within the bounds of rawText, return false and output the default character value
            currentCharacter = default;
            return false;
        }
    }
}