using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani postal codes.
/// Format: 5 digits (e.g., 44000 for Islamabad, 75500 for Karachi).
/// First 2 digits represent the postal region/district.
/// </summary>
public static partial class PostalCodeValidator
{
    // Major city/region postal code ranges
    private static readonly Dictionary<string, string> RegionPrefixes = new()
    {
        ["10"] = "Peshawar",
        ["12"] = "Peshawar Cantonment",
        ["17"] = "Haripur / Hazara",
        ["18"] = "Abbottabad",
        ["19"] = "Mansehra",
        ["20"] = "Rawalpindi",
        ["21"] = "Rawalpindi",
        ["22"] = "Attock",
        ["25"] = "Murree / Galiyat",
        ["30"] = "Lahore",
        ["31"] = "Lahore",
        ["33"] = "Gujranwala",
        ["34"] = "Sialkot",
        ["35"] = "Faisalabad",
        ["38"] = "Multan",
        ["40"] = "Sargodha",
        ["42"] = "Sahiwal",
        ["44"] = "Islamabad",
        ["45"] = "Islamabad",
        ["46"] = "Islamabad / Rawalpindi",
        ["47"] = "Taxila / Wah",
        ["50"] = "Bahawalpur",
        ["54"] = "Lahore (extended)",
        ["56"] = "Gujrat",
        ["60"] = "Dera Ghazi Khan",
        ["62"] = "Rahim Yar Khan",
        ["64"] = "Vehari",
        ["70"] = "Hyderabad",
        ["71"] = "Hyderabad",
        ["72"] = "Sukkur",
        ["74"] = "Karachi",
        ["75"] = "Karachi",
        ["76"] = "Thatta / Badin",
        ["77"] = "Larkana",
        ["80"] = "Quetta",
        ["82"] = "Zhob / Loralai",
        ["84"] = "Turbat / Gwadar",
        ["86"] = "Kalat / Khuzdar",
    };

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex PostalPattern();
#else
    private static readonly Regex _postalPattern = new(@"^\d{5}$", RegexOptions.Compiled);
    private static Regex PostalPattern() => _postalPattern;
#endif

    /// <summary>
    /// Validates a Pakistani postal code (5 digits).
    /// </summary>
    public static ValidationResult Validate(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return ValidationResult.Failure("Postal code is required.");

        var input = postalCode.Trim().Replace(" ", "");

        if (!PostalPattern().IsMatch(input))
            return ValidationResult.Failure("Postal code must be exactly 5 digits.");

        var prefix = input[..2];
        var code = int.Parse(input);

        // Pakistani postal codes range roughly from 10000 to 97000
        if (code < 10000 || code > 97000)
            return ValidationResult.Failure("Postal code is outside the valid Pakistani range (10000-97000).");

        var metadata = new Dictionary<string, string>
        {
            ["RegionPrefix"] = prefix,
        };

        if (RegionPrefixes.TryGetValue(prefix, out var region))
        {
            metadata["Region"] = region;
        }

        return ValidationResult.Success(input, metadata);
    }

    /// <summary>
    /// Quick check — returns true if postal code is valid.
    /// </summary>
    public static bool IsValid(string? postalCode) => Validate(postalCode).IsValid;

    /// <summary>
    /// Gets the region/city for a postal code prefix.
    /// </summary>
    public static string? GetRegion(string? postalCode)
    {
        var result = Validate(postalCode);
        return result.IsValid && result.Metadata.ContainsKey("Region") ? result.Metadata["Region"] : null;
    }
}
