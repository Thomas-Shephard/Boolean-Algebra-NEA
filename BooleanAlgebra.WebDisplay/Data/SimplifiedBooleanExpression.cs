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
        //The Simplifications array contains the original expression at index 0.
        //Therefore, the number of simplifications is the length of the array - 1.
        int simplificationsCount = Simplifications?.Length - 1 ?? 0;
        //Provides a formatted number of simplifications.
        //If there is more than 1 simplification, the string is pluralized.
        return $"{simplificationsCount} simplification{(simplificationsCount == 1 ? "" : "s")}";
    }
}