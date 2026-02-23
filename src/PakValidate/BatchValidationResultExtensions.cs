namespace PakValidate;

/// <summary>
/// Extension methods for convenient access to BatchValidationResult.
/// Provides helpers for error handling and iteration.
/// </summary>
public static class BatchValidationResultExtensions
{
    /// <summary>
    /// Gets the error message for a specific field, or null if the field is valid.
    /// </summary>
    /// <param name="batch">The batch validation result.</param>
    /// <param name="fieldName">The name of the field to check.</param>
    /// <returns>The error message if invalid, null if valid.</returns>
    public static string? GetError(this BatchValidationResult batch, string fieldName)
        => batch.Errors.TryGetValue(fieldName, out var error) ? error : null;

    /// <summary>
    /// Gets all errors as an enumerable of (field, error) tuples for convenient iteration.
    /// </summary>
    /// <param name="batch">The batch validation result.</param>
    /// <returns>An enumerable of (field name, error message) tuples.</returns>
    /// <example>
    /// <code>
    /// foreach (var (field, error) in batch.GetErrors())
    ///     Console.WriteLine($"{field}: {error}");
    /// </code>
    /// </example>
    public static IEnumerable<(string Field, string Error)> GetErrors(this BatchValidationResult batch)
        => batch.Errors.Select(kvp => (kvp.Key, kvp.Value));

    /// <summary>
    /// Throws a ValidationException if any validations failed.
    /// </summary>
    /// <param name="batch">The batch validation result.</param>
    /// <exception cref="ValidationException">Thrown when batch validation is invalid.</exception>
    /// <example>
    /// <code>
    /// batch.ThrowIfInvalid();  // Throws if any field failed validation
    /// </code>
    /// </example>
    public static void ThrowIfInvalid(this BatchValidationResult batch)
    {
        if (!batch.IsValid)
        {
            var errorSummary = string.Join(", ", batch.Errors.Keys);
            throw new ValidationException($"Validation failed for: {errorSummary}");
        }
    }

    /// <summary>
    /// Gets a value indicating whether any validations failed (opposite of IsValid).
    /// </summary>
    public static bool IsInvalid(this BatchValidationResult batch) => !batch.IsValid;
}
