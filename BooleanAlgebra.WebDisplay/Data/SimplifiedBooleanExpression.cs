using System.Diagnostics.CodeAnalysis;
using BooleanAlgebra.Simplifier;

namespace BooleanAlgebra.WebDisplay.Data; 
public class SimplifiedBooleanExpression {
    /// <summary>
    /// Whether the boolean expression was simplified successfully.
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// When the boolean expression was not simplified successfully, this contains the error message, else null.
    /// </summary>
    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public string? ErrorMessage { get; }
    /// <summary>
    /// When the boolean expression was simplified successfully, this contains all the simplification steps, else null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SimplificationRulePair[]? SimplificationSteps { get; }

    public SimplifiedBooleanExpression(IEnumerable<SimplificationRulePair> simplificationSteps) {
        SimplificationSteps = simplificationSteps.ToArray();
        IsSuccess = true;
    }

    public SimplifiedBooleanExpression(Exception userHandledException) {
        ErrorMessage = userHandledException.Message;
        IsSuccess = false;
    }

    /// <summary>
    /// Returns a formatted string representation of the number of simplification steps.
    /// </summary>
    /// <returns>A formatted string representation of the number of simplification steps.</returns>
    public string FormatNumberOfSimplifications() {
        //The Simplifications array contains the original expression at index 0.
        //Therefore, the number of simplifications is the length of the array - 1.
        int simplificationsCount = SimplificationSteps?.Length - 1 ?? 0;
        //If there is more than 1 simplification, the string is pluralized.
        return $"{simplificationsCount} simplification{(simplificationsCount == 1 ? "" : "s")}";
    }
}