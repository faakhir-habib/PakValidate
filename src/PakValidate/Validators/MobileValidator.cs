using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani mobile phone numbers.
/// Supports formats: 03001234567, 0300-1234567, +923001234567, 923001234567
/// Detects mobile network operator from prefix.
/// </summary>
public static partial class MobileValidator
{
    private static readonly Dictionary<string, string> CarrierPrefixes = new()
    {
        // Jazz (Mobilink + Warid)
        ["0300"] = "Jazz", ["0301"] = "Jazz", ["0302"] = "Jazz",
        ["0303"] = "Jazz", ["0304"] = "Jazz", ["0305"] = "Jazz",
        ["0306"] = "Jazz", ["0307"] = "Jazz", ["0308"] = "Jazz", ["0309"] = "Jazz",
        ["0321"] = "Jazz", ["0322"] = "Jazz", ["0323"] = "Jazz",
        ["0324"] = "Jazz", ["0325"] = "Jazz",

        // Telenor
        ["0340"] = "Telenor", ["0341"] = "Telenor", ["0342"] = "Telenor",
        ["0343"] = "Telenor", ["0344"] = "Telenor", ["0345"] = "Telenor",
        ["0346"] = "Telenor", ["0347"] = "Telenor", ["0348"] = "Telenor", ["0349"] = "Telenor",

        // Zong
        ["0310"] = "Zong", ["0311"] = "Zong", ["0312"] = "Zong",
        ["0313"] = "Zong", ["0314"] = "Zong", ["0315"] = "Zong",
        ["0316"] = "Zong", ["0317"] = "Zong", ["0318"] = "Zong", ["0319"] = "Zong",

        // Ufone
        ["0330"] = "Ufone", ["0331"] = "Ufone", ["0332"] = "Ufone",
        ["0333"] = "Ufone", ["0334"] = "Ufone", ["0335"] = "Ufone",
        ["0336"] = "Ufone", ["0337"] = "Ufone", ["0338"] = "Ufone", ["0339"] = "Ufone",

        // SCO (Special Communications Organization - AJK/GB)
        ["0355"] = "SCO", ["0356"] = "SCO", ["0357"] = "SCO",
    };

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^(?:(?:00|\+)?92|0)?(3\d{9})$")]
    private static partial Regex MobilePattern();
#else
    private static readonly Regex _mobilePattern = new(@"^(?:(?:00|\+)?92|0)?(3\d{9})$", RegexOptions.Compiled);
    private static Regex MobilePattern() => _mobilePattern;
#endif

    /// <summary>
    /// Validates a Pakistani mobile number.
    /// </summary>
    public static ValidationResult Validate(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return ValidationResult.Failure("Mobile number is required.");

        var input = mobile.Trim().Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");

        // Reject non-ASCII characters early (e.g. Urdu digits ۰۱۲)
        if (input.Any(c => c > 127))
            return ValidationResult.Failure("Mobile number must contain only ASCII digits.");

        var match = MobilePattern().Match(input);
        if (!match.Success)
            return ValidationResult.Failure("Invalid Pakistani mobile number. Expected format: 03XXXXXXXXX, +923XXXXXXXXX.");

        var digits = match.Groups[1].Value; // 10 digits starting with 3
        var fullLocal = "0" + digits;       // 03XXXXXXXXX
        var prefix = fullLocal[..4];         // 03XX

        var metadata = new Dictionary<string, string>
        {
            ["LocalFormat"] = fullLocal,
            ["InternationalFormat"] = $"+92{digits}",
            ["E164"] = $"+92{digits}",
            ["Prefix"] = prefix,
        };

        if (CarrierPrefixes.TryGetValue(prefix, out var carrier))
        {
            metadata["Carrier"] = carrier;
        }
        else
        {
            metadata["Carrier"] = "Unknown";
        }

        return ValidationResult.Success(fullLocal, metadata);
    }

    /// <summary>
    /// Quick check — returns true if mobile number is valid.
    /// </summary>
    public static bool IsValid(string? mobile) => Validate(mobile).IsValid;

    /// <summary>
    /// Gets the carrier name for a mobile number.
    /// </summary>
    public static string? GetCarrier(string? mobile)
    {
        var result = Validate(mobile);
        return result.IsValid && result.Metadata.ContainsKey("Carrier") ? result.Metadata["Carrier"] : null;
    }

    /// <summary>
    /// Formats to international E.164 format.
    /// </summary>
    public static string? ToInternational(string? mobile)
    {
        var result = Validate(mobile);
        return result.IsValid ? result.Metadata["E164"] : null;
    }
}
