using System;
using System.Collections.Generic;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;

while (true) {
    Console.Write("Enter text > ");
    string rawText = Console.ReadLine() ?? "";
    bool hasLexedSuccessfully = Lexer.Lex(rawText, out List<ILexeme> lexemes);
    if (!hasLexedSuccessfully) {
        Console.WriteLine("Unknown lexeme identified in input string");
        continue;
    }
    lexemes.ForEach(Console.WriteLine);
    Console.WriteLine(Parser.Parse(lexemes)?.ToString() ?? "");
}