﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    var timer = new Stopwatch();
    timer.Start();
    bool hasLexedSuccessfully = Lexer.Lex(rawText, out List<Lexeme> lexemes);
    timer.Stop();
    Console.WriteLine($"Time taken for lexing: {timer.Elapsed.Milliseconds}ms"); 
    if (!hasLexedSuccessfully) {
        Console.WriteLine("Unknown lexeme identified in input string");
        lexemes.ForEach(Console.WriteLine);
        continue;
    }
    if(isDebugModeEnabled)
        lexemes.ForEach(Console.WriteLine);
    timer = new Stopwatch();
    timer.Start();
    SyntaxItem? syntaxItem = Parser.Parse(lexemes, false); 
    timer.Stop();
    Console.WriteLine($"Time taken for parsing: {timer.Elapsed.Milliseconds}ms"); 
    if(isDebugModeEnabled)
        Console.WriteLine( syntaxItem?.ToString() ?? "");
    if (syntaxItem is not null) {
        timer = new Stopwatch();
        timer.Start();
        Simplification simplification = new(syntaxItem);
        timer.Stop();
        Console.WriteLine($"Time taken for simplification: {timer.Elapsed.Milliseconds}ms");
        Console.WriteLine(simplification);
    }
}