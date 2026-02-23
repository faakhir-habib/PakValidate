using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani STRN (Sales Tax Registration Number) issued by FBR.
/// Format: 13 digits, typically structured as region code + registration number.
/// Used for GST/Sales Tax compliance.
/// </summary>
public static partial class StrnValidator
{
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^\d{13}$")]
    private static partial Regex StrnPattern();
#else
    private static readonly Regex _strnPattern = new(@"^\d{13}$", RegexOptions.Compiled);
    private static Regex StrnPattern() => _strnPattern;
#endif

    // Known STRN regional prefixes (first 2 digits indicate RTO jurisdiction)
    private static readonly Dictionary<string, string> RtoJurisdictions = new()
    {
        ["01"] = "RTO Karachi (Zone-I)",
        ["02"] = "RTO Karachi (Zone-II)",
        ["03"] = "RTO Karachi (Zone-III)",
        ["04"] = "RTO Hyderabad",
        ["05"] = "RTO Sukkur",
        ["06"] = "RTO Lahore (Zone-I)",
        ["07"] = "RTO Lahore (Zone-II)",
        ["08"] = "RTO Faisalabad",
        ["09"] = "RTO Multan",
        ["10"] = "RTO Gujranwala",
        ["11"] = "RTO Sialkot",
        ["12"] = "RTO Rawalpindi",
        ["13"] = "RTO Islamabad",
        ["14"] = "RTO Peshawar",
        ["15"] = "RTO Quetta",
        ["16"] = "RTO Abbottabad",
        ["17"] = "RTO Sargodha",
        ["18"] = "RTO Bahawalpur",
    };

    /// <summary>
    /// Validates a Pakistani STRN (Sales Tax Registration Number).
    /// </summary>
    public static ValidationResult Validate(string? strn)
    {
        if (string.IsNullOrWhiteSpace(strn))
            return ValidationResult.Failure("STRN is required.");

        var input = strn.Trim().Replace("-", "").Replace(" ", "");

        if (!StrnPattern().IsMatch(input))
            return ValidationResult.Failure("STRN must be exactly 13 digits.");

        if (input.Distinct().Count() == 1)
            return ValidationResult.Failure("STRN cannot contain all identical digits.");

        if (input.All(c => c == '0'))
            return ValidationResult.Failure("STRN cannot be all zeros.");

        var prefix = input[..2];

        var metadata = new Dictionary<string, string>
        {
            ["Formatted"] = $"{input[..2]}-{input[2..6]}-{input[6..10]}-{input[10..13]}",
            ["RegionCode"] = prefix,
        };

        if (RtoJurisdictions.TryGetValue(prefix, out var jurisdiction))
        {
            metadata["Jurisdiction"] = jurisdiction;
        }

        return ValidationResult.Success(input, metadata);
    }

    /// <summary>
    /// Quick check — returns true if STRN is valid.
    /// </summary>
    public static bool IsValid(string? strn) => Validate(strn).IsValid;

    /// <summary>
    /// Formats a STRN to readable format.
    /// </summary>
    public static string? Format(string? strn)
    {
        var result = Validate(strn);
        return result.IsValid ? result.Metadata["Formatted"] : null;
    }
}
