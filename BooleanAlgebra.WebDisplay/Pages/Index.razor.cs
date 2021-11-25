using BooleanAlgebra.WebDisplay.Data;

namespace BooleanAlgebra.WebDisplay.Pages;
public partial class Index {
    private string _currentBooleanAlgebraExpression = string.Empty;
    private string CurrentBooleanAlgebraExpression {
        get => _currentBooleanAlgebraExpression;
        set {
            switch (value.Length) {
                case > 100:
                    errorMessage = "The boolean expression cannot exceed 100 characters";
                    _isUserInputError = true;
                    break;
                case 0:
                    errorMessage = "The boolean expression must not be empty";
                    _isUserInputError = true;
                    break;
                default:
                    errorMessage = string.Empty;
                    _isUserInputError = false;
                    break;
            }

            _currentBooleanAlgebraExpression = value;
        }
    }

    private bool _isUserInputError;
    
    private string errorMessage = string.Empty;
    private SimplifiedBooleanExpression? _simplifiedBooleanExpression;
    private bool _showFullExpansion;
    private bool _isSimplifying;
    private bool _isInErrorState;
    
    private async Task SimplifyBooleanExpression() {
        if (_isUserInputError)
            return;
        _isSimplifying = true;
        _simplifiedBooleanExpression = await BooleanExpressionSimplificationService.SimplifyBooleanExpressionAsync(CurrentBooleanAlgebraExpression);
        _isSimplifying = false;
    }

    private string FormatNumberOfSimplifications() {
        return _simplifiedBooleanExpression?.FormatNumberOfSimplifications() ?? throw new NullReferenceException(nameof(_simplifiedBooleanExpression));
    }

    private string GetErrorMessage() {
        return errorMessage;
    }
}