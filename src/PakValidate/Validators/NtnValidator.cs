using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani NTN (National Tax Number) issued by FBR.
/// Format: 7 digits followed by a check digit (e.g., 1234567-8 or 12345678)
/// NTN can also be a CNIC-based NTN (13 digits matching CNIC format).
/// </summary>
public static partial class NtnValidator
{
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^\d{7}-?\d$")]
    private static partial Regex NtnPattern();
#else
    private static readonly Regex _ntnPattern = new(@"^\d{7}-?\d$", RegexOptions.Compiled);
    private static Regex NtnPattern() => _ntnPattern;
#endif

    /// <summary>
    /// Validates a Pakistani NTN (National Tax Number).
    /// Accepts both 7+1 digit NTN and 13-digit CNIC-based NTN.
    /// </summary>
    public static ValidationResult Validate(string? ntn)
    {
        if (string.IsNullOrWhiteSpace(ntn))
            return ValidationResult.Failure("NTN is required.");

        var trimmed = ntn.Trim();
        var digits = trimmed.Replace("-", "");

        // Must be all digits after removing dashes
        if (!digits.All(char.IsDigit))
            return ValidationResult.Failure("NTN must contain only digits and optional dashes.");

        // CNIC-based NTN (13 digits)
        if (digits.Length == 13)
        {
            var cnicResult = CnicValidator.Validate(digits);
            if (!cnicResult.IsValid)
                return ValidationResult.Failure("CNIC-based NTN has invalid CNIC format.");

            var metadata = new Dictionary<string, string>
            {
                ["Type"] = "CNIC-based",
                ["Formatted"] = $"{digits[..5]}-{digits[5..12]}-{digits[12]}",
            };

            return ValidationResult.Success(digits, metadata);
        }

        // Standard NTN: must match XXXXXXX-X or XXXXXXXX (exactly 8 digits)
        if (!NtnPattern().IsMatch(trimmed))
            return ValidationResult.Failure("NTN must be 7+1 digits (e.g., 1234567-8) or a 13-digit CNIC-based NTN.");

        if (digits.Length != 8)
            return ValidationResult.Failure("Standard NTN must contain exactly 8 digits.");

        if (digits.Distinct().Count() == 1)
            return ValidationResult.Failure("NTN cannot contain all identical digits.");

        var ntnMetadata = new Dictionary<string, string>
        {
            ["Type"] = "Standard",
            ["Formatted"] = $"{digits[..7]}-{digits[7]}",
        };

        return ValidationResult.Success(digits, ntnMetadata);
    }

    /// <summary>
    /// Quick check — returns true if NTN is valid.
    /// </summary>
    public static bool IsValid(string? ntn) => Validate(ntn).IsValid;

    /// <summary>
    /// Formats an NTN to standard format.
    /// </summary>
    public static string? Format(string? ntn)
    {
        var result = Validate(ntn);
        return result.IsValid ? result.Metadata["Formatted"] : null;
    }
}
