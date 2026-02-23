using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani landline phone numbers.
/// Formats: 021-XXXXXXXX (Karachi/Lahore - 3 digit code, 8 digit subscriber),
///          051-XXXXXXX (Islamabad - 3 digit code, 7 digit subscriber)
/// Accepts: 0XX-XXXXXXX, +92-XX-XXXXXXX, 92XXXXXXXXX
/// </summary>
public static partial class LandlineValidator
{
    // Area code -> (City name, expected subscriber digit count)
    private static readonly Dictionary<string, (string City, int SubscriberDigits)> AreaCodes = new()
    {
        // 3-digit area codes with 8-digit subscriber (major metros)
        ["021"] = ("Karachi", 8),
        ["042"] = ("Lahore", 8),

        // 3-digit area codes with 7-digit subscriber
        ["041"] = ("Faisalabad", 7),
        ["051"] = ("Islamabad / Rawalpindi", 7),
        ["052"] = ("Sialkot", 7),
        ["053"] = ("Gujrat", 7),
        ["055"] = ("Gujranwala", 7),
        ["061"] = ("Multan", 7),
        ["062"] = ("Bahawalpur", 7),
        ["068"] = ("Rahim Yar Khan", 7),
        ["071"] = ("Sukkur", 7),
        ["022"] = ("Hyderabad", 7),
        ["081"] = ("Quetta", 7),
        ["091"] = ("Peshawar", 7),
        ["092"] = ("Mardan", 7),
        ["044"] = ("Okara", 7),
        ["046"] = ("Sargodha", 7),
        ["047"] = ("Mianwali", 7),
        ["048"] = ("Jhang", 7),
        ["056"] = ("Jhelum", 7),
        ["057"] = ("Attock", 7),
        ["063"] = ("Dera Ghazi Khan", 7),
        ["064"] = ("Vehari", 7),
        ["066"] = ("Sahiwal", 7),
        ["074"] = ("Larkana", 7),

        // 4-digit area codes with 7-digit subscriber
        ["0992"] = ("Abbottabad", 7),
        ["0943"] = ("Muzaffarabad", 7),
        ["0946"] = ("Swat", 7),
        ["0995"] = ("Haripur", 7),
        ["0997"] = ("Mansehra", 7),
    };

    /// <summary>
    /// Validates a Pakistani landline number.
    /// Accepts: 021-12345678, 051-1234567, +92-21-12345678, +925112345670, 05112345670
    /// </summary>
    public static ValidationResult Validate(string? landline)
    {
        if (string.IsNullOrWhiteSpace(landline))
            return ValidationResult.Failure("Landline number is required.");

        // Step 1: Strip all formatting characters
        var stripped = landline.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        // Step 2: Must be all digits after stripping (except leading +)
        var digitsPart = stripped.StartsWith("+") ? stripped[1..] : stripped;
        if (!digitsPart.All(char.IsDigit))
            return ValidationResult.Failure("Landline number must contain only digits, spaces, dashes, and optional +92 prefix.");

        // Step 3: Normalize to local format starting with 0
        string local;
        if (stripped.StartsWith("+92"))
            local = "0" + stripped[3..];
        else if (stripped.StartsWith("92") && stripped.Length >= 11 && !stripped.StartsWith("0"))
            local = "0" + stripped[2..];
        else if (stripped.StartsWith("0"))
            local = stripped;
        else
            return ValidationResult.Failure("Landline must start with 0, +92, or 92.");

        // Step 4: Must be all digits now
        if (!local.All(char.IsDigit))
            return ValidationResult.Failure("Invalid characters in landline number.");

        // Step 5: Total length check (10 for 3-digit code + 7 subscriber, 11 for 3-digit code + 8 subscriber or 4-digit code + 7)
        if (local.Length < 10 || local.Length > 12)
            return ValidationResult.Failure($"Landline number has invalid length ({local.Length} digits). Expected 10-12 digits including area code.");

        // Step 6: Try to match known area codes (longest first for 4-digit codes)
        foreach (var codeLength in new[] { 4, 3 })
        {
            if (local.Length <= codeLength) continue;

            var areaCode = local[..codeLength];
            if (AreaCodes.TryGetValue(areaCode, out var info))
            {
                var subscriber = local[codeLength..];

                if (subscriber.Length != info.SubscriberDigits)
                    return ValidationResult.Failure(
                        $"Landline for {info.City} ({areaCode}) requires {info.SubscriberDigits} subscriber digits, got {subscriber.Length}.");

                var metadata = new Dictionary<string, string>
                {
                    ["AreaCode"] = areaCode,
                    ["City"] = info.City,
                    ["SubscriberNumber"] = subscriber,
                    ["LocalFormat"] = $"{areaCode}-{subscriber}",
                    ["InternationalFormat"] = $"+92{local[1..]}",
                };

                return ValidationResult.Success(local, metadata);
            }
        }

        // Step 7: Unknown area code — still accept if reasonable length
        // Pakistani landlines are 10-11 digits total with 0 prefix
        if (local.Length >= 10 && local.Length <= 11)
        {
            var metadata = new Dictionary<string, string>
            {
                ["LocalFormat"] = local,
                ["InternationalFormat"] = $"+92{local[1..]}",
                ["AreaCode"] = "Unknown",
            };

            return ValidationResult.Success(local, metadata);
        }

        return ValidationResult.Failure("Invalid Pakistani landline number format.");
    }

    /// <summary>
    /// Quick check — returns true if landline number is valid.
    /// </summary>
    public static bool IsValid(string? landline) => Validate(landline).IsValid;

    /// <summary>
    /// Gets the city for a landline number.
    /// </summary>
    public static string? GetCity(string? landline)
    {
        var result = Validate(landline);
        return result.IsValid && result.Metadata.TryGetValue("City", out var city) ? city : null;
    }
}
