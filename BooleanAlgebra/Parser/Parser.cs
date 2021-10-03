using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser.Syntax;
using BooleanAlgebra.Parser.Syntax.Identifiers;
using BooleanAlgebra.Parser.Syntax.Operands;
using BooleanAlgebra.Parser.Syntax.Operators;

namespace BooleanAlgebra.Parser {
    public static class Parser {
        public static SyntaxItem? Parse(IReadOnlyList<ILexeme> lexemes) {
            uint x = 0;
            return InternalParse(lexemes, ref x);
        }

        private static SyntaxItem? InternalParse(IReadOnlyList<ILexeme> lexemes, ref uint currentPosition, uint currentPrecedence = 0, LexemeIdentifier? endLexemeIdentifier = null, SyntaxItem? previousSyntaxItem = null) {
            if(currentPrecedence < SyntaxIdentifier.GetMaximumPrecedence())
                previousSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence + 1, endLexemeIdentifier, previousSyntaxItem);
            
            if(!TryGetLexemeAtPosition(lexemes, currentPosition, out ILexeme currentLexeme) || currentLexeme.LexemeIdentifier.Equals(endLexemeIdentifier))
                return previousSyntaxItem;

            if (TryGetSyntaxIdentifierFromLexeme(currentLexeme, currentPrecedence, out SyntaxIdentifier currentSyntaxIdentifier)) {
                ContextualLexeme? contextualLexeme = null;
                if (currentLexeme.LexemeIdentifier.IsContextRequired) {
                    if (currentLexeme is not ContextualLexeme tempContextualLexeme) 
                        throw new ParserException(currentLexeme.LexemePosition, "Was not of type contextual lexeme");
                    contextualLexeme = tempContextualLexeme;
                }

                SyntaxItem? nextSyntaxItem;
                switch (currentSyntaxIdentifier.SyntaxIdentifierType) {
                    case SyntaxIdentifierType.LITERAL_OPERAND:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null");
                        if(contextualLexeme is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Was not of type contextual lexeme");
                        currentPosition++;
                        previousSyntaxItem = new BooleanOperand(contextualLexeme.LexemeValue);
                        break;
                    case SyntaxIdentifierType.VARIABLE_OPERAND:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null");
                        if(contextualLexeme is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Was not of type contextual lexeme");
                        currentPosition++;
                        previousSyntaxItem = new VariableOperand(contextualLexeme.LexemeValue);
                        break;
                    case SyntaxIdentifierType.BINARY_OPERATOR:
                        if (previousSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null");
                        List<SyntaxItem> syntaxItems = new() {previousSyntaxItem};
                        do {
                            currentPosition++;
                            nextSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence + 1, endLexemeIdentifier);
                            if (nextSyntaxItem is null)
                                throw new ParserException(currentLexeme.LexemePosition,
                                    $"Expected operand after highlighted token @ position '{currentLexeme.LexemePosition}'");
                            syntaxItems.Add(nextSyntaxItem);
                        } while (TryGetLexemeAtPosition(lexemes, currentPosition, out currentLexeme)
                                 && TryGetSyntaxIdentifierFromLexeme(currentLexeme, currentPrecedence, out SyntaxIdentifier tempSyntaxIdentifier)
                                 && currentSyntaxIdentifier.Equals(tempSyntaxIdentifier));

                        previousSyntaxItem = new BinaryOperator(currentSyntaxIdentifier.LexemeIdentifier.Name, syntaxItems);
                        break;
                    case SyntaxIdentifierType.UNARY_OPERATOR:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null");
                        currentPosition++;
                        nextSyntaxItem = InternalParse(lexemes, ref currentPosition, currentPrecedence, endLexemeIdentifier);
                        if (nextSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null");
                        previousSyntaxItem = new UnaryOperator(currentLexeme.LexemeIdentifier.Name, nextSyntaxItem);
                        break;
                    case SyntaxIdentifierType.GROUPING_OPERATOR:
                        if (previousSyntaxItem is not null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected null");
                        currentPosition++;
                        nextSyntaxItem = InternalParse(lexemes, ref currentPosition, 0, LexemeIdentifier.RIGHT_PARENTHESIS);
                        if (nextSyntaxItem is null)
                            throw new ParserException(currentLexeme.LexemePosition, "Expected not null");
                        if (!TryGetLexemeAtPosition(lexemes, currentPosition, out currentLexeme) ||
                            !currentLexeme.LexemeIdentifier.Equals(LexemeIdentifier.RIGHT_PARENTHESIS))
                            throw new ParserException(currentLexeme.LexemePosition,$"Expected right parenthesis after highlighted token");
                        currentPosition++;
                        previousSyntaxItem = new GroupingOperator(nextSyntaxItem);
                        break;
                    default:
                        throw new ParserException(currentLexeme.LexemePosition, "Unknown token");
                }
                
            }

            if (currentPrecedence == 0 && currentLexeme is not null && !currentLexeme.LexemeIdentifier.Equals(endLexemeIdentifier)) {
                if (previousSyntaxItem is null)
                    throw new ParserException(currentLexeme.LexemePosition,
                        $"Unexpected token (highlighted) @ position '{currentLexeme.LexemePosition}', was expecting a unary operator");
                throw new ParserException(currentLexeme.LexemePosition,
                    $"Unexpected token (highlighted) @ position '{currentLexeme.LexemePosition}', was expecting a binary operator");
            }
            
            return previousSyntaxItem;
        }
        
        private static bool TryGetSyntaxIdentifierFromLexeme(ILexeme currentLexeme, uint currentPrecedence, out SyntaxIdentifier currentSyntaxIdentifier) {
            currentSyntaxIdentifier = SyntaxIdentifier.GetSyntaxIdentifiers().FirstOrDefault(syntaxIdentifier => syntaxIdentifier.LexemeIdentifier.Equals(currentLexeme.LexemeIdentifier) && syntaxIdentifier.Precedence == currentPrecedence) ?? SyntaxIdentifier.UNKNOWN;
            return currentSyntaxIdentifier.SyntaxIdentifierType is not SyntaxIdentifierType.UNKNOWN;
        }
        
        private static bool TryGetLexemeAtPosition(IReadOnlyList<ILexeme> lexemes, uint currentPosition, out ILexeme currentLexeme) {
            if (currentPosition < lexemes.Count && currentPosition < int.MaxValue) {
                currentLexeme = lexemes[(int)currentPosition];
                return true;
            }

            currentLexeme = new ContextFreeLexeme(LexemeIdentifier.UNKNOWN, new LexemePosition(currentPosition));
            return false;
        }
    }
}