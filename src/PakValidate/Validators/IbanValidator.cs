using System.Numerics;
using System.Text.RegularExpressions;

namespace PakValidate.Validators;

/// <summary>
/// Validates Pakistani IBAN (International Bank Account Number).
/// Format: PK + 2 check digits + 4 letter bank code + 16 digit account number = 24 characters.
/// Performs MOD-97 check digit verification per ISO 13616.
/// </summary>
public static partial class IbanValidator
{
    private static readonly Dictionary<string, string> BankCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ABPA"] = "Allied Bank Limited",
        ["ALFH"] = "Alfalah Bank Limited",
        ["AIIN"] = "Al Baraka Bank Pakistan",
        ["ASCM"] = "Askari Bank Limited",
        ["BAHL"] = "Bank Al Habib Limited",
        ["BKIP"] = "Bank Islami Pakistan",
        ["BPUN"] = "Bank of Punjab",
        ["DUIB"] = "Dubai Islamic Bank",
        ["FAYS"] = "Faysal Bank Limited",
        ["HABB"] = "Habib Bank Limited",
        ["HBZM"] = "Habib Metropolitan Bank",
        ["JSBL"] = "JS Bank Limited",
        ["KLBL"] = "Khushhali Microfinance Bank",
        ["MCBL"] = "Muslim Commercial Bank",
        ["MEZN"] = "Meezan Bank Limited",
        ["MPBL"] = "Mobilink Microfinance Bank",
        ["MUCB"] = "MCB Bank Limited",
        ["NBPA"] = "National Bank of Pakistan",
        ["PRSA"] = "The Bank of Khyber",
        ["SCBL"] = "Standard Chartered Pakistan",
        ["SNDB"] = "Sindh Bank Limited",
        ["SBPL"] = "State Bank of Pakistan",
        ["SONE"] = "Soneri Bank Limited",
        ["SMBL"] = "Summit Bank Limited",
        ["UNIL"] = "United Bank Limited",
        ["ZCBL"] = "Zarai Taraqiati Bank Limited",
    };

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^PK\d{2}[A-Z]{4}\d{16}$", RegexOptions.IgnoreCase)]
    private static partial Regex IbanPattern();
#else
    private static readonly Regex _ibanPattern = new(@"^PK\d{2}[A-Z]{4}\d{16}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static Regex IbanPattern() => _ibanPattern;
#endif

    /// <summary>
    /// Validates a Pakistani IBAN.
    /// Performs format check, MOD-97 verification, and bank identification.
    /// </summary>
    public static ValidationResult Validate(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return ValidationResult.Failure("IBAN is required.");

        var input = iban.Trim().Replace(" ", "").Replace("-", "").ToUpperInvariant();

        if (!input.StartsWith("PK"))
            return ValidationResult.Failure("Pakistani IBAN must start with 'PK'.");

        if (input.Length != 24)
            return ValidationResult.Failure("Pakistani IBAN must be exactly 24 characters.");

        if (!IbanPattern().IsMatch(input))
            return ValidationResult.Failure("Invalid IBAN format. Expected: PK## XXXX ################.");

        // MOD-97 check (ISO 13616)
        if (!PassesMod97Check(input))
            return ValidationResult.Failure("IBAN check digits are invalid (MOD-97 failed).");

        var bankCode = input.Substring(4, 4);
        var accountNumber = input[8..];

        var metadata = new Dictionary<string, string>
        {
            ["BankCode"] = bankCode,
            ["AccountNumber"] = accountNumber,
            ["CheckDigits"] = input.Substring(2, 2),
            ["Formatted"] = $"{input[..4]} {input[4..8]} {input[8..12]} {input[12..16]} {input[16..20]} {input[20..24]}",
        };

        if (BankCodes.TryGetValue(bankCode, out var bankName))
        {
            metadata["BankName"] = bankName;
        }

        return ValidationResult.Success(input, metadata);
    }

    /// <summary>
    /// Performs MOD-97 check per ISO 13616.
    /// Move first 4 chars to end, convert letters to numbers (A=10, B=11, ...), check mod 97 == 1.
    /// </summary>
    private static bool PassesMod97Check(string iban)
    {
        // Rearrange: move first 4 characters to end
        var rearranged = iban[4..] + iban[..4];

        // Convert letters to numbers
        var numericString = new System.Text.StringBuilder();
        foreach (var c in rearranged)
        {
            if (char.IsLetter(c))
                numericString.Append((c - 'A' + 10).ToString());
            else
                numericString.Append(c);
        }

        // MOD 97 using BigInteger
        var number = BigInteger.Parse(numericString.ToString());
        return number % 97 == 1;
    }

    /// <summary>
    /// Quick check — returns true if IBAN is valid.
    /// </summary>
    public static bool IsValid(string? iban) => Validate(iban).IsValid;

    /// <summary>
    /// Gets the bank name for a given IBAN.
    /// </summary>
    public static string? GetBankName(string? iban)
    {
        var result = Validate(iban);
        return result.IsValid && result.Metadata.ContainsKey("BankName") ? result.Metadata["BankName"] : null;
    }
}
