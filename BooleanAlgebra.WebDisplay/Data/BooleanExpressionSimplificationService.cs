using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;
using BooleanAlgebra.Simplifier;

namespace BooleanAlgebra.WebDisplay.Data;

public static class BooleanExpressionSimplificationService {
    public static async Task<SimplifiedBooleanExpression> SimplifyBooleanExpressionAsync(string booleanExpression) {
        Lexer.Lexer lexer = new(booleanExpression);
        List<Lexeme> lexemes;
        try {
            lexemes = await Task.Run(() => lexer.Lex());
        } catch (UnknownLexemeException unknownLexemeException) {
            return new SimplifiedBooleanExpression(unknownLexemeException);
        }

        Parser.Parser parser = new(lexemes);
        ISyntaxItem parsedSyntaxTree;
        try {
            parsedSyntaxTree = await Task.Run(() => parser.Parse());
        } catch (ParserException parserException) {
            return new SimplifiedBooleanExpression(parserException);
        }
        
        Simplifier.Simplifier simplifier = new(parsedSyntaxTree);
        List<SimplificationRulePair> simplificationOrder = await Task.Run(() => simplifier.Simplify());
        return new SimplifiedBooleanExpression(simplificationOrder);
    }
}