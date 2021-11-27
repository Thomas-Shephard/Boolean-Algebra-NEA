using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using BooleanAlgebra.Lexer;
using BooleanAlgebra.Parser;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.WebDisplay.Data; 
public class SimplifiedBooleanExpression {
    public bool IsSuccess { get; }
    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public string? ErrorMessage { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SyntaxItem? StartSyntaxTree { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SyntaxItem? EndSyntaxTree { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public Tuple<SyntaxItem, string>[]? Simplifications { get; }

    public SimplifiedBooleanExpression(IEnumerable<Tuple<SyntaxItem, string>> simplifications, SyntaxItem startSyntaxTree, SyntaxItem endSyntaxTree) {
        Simplifications = simplifications.ToArray();
        StartSyntaxTree = startSyntaxTree;
        EndSyntaxTree = endSyntaxTree;
        IsSuccess = true;
    }

    public SimplifiedBooleanExpression(Exception unknownLexemeException) {
        ErrorMessage = unknownLexemeException.Message;
        IsSuccess = false;
    }

    public string FormatNumberOfSimplifications()
    {
        int simplificationsCount = Simplifications?.Length - 1 ?? 0;
        return $"{simplificationsCount} simplification{(simplificationsCount == 1 ? "" : "s")}";
    }
}