﻿using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Identifiers;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Parser {
    public sealed class Parser {
        private IEnumerable<Lexeme> Lexemes { get; }
        private Queue<Lexeme> LexemesQueue { get; set; }

        private bool TryIncrementLexemesQueue() {
            return LexemesQueue.TryDequeue(out _);
        }

        public Parser(IEnumerable<Lexeme> lexemes) {
            lexemes = lexemes.ToArray();
            Lexemes = lexemes ?? throw new ArgumentNullException(nameof(lexemes));
            if (lexemes.Any(lexeme => lexeme.Identifier.Equals(IdentifierUtils.UnknownIdentifier)))
                throw new ArgumentException($"The parameter {nameof(lexemes)} must not contain an unknown lexeme");
            LexemesQueue = new Queue<Lexeme>(lexemes);
        }

        public bool TryParse(out SyntaxItem abstractSyntaxTree) {
            LexemesQueue = new Queue<Lexeme>(Lexemes);
            SyntaxItem? parsedSyntaxTree = InternalParse();
            if (parsedSyntaxTree is null) {
                abstractSyntaxTree = new Operand("");
                return false;
            }

            abstractSyntaxTree = parsedSyntaxTree;
            return true;
        }

        private SyntaxItem? InternalParse(uint currentPrecedence = 0, SyntaxItem? previousSyntaxItem = null, IdentifierType endSyntaxIdentifierType = IdentifierType.UNKNOWN) {
            if (currentPrecedence < IdentifierUtils.GetMaximumPrecedence())
                previousSyntaxItem = InternalParse(currentPrecedence + 1, previousSyntaxItem, endSyntaxIdentifierType);
            while (LexemesQueue.TryPeek(out Lexeme? currentLexeme) && currentLexeme.Identifier.IdentifierType != endSyntaxIdentifierType && currentLexeme.Identifier.Precedence == currentPrecedence) {
                previousSyntaxItem = GenerateSyntaxItemFromSyntaxIdentifier(LexemesQueue.Dequeue(), previousSyntaxItem, endSyntaxIdentifierType);
            }
            return previousSyntaxItem;
        }

        private SyntaxItem GenerateSyntaxItemFromSyntaxIdentifier(Lexeme currentLexeme, SyntaxItem? previousSyntaxItem, IdentifierType endSyntaxIdentifierType) {
            SyntaxItem nextSyntaxItem;
            switch (currentLexeme.Identifier.IdentifierType) {
                case IdentifierType.OPERAND:
                    if (previousSyntaxItem is not null)
                        throw new ParserException(currentLexeme.LexemePosition, "(x00)");
                    string value = (currentLexeme as ContextualLexeme)?.LexemeValue ?? throw new ParserException(currentLexeme.LexemePosition, "Was not of type contextual lexeme");
                    nextSyntaxItem = new Operand(value);
                    break;
                case IdentifierType.UNARY_OPERATOR:
                    if (previousSyntaxItem is not null)
                        throw new ParserException(currentLexeme.LexemePosition, "Expected null before (x01)");
                    nextSyntaxItem = InternalParse(currentLexeme.Identifier.Precedence, endSyntaxIdentifierType: endSyntaxIdentifierType) 
                                     ?? throw new ParserException(currentLexeme.LexemePosition, "Expression expected after unary operator");
                    nextSyntaxItem = new UnaryOperator(currentLexeme.Identifier.Name, nextSyntaxItem);
                    break;
                case IdentifierType.BINARY_OPERATOR:
                    if (previousSyntaxItem is null)
                        throw new ParserException(currentLexeme.LexemePosition, "Expected not null before (x03)");
                    List<SyntaxItem> daughterSyntaxItems = new() {previousSyntaxItem};
                    do {
                        daughterSyntaxItems.Add(InternalParse(currentLexeme.Identifier.Precedence + 1, endSyntaxIdentifierType: endSyntaxIdentifierType) 
                            ?? throw new ParserException(currentLexeme.LexemePosition, "Expected not null after (x04)"));
                    } while(NextSyntaxIdentifierIsOfSameLexemeType(currentLexeme.Identifier));
                    nextSyntaxItem = new BinaryOperator(currentLexeme.Identifier.Name, daughterSyntaxItems);
                    break;
                case IdentifierType.GROUPING_OPERATOR_START:
                    if (previousSyntaxItem is not null)
                        throw new ParserException(currentLexeme.LexemePosition, "Expected null before (x05)");
                    nextSyntaxItem = InternalParse(endSyntaxIdentifierType: IdentifierType.GROUPING_OPERATOR_END)
                        ?? throw new ParserException(currentLexeme.LexemePosition, "Expected not null after (x06)");
                    if(!NextSyntaxIdentifierIsOfSameLexemeType(new Identifier(IdentifierType.GROUPING_OPERATOR_END, 4, "RIGHT_PARENTHESIS", "^\\)$", false)))
                        throw new ParserException(currentLexeme.LexemePosition, "Expected right parenthesis after (x07)");
                    break;
                case IdentifierType.GROUPING_OPERATOR_END:
                    throw new ParserException(currentLexeme.LexemePosition, "Unexpected right parenthesis.");
                default:
                    throw new ParserException(currentLexeme.LexemePosition, "An unknown syntax identifier was mistakenly lexed");
            }

            return nextSyntaxItem;
        }

        private bool NextSyntaxIdentifierIsOfSameLexemeType(Identifier identifier) {
            return LexemesQueue.Count > 0
                   && LexemesQueue.Peek().Identifier.Equals(identifier)
                   && TryIncrementLexemesQueue();
        }
    }
}