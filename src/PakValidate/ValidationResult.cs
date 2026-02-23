namespace PakValidate;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>
    /// Whether the validation passed.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Error message if validation failed; null if valid.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Sanitized/formatted version of the input (e.g., CNIC without dashes).
    /// </summary>
    public string? Sanitized { get; }

    /// <summary>
    /// Additional metadata extracted during validation.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    private ValidationResult(bool isValid, string? errorMessage, string? sanitized, Dictionary<string, string>? metadata)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        Sanitized = sanitized;
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success(string? sanitized = null, Dictionary<string, string>? metadata = null)
        => new(true, null, sanitized, metadata);

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    public static ValidationResult Failure(string errorMessage)
        => new(false, errorMessage, null, null);

    /// <summary>
    /// Implicit conversion to bool for easy if-checks.
    /// </summary>
    public static implicit operator bool(ValidationResult result) => result.IsValid;

    /// <inheritdoc />
    public override string ToString()
        => IsValid ? $"Valid ({Sanitized})" : $"Invalid: {ErrorMessage}";
}
