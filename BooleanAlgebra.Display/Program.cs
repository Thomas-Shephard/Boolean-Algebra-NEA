using System;
using System.Collections.Generic;
using System.Linq;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;

while (true) {
    Console.Write("Enter text > ");
    string rawText = Console.ReadLine() ?? "";
    List<ILexeme> lexemes = Lexer.Lex(rawText).ToList();
    lexemes.ForEach(Console.WriteLine);
    Console.WriteLine(Parser.Parse(lexemes)?.ToString() ?? "");
}