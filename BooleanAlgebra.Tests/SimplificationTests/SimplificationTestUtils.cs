namespace BooleanAlgebra.Tests.SimplificationTests;
public static class SimplificationTestUtils {
    public static bool Compare(string input, string expected) {
        //Lexing the input and expected
        List<Lexeme> inputLexemes = new Lexer.Lexer(input).Lex();
        List<Lexeme> expectedLexemes = new Lexer.Lexer(expected).Lex();
        //Parsing the input and expected
        ISyntaxItem parsedInput = new Parser.Parser(inputLexemes, input).Parse();
        ISyntaxItem parsedExpected = new Parser.Parser(expectedLexemes, expected).Parse();
        //Simplifying the input
        List<SimplificationRulePair> simplificationSteps = new Simplifier.Simplifier(parsedInput).Simplify();
        //Comparing the simplified input to the expected
        return simplificationSteps.Last().SimplifiedSyntaxItem.Equals(parsedExpected);
    }
}