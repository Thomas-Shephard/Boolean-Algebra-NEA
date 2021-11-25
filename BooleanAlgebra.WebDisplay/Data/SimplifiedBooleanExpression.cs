using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.WebDisplay.Data; 
public class SimplifiedBooleanExpression {
    public bool IsSuccess { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SyntaxItem? StartSyntaxTree { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SyntaxItem? EndSyntaxTree { get; }
    public Tuple<SyntaxItem, string>[] Simplifications { get; }

    LexerException? LexerException;

    public SimplifiedBooleanExpression(bool isSuccess, IEnumerable<Tuple<SyntaxItem, string>> simplifications, SyntaxItem? startSyntaxTree = null, SyntaxItem? endSyntaxTree = null, LexerException lexerException = null) {
        if (!isSuccess) {
            startSyntaxTree = null;
            endSyntaxTree = null;
        }
        
        IsSuccess = isSuccess;
        Simplifications = simplifications.ToArray();
        StartSyntaxTree = startSyntaxTree;
        EndSyntaxTree = endSyntaxTree;
        LexerException = lexerException;
    }

    public string FormatNumberOfSimplifications()
    {
        int simplificationsCount = Simplifications.Length - 1;
        return $"{simplificationsCount} simplification{(simplificationsCount == 1 ? "" : "s")}";
    }

    private string GetExceptionMessage()
    {
        if (IsSuccess)
            return "";
        return LexerException is not null ? LexerException.Message : "";

    }
    
    public static SimplifiedBooleanExpression LexingError(LexerException lexerException) {
        return new SimplifiedBooleanExpression(false, Array.Empty<Tuple<SyntaxItem, string>>(), lexerException: lexerException);
    }
    
    public static SimplifiedBooleanExpression ParsingError(ParserException parserException) {
        return new SimplifiedBooleanExpression(false, Array.Empty<Tuple<SyntaxItem, string>>());
    }
    
    public static SimplifiedBooleanExpression SimplificationError() {
        return new SimplifiedBooleanExpression(false, Array.Empty<Tuple<SyntaxItem, string>>());
    }
}