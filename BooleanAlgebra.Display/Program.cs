using System;
using System.Collections.Generic;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;
using BooleanAlgebra.Simplification;

bool isDebugModeEnabled = false;
while (true) {
    Console.Write("Enter text > ");
    string rawText = Console.ReadLine() ?? "";
    if (rawText == "#Debug") {
        isDebugModeEnabled = !isDebugModeEnabled;
        Console.WriteLine($"Debug mode has been {(isDebugModeEnabled ? "enabled" : "disabled")}");
        continue;
    }
    bool hasLexedSuccessfully = Lexer.Lex(rawText, out List<Lexeme> lexemes);
    if (!hasLexedSuccessfully) {
        Console.WriteLine("Unknown lexeme identified in input string");
        lexemes.ForEach(Console.WriteLine);
        continue;
    }
    if(isDebugModeEnabled)
        lexemes.ForEach(Console.WriteLine);
    SyntaxItem? syntaxItem = Parser.Parse(lexemes); 
    Console.WriteLine( syntaxItem?.ToString() ?? "");
    if(syntaxItem is not null)
        Console.WriteLine(new Simplification(syntaxItem).ToString());
}