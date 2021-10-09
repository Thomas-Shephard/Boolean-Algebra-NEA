﻿using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Syntax;
using BooleanAlgebra.Syntax.Identifiers;
using BooleanAlgebra.Syntax.Operands;
using BooleanAlgebra.Syntax.Operators;

namespace BooleanAlgebra.Parser {
    public static class Parser {
        public static SyntaxItem? Parse(IReadOnlyList<ILexeme> lexemes) {
            uint currentPosition = 0;
            return InternalParse(lexemes, ref currentPosition);
        }

        private static SyntaxItem? InternalParse(IReadOnlyList<ILexeme> lexemes, ref uint currentPosition, uint currentPrecedence = 0, LexemeIdentifier? endLexemeIdentifier = null, SyntaxItem? previousSyntaxItem = null) {
            if (lexemes.Any(lexeme => lexeme.LexemeIdentifier.Equals(IdentifierUtils.LEXEME_UNKNOWN)))
                throw new ArgumentException($"The parameter {nameof(lexemes)} must not contain an unknown lexeme.");
            if(currentPrecedence < IdentifierUtils.GetMaximumSyntaxIdentifierPrecedence())
                previousSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence + 1, endLexemeIdentifier, previousSyntaxItem);
            
            while(TryGetLexemeAtPosition(lexemes, currentPosition, out ILexeme currentLexeme)) {
                if (currentLexeme.LexemeIdentifier.Equals(endLexemeIdentifier))
                    break;
                if (!TryGetSyntaxIdentifierFromLexeme(currentLexeme, currentPrecedence, out ISyntaxIdentifier currentSyntaxIdentifier))
                    break;
                
                currentPosition++;
                SyntaxItem? nextSyntaxItem;
                switch (currentSyntaxIdentifier.SyntaxIdentifierType) {
                    case SyntaxIdentifierType.OPERAND:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null before (x000)");
                        previousSyntaxItem = new Operand((currentLexeme as ContextualLexeme)?.LexemeValue ?? throw new ParserException(currentLexeme.LexemePosition, "Was not of type contextual lexeme"));
                        break;
                    case SyntaxIdentifierType.BINARY_OPERATOR:
                        if (previousSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null before (x001)");
                        nextSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence + 1, endLexemeIdentifier);
                        if (nextSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null after (x002)");
                        previousSyntaxItem = new BinaryOperator(currentSyntaxIdentifier.GetLexemeIdentifiers().First().Name, previousSyntaxItem, nextSyntaxItem);
                        break;
                    case SyntaxIdentifierType.UNARY_OPERATOR:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null before (x003)");
                        nextSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence, endLexemeIdentifier);
                        if (nextSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null after (x004)");
                        previousSyntaxItem = new UnaryOperator(currentLexeme.LexemeIdentifier.Name, nextSyntaxItem);
                        break;
                    case SyntaxIdentifierType.GROUPING_OPERATOR:
                        LexemeIdentifier endIdentifier = currentSyntaxIdentifier.GetLexemeIdentifiers().Last();
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null before (x005)");
                        nextSyntaxItem = InternalParse(lexemes, ref currentPosition, 0, endIdentifier);
                        if (nextSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null after (x006)");
                        if (!TryGetLexemeAtPosition(lexemes, currentPosition, out currentLexeme) ||
                            !currentLexeme.LexemeIdentifier.Equals(endIdentifier))
                            throw new ParserException(currentLexeme.LexemePosition,"Expected right parenthesis after highlighted token");
                        currentPosition++;
                        previousSyntaxItem = new GroupingOperator(nextSyntaxItem);
                        break;
                    default:
                        throw new ParserException(currentLexeme.LexemePosition, "Unknown token");
                }
            }

            return previousSyntaxItem;
        }
        
        private static bool TryGetSyntaxIdentifierFromLexeme(ILexeme currentLexeme, uint currentPrecedence, out ISyntaxIdentifier currentSyntaxIdentifier) {
            currentSyntaxIdentifier = IdentifierUtils.GetSyntaxIdentifiers().FirstOrDefault(syntaxIdentifier => syntaxIdentifier.GetLexemeIdentifiers().Any(lexemeIdentifier => Equals(lexemeIdentifier, currentLexeme.LexemeIdentifier)) && syntaxIdentifier.Precedence == currentPrecedence) 
                                      ?? IdentifierUtils.SYNTAX_UNKNOWN;
            return currentSyntaxIdentifier.SyntaxIdentifierType is not SyntaxIdentifierType.UNKNOWN;
        }
        
        private static bool TryGetLexemeAtPosition(IReadOnlyList<ILexeme> lexemes, uint currentPosition, out ILexeme currentLexeme) {
            if (currentPosition < lexemes.Count && currentPosition < int.MaxValue) {
                currentLexeme = lexemes[(int)currentPosition];
                return true;
            }

            currentLexeme = new ContextFreeLexeme(IdentifierUtils.LEXEME_UNKNOWN, new LexemePosition(currentPosition));
            return false;
        }
    }
}