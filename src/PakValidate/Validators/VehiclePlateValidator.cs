using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani vehicle registration plate numbers.
/// Common formats:
///   - New format: XXX-1234 (3 letters, 3-4 digits)
///   - Old format: XX-1234 (2 letters, 3-4 digits)
///   - Islamabad: ISB-123, ISB-1234
///   - Government: G-1234, GS-1234, DN-1234
///   - Single letter: L-1234 (legacy Lahore)
/// Minimum 3 digits required for non-government plates, 2 for government.
/// </summary>
public static partial class VehiclePlateValidator
{
    private static readonly Dictionary<string, string> RegistrationPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Islamabad
        ["ISB"] = "Islamabad",

        // Punjab
        ["L"] = "Lahore", ["LE"] = "Lahore", ["LEA"] = "Lahore",
        ["LEB"] = "Lahore", ["LEC"] = "Lahore", ["LED"] = "Lahore",
        ["LEE"] = "Lahore", ["LEF"] = "Lahore",
        ["R"] = "Rawalpindi", ["RI"] = "Rawalpindi", ["RIA"] = "Rawalpindi",
        ["RIB"] = "Rawalpindi", ["RIC"] = "Rawalpindi",
        ["F"] = "Faisalabad", ["FSD"] = "Faisalabad",
        ["MN"] = "Multan", ["MUL"] = "Multan",
        ["GJ"] = "Gujranwala", ["GRW"] = "Gujranwala",
        ["SK"] = "Sialkot", ["SGD"] = "Sargodha",
        ["BWP"] = "Bahawalpur", ["JH"] = "Jhang",
        ["RYK"] = "Rahim Yar Khan", ["SWL"] = "Sahiwal",
        ["DGK"] = "Dera Ghazi Khan", ["JHL"] = "Jhelum",
        ["ATK"] = "Attock", ["MWI"] = "Mianwali",

        // Sindh
        ["K"] = "Karachi", ["KA"] = "Karachi",
        ["AJK"] = "Karachi (new)", ["AKA"] = "Karachi (new)",
        ["HYD"] = "Hyderabad", ["SKR"] = "Sukkur",
        ["LRK"] = "Larkana", ["NWS"] = "Nawabshah",

        // KPK
        ["P"] = "Peshawar", ["PES"] = "Peshawar",
        ["A"] = "Peshawar (old)", ["ABT"] = "Abbottabad",
        ["MRD"] = "Mardan", ["SWT"] = "Swat",

        // Balochistan
        ["Q"] = "Quetta", ["QTA"] = "Quetta",

        // AJK & GB
        ["AJ"] = "Azad Jammu & Kashmir",
        ["GB"] = "Gilgit-Baltistan",

        // Government & Diplomatic
        ["G"] = "Government (Federal)",
        ["GS"] = "Government (Senate)",
        ["GN"] = "Government (National Assembly)",
        ["DN"] = "Diplomatic",
        ["UN"] = "United Nations",
    };

    private static readonly HashSet<string> GovernmentPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "G", "GS", "GN", "DN", "UN"
    };

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^([A-Z]{1,4})\s*[-\s]?\s*(\d{2,5})$", RegexOptions.IgnoreCase)]
    private static partial Regex PlatePattern();
#else
    private static readonly Regex _platePattern = new(@"^([A-Z]{1,4})\s*[-\s]?\s*(\d{2,5})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static Regex PlatePattern() => _platePattern;
#endif

    /// <summary>
    /// Validates a Pakistani vehicle registration plate number.
    /// </summary>
    public static ValidationResult Validate(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return ValidationResult.Failure("Vehicle plate number is required.");

        var input = plate.Trim();

        var match = PlatePattern().Match(input);
        if (!match.Success)
            return ValidationResult.Failure("Invalid plate format. Expected: ABC-1234, AB-1234, or similar Pakistani plate format.");

        var letters = match.Groups[1].Value.ToUpperInvariant();
        var digits = match.Groups[2].Value;

        // Enforce minimum digit count based on plate type
        var isGovernment = GovernmentPrefixes.Contains(letters);
        var minDigits = isGovernment ? 2 : 3;

        if (digits.Length < minDigits)
            return ValidationResult.Failure(
                $"Plate number requires at least {minDigits} digits, got {digits.Length}.");

        if (digits.Length > 5)
            return ValidationResult.Failure("Plate number cannot exceed 5 digits.");

        var formatted = $"{letters}-{digits}";
        var metadata = new Dictionary<string, string>
        {
            ["Prefix"] = letters,
            ["Number"] = digits,
            ["Formatted"] = formatted,
        };

        if (isGovernment)
            metadata["Type"] = "Government/Diplomatic";

        // Try to identify registration city (longest match first)
        string? city = null;
        if (RegistrationPrefixes.TryGetValue(letters, out city)) { }
        else if (letters.Length >= 3 && RegistrationPrefixes.TryGetValue(letters[..3], out city)) { }
        else if (letters.Length >= 2 && RegistrationPrefixes.TryGetValue(letters[..2], out city)) { }
        else if (RegistrationPrefixes.TryGetValue(letters[..1], out city)) { }

        if (city != null)
            metadata["RegistrationCity"] = city;

        return ValidationResult.Success(formatted, metadata);
    }

    /// <summary>
    /// Quick check — returns true if plate is valid.
    /// </summary>
    public static bool IsValid(string? plate) => Validate(plate).IsValid;

    /// <summary>
    /// Gets the registration city for a plate.
    /// </summary>
    public static string? GetCity(string? plate)
    {
        var result = Validate(plate);
        return result.IsValid && result.Metadata.TryGetValue("RegistrationCity", out var city) ? city : null;
    }
}
