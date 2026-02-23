using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani CNIC (Computerized National Identity Card) numbers.
/// Format: 5 digits - 7 digits - 1 digit (e.g., 35202-1234567-1)
/// Total 13 digits. Last digit indicates gender (odd = male, even = female).
/// First 5 digits represent the locality code.
/// </summary>
public static partial class CnicValidator
{
    // Known province prefixes for the first digit of CNIC
    private static readonly Dictionary<char, string> ProvinceMap = new()
    {
        ['1'] = "Khyber Pakhtunkhwa",
        ['2'] = "FATA / Merged Areas",
        ['3'] = "Punjab",
        ['4'] = "Sindh",
        ['5'] = "Balochistan",
        ['6'] = "Islamabad",
        ['7'] = "Gilgit-Baltistan / AJK",
    };

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^\d{5}-?\d{7}-?\d{1}$")]
    private static partial Regex CnicPattern();
#else
    private static readonly Regex _cnicPattern = new(@"^\d{5}-?\d{7}-?\d{1}$", RegexOptions.Compiled);
    private static Regex CnicPattern() => _cnicPattern;
#endif

    /// <summary>
    /// Validates a Pakistani CNIC number.
    /// Accepts formats: 3520212345671, 35202-1234567-1
    /// </summary>
    /// <param name="cnic">The CNIC number to validate.</param>
    /// <returns>A ValidationResult with sanitized CNIC and metadata.</returns>
    public static ValidationResult Validate(string? cnic)
    {
        if (string.IsNullOrWhiteSpace(cnic))
            return ValidationResult.Failure("CNIC is required.");

        var input = cnic.Trim();

        // Reject non-ASCII characters early (e.g. Urdu digits ١٢٣)
        if (input.Any(c => c > 127))
            return ValidationResult.Failure("CNIC must contain only ASCII digits and optional dashes.");

        if (!CnicPattern().IsMatch(input))
            return ValidationResult.Failure("CNIC must be 13 digits in format XXXXX-XXXXXXX-X or XXXXXXXXXXXXX.");

        var digits = input.Replace("-", "");

        if (digits.Length != 13)
            return ValidationResult.Failure("CNIC must contain exactly 13 digits.");

        // Check for all same digits
        if (digits.Distinct().Count() == 1)
            return ValidationResult.Failure("CNIC cannot contain all identical digits.");

        var firstDigit = digits[0];
        var lastDigit = int.Parse(digits[12].ToString());

        var metadata = new Dictionary<string, string>
        {
            ["Gender"] = lastDigit % 2 == 0 ? "Female" : "Male",
            ["LocalityCode"] = digits[..5],
            ["Formatted"] = $"{digits[..5]}-{digits[5..12]}-{digits[12]}"
        };

        if (ProvinceMap.TryGetValue(firstDigit, out var province))
        {
            metadata["Province"] = province;
        }

        return ValidationResult.Success(digits, metadata);
    }

    /// <summary>
    /// Quick check — returns true if CNIC is valid.
    /// </summary>
    public static bool IsValid(string? cnic) => Validate(cnic).IsValid;

    /// <summary>
    /// Formats a CNIC number to standard format: XXXXX-XXXXXXX-X
    /// </summary>
    public static string? Format(string? cnic)
    {
        var result = Validate(cnic);
        return result.IsValid ? result.Metadata["Formatted"] : null;
    }
}
