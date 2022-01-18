using System.Diagnostics.CodeAnalysis;
using BooleanAlgebra.Parser.Syntax;

namespace BooleanAlgebra.WebDisplay.Data; 
public class SimplifiedBooleanExpression {
    public bool IsSuccess { get; }
    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public string? ErrorMessage { get; }
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public Tuple<ISyntaxItem, string>[]? Simplifications { get; }

    public SimplifiedBooleanExpression(IEnumerable<Tuple<ISyntaxItem, string>> simplifications) {
        Simplifications = simplifications.ToArray();
        IsSuccess = true;
    }

    public SimplifiedBooleanExpression(Exception userHandledException) {
        ErrorMessage = userHandledException.Message;
        IsSuccess = false;
    }

    public string FormatNumberOfSimplifications() {
        int simplificationsCount = Simplifications?.Length - 1 ?? 0;
        return $"{simplificationsCount} simplification{(simplificationsCount == 1 ? "" : "s")}";
    }
}