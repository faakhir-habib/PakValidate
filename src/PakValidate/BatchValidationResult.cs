namespace PakValidate;

/// <summary>
/// Represents the result of batch validation for multiple fields.
/// </summary>
public sealed class BatchValidationResult
{
    private readonly IReadOnlyDictionary<string, ValidationResult> _results;
    private readonly IReadOnlyDictionary<string, string> _errors;

    /// <summary>
    /// Gets a value indicating whether all validations passed.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets all validation results, keyed by field name.
    /// </summary>
    public IReadOnlyDictionary<string, ValidationResult> Results => _results;

    /// <summary>
    /// Gets only the failed validations, keyed by field name with error message as value.
    /// </summary>
    public IReadOnlyDictionary<string, string> Errors => _errors;

    /// <summary>
    /// Creates a new batch validation result.
    /// </summary>
    /// <param name="validations">Enumerable of (field name, validation result) tuples.</param>
    public BatchValidationResult(IEnumerable<(string Field, ValidationResult Result)> validations)
    {
        var resultsList = validations.ToList();
        _results = resultsList.ToDictionary(x => x.Field, x => x.Result);
        _errors = resultsList
            .Where(x => !x.Result.IsValid)
            .ToDictionary(x => x.Field, x => x.Result.ErrorMessage ?? "Validation failed");
        IsValid = _errors.Count == 0;
    }

    /// <summary>
    /// Implicit conversion to bool for easy validation checks.
    /// </summary>
    public static implicit operator bool(BatchValidationResult result) => result.IsValid;
}
