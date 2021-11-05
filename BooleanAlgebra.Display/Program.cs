using BooleanAlgebra.Lexer;
using BooleanAlgebra.Lexer.Lexemes;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;
using BooleanAlgebra.Simplification;
using System.Diagnostics;

bool isDebugModeEnabled = false;
while (true) {
    Console.Write("Enter text > ");
    string rawText = Console.ReadLine() ?? "";
    if (rawText == "#Debug") {
        isDebugModeEnabled = !isDebugModeEnabled;
        Console.WriteLine($"Debug mode has been {(isDebugModeEnabled ? "enabled" : "disabled")}!");
        continue;
    }
    Stopwatch timer = new();
    timer.Start();
    bool hasLexedSuccessfully = new Lexer(rawText).Lex(out List<Lexeme> lexemes);
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
    bool hasParsedSuccessfully = new Parser(lexemes).TryParse(out SyntaxItem? syntaxTree);
    timer.Stop();
    Console.WriteLine($"Time taken for parsing: {timer.Elapsed.Milliseconds}ms");
    if (!hasParsedSuccessfully) {
        Console.WriteLine("Parsing completed unsuccessfully");
        continue;
    }
    if(isDebugModeEnabled)
        Console.WriteLine(syntaxTree.ToString());

    timer = new Stopwatch();
    timer.Start();
    Simplification simplification = new(syntaxTree);
    timer.Stop();
    Console.WriteLine($"Time taken for simplification: {timer.Elapsed.Milliseconds}ms");
    Console.WriteLine(simplification);
}