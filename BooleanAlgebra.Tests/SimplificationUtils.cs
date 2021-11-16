using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.Tests; 

public static class SimplificationUtils {
    public static bool Compare(string input, string expected) {
        //Lexing the input and expected
        if (!new Lexer.Lexer(input).Lex(out List<Lexeme> inputLexemes))
            throw new ArgumentException(nameof(input));
        if (!new Lexer.Lexer(expected).Lex(out List<Lexeme> expectedLexemes))
            throw new ArgumentException(nameof(expected));
        //Parsing the left and right hand sides
        if (!new Parser.Parser(inputLexemes, true).TryParse(out SyntaxItem? inputSyntaxTree))
            throw new ArgumentException(nameof(inputLexemes));
        if (!new Parser.Parser(expectedLexemes, true).TryParse(out SyntaxItem? expectedSyntaxTree))
            throw new ArgumentException(nameof(expectedLexemes));
        
        Simplification.Simplification simplification = new(inputSyntaxTree);
        return simplification.Simplifications.Last().Item1.Equals(expectedSyntaxTree);
    }
}