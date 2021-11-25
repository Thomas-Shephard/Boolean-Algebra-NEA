using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Tests; 

public static class SimplificationUtils {
    public static bool Compare(string input, string expected) {
        //Lexing the input and expected
        List<Lexeme> inputLexemes = new Lexer.Lexer(input).InternalLex();
        List<Lexeme> expectedLexemes = new Lexer.Lexer(expected).InternalLex();

        SyntaxItem parsedInput = new Parser.Parser(inputLexemes).Parse();
        SyntaxItem parsedExpected = new Parser.Parser(expectedLexemes).Parse();

        Simplifier.Simplifier simplifier = new(parsedInput);
        List<Tuple<SyntaxItem, string>> simplificationSteps = simplifier.Simplify();
        
        return simplificationSteps.Last().Item1.Equals(parsedExpected);
    }
}