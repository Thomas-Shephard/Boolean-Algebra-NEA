using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;
using BooleanAlgebra.Simplification;

namespace BooleanAlgebra.WebDisplay.Data;

public class BooleanExpressionSimplificationService {
    public BooleanExpressionSimplificationService() {
        _ = SimplificationRule.GetSimplificationRules();
    }
    
    public static async Task<SimplifiedBooleanExpression> SimplifyBooleanExpressionAsync(string booleanExpression) {
        Lexer.Lexer lexer = new(booleanExpression);
        List<Lexeme> lexemes;
        try {
            lexemes = await Task.Run(() => lexer.InternalLex());
        } catch (LexerException lexerException) {
            return SimplifiedBooleanExpression.LexingError(lexerException);
        }

        Parser.Parser parser = new(lexemes);
        SyntaxItem parsedSyntaxTree;
        try {
            parsedSyntaxTree = await Task.Run(() => parser.Parse());
        } catch (ParserException parserException) {
            return SimplifiedBooleanExpression.ParsingError(parserException);
        }
        
        Simplifier.Simplifier simplifier = new(parsedSyntaxTree);
        List<Tuple<SyntaxItem, string>> simplificationOrder = await Task.Run(() => simplifier.Simplify());
        return new SimplifiedBooleanExpression(true, simplificationOrder, simplificationOrder.First().Item1);
    }
}