using System;
using System.Collections.Generic;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;

bool isDebugModeEnabled = false;
while (true) {
    Console.Write("Enter text > ");
    string rawText = Console.ReadLine() ?? "";
    if (rawText == "#Debug") {
        isDebugModeEnabled = !isDebugModeEnabled;
        Console.WriteLine($"Debug mode has been {(isDebugModeEnabled ? "enabled" : "disabled")}");
        continue;
    }
    bool hasLexedSuccessfully = Lexer.Lex(rawText, out List<ILexeme> lexemes);
    if (!hasLexedSuccessfully) {
        Console.WriteLine("Unknown lexeme identified in input string");
        lexemes.ForEach(Console.WriteLine);
        continue;
    }
    if(isDebugModeEnabled)
        lexemes.ForEach(Console.WriteLine);
    Console.WriteLine(Parser.Parse(lexemes)?.ToString() ?? "");
}