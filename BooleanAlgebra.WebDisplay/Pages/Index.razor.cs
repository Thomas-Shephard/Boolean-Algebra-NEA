using BooleanAlgebra.WebDisplay.Data;

namespace BooleanAlgebra.WebDisplay.Pages;
public partial class Index {
    private SimplifiedBooleanExpression? _simplifiedBooleanExpression;
    private bool _isSimplifying;
    private bool _isInErrorState;
    private string _errorMessage = string.Empty;
    private string _currentBooleanAlgebraExpression = string.Empty;
    private string CurrentBooleanAlgebraExpression {
        get => _currentBooleanAlgebraExpression;
        set {
            switch (value.Length) {
                case > 100:
                    _errorMessage = "The boolean expression must not exceed 100 characters in length";
                    _isInErrorState = true;
                    break;
                case 0:
                    _errorMessage = "The boolean expression must not be empty";
                    _isInErrorState = true;
                    break;
                default:
                    _errorMessage = string.Empty;
                    _isInErrorState = false;
                    break;
            }

            _currentBooleanAlgebraExpression = value;
        }
    }

    private async Task SimplifyBooleanExpression() {
        if (_isInErrorState)
            return;

        _isSimplifying = true;
        _simplifiedBooleanExpression = await BooleanExpressionSimplificationService.SimplifyBooleanExpressionAsync(CurrentBooleanAlgebraExpression);

        if (!_simplifiedBooleanExpression.IsSuccess) {
            _errorMessage = _simplifiedBooleanExpression.ErrorMessage;
            _isInErrorState = true;
        }
        
        _isSimplifying = false;
    }

    private string FormatNumberOfSimplifications() {
        return _simplifiedBooleanExpression?.FormatNumberOfSimplifications() ?? throw new NullReferenceException(nameof(_simplifiedBooleanExpression));
    }
}