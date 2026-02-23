namespace PakValidate;

/// <summary>
/// Extension methods for convenient metadata access on ValidationResult.
/// Provides cleaner syntax than dictionary access with property-like accessors.
/// </summary>
public static class ValidationResultExtensions
{
    // Metadata key constants to avoid magic strings
    private const string GenderKey = "Gender";
    private const string ProvinceKey = "Province";
    private const string LocalityCodeKey = "LocalityCode";
    private const string FormattedKey = "Formatted";
    private const string CarrierKey = "Carrier";
    private const string LocalFormatKey = "LocalFormat";
    private const string InternationalFormatKey = "InternationalFormat";
    private const string E164Key = "E164";
    private const string PrefixKey = "Prefix";
    private const string TypeKey = "Type";
    private const string BankCodeKey = "BankCode";
    private const string BankNameKey = "BankName";
    private const string AccountNumberKey = "AccountNumber";
    private const string CheckDigitsKey = "CheckDigits";
    private const string RegionKey = "Region";
    private const string RegionPrefixKey = "RegionPrefix";
    private const string AreaCodeKey = "AreaCode";
    private const string CityKey = "City";
    private const string RegistrationCityKey = "RegistrationCity";
    private const string NumberKey = "Number";
    private const string JurisdictionKey = "Jurisdiction";
    private const string RegionCodeKey = "RegionCode";

    /// <summary>Gets the gender from CNIC validation result (Male/Female).</summary>
    public static string? Gender(this ValidationResult result)
        => result.Metadata?.TryGetValue(GenderKey, out var value) == true ? value : null;

    /// <summary>Gets the province from CNIC validation result.</summary>
    public static string? Province(this ValidationResult result)
        => result.Metadata?.TryGetValue(ProvinceKey, out var value) == true ? value : null;

    /// <summary>Gets the locality code from CNIC validation result.</summary>
    public static string? LocalityCode(this ValidationResult result)
        => result.Metadata?.TryGetValue(LocalityCodeKey, out var value) == true ? value : null;

    /// <summary>Gets the formatted version of validated data (CNIC, NTN, STRN, etc.).</summary>
    public static string? Formatted(this ValidationResult result)
        => result.Metadata?.TryGetValue(FormattedKey, out var value) == true ? value : null;

    /// <summary>Gets the carrier from mobile number validation result (Jazz, Telenor, Zong, Ufone, SCO).</summary>
    public static string? Carrier(this ValidationResult result)
        => result.Metadata?.TryGetValue(CarrierKey, out var value) == true ? value : null;

    /// <summary>Gets the local format of a mobile number.</summary>
    public static string? LocalFormat(this ValidationResult result)
        => result.Metadata?.TryGetValue(LocalFormatKey, out var value) == true ? value : null;

    /// <summary>Gets the international format of a mobile or landline number.</summary>
    public static string? InternationalFormat(this ValidationResult result)
        => result.Metadata?.TryGetValue(InternationalFormatKey, out var value) == true ? value : null;

    /// <summary>Gets the E.164 format of a mobile number.</summary>
    public static string? E164(this ValidationResult result)
        => result.Metadata?.TryGetValue(E164Key, out var value) == true ? value : null;

    /// <summary>Gets the prefix of a mobile number.</summary>
    public static string? Prefix(this ValidationResult result)
        => result.Metadata?.TryGetValue(PrefixKey, out var value) == true ? value : null;

    /// <summary>Gets the NTN type from validation result (Standard/CNIC-based).</summary>
    public static string? NtnType(this ValidationResult result)
        => result.Metadata?.TryGetValue(TypeKey, out var value) == true ? value : null;

    /// <summary>Gets the bank code from IBAN validation result.</summary>
    public static string? BankCode(this ValidationResult result)
        => result.Metadata?.TryGetValue(BankCodeKey, out var value) == true ? value : null;

    /// <summary>Gets the bank name from IBAN validation result.</summary>
    public static string? BankName(this ValidationResult result)
        => result.Metadata?.TryGetValue(BankNameKey, out var value) == true ? value : null;

    /// <summary>Gets the account number from IBAN validation result.</summary>
    public static string? AccountNumber(this ValidationResult result)
        => result.Metadata?.TryGetValue(AccountNumberKey, out var value) == true ? value : null;

    /// <summary>Gets the check digits from IBAN validation result.</summary>
    public static string? CheckDigits(this ValidationResult result)
        => result.Metadata?.TryGetValue(CheckDigitsKey, out var value) == true ? value : null;

    /// <summary>Gets the region from postal code validation result.</summary>
    public static string? Region(this ValidationResult result)
        => result.Metadata?.TryGetValue(RegionKey, out var value) == true ? value : null;

    /// <summary>Gets the region prefix from postal code validation result.</summary>
    public static string? RegionPrefix(this ValidationResult result)
        => result.Metadata?.TryGetValue(RegionPrefixKey, out var value) == true ? value : null;

    /// <summary>Gets the area code from landline validation result.</summary>
    public static string? AreaCode(this ValidationResult result)
        => result.Metadata?.TryGetValue(AreaCodeKey, out var value) == true ? value : null;

    /// <summary>Gets the city from landline or vehicle plate validation result.</summary>
    public static string? City(this ValidationResult result)
        => result.Metadata?.TryGetValue(CityKey, out var value) == true ? value : null;

    /// <summary>Gets the registration city from vehicle plate validation result.</summary>
    public static string? RegistrationCity(this ValidationResult result)
        => result.Metadata?.TryGetValue(RegistrationCityKey, out var value) == true ? value : null;

    /// <summary>Gets the plate prefix from vehicle plate validation result.</summary>
    public static string? PlatePrefix(this ValidationResult result)
        => result.Metadata?.TryGetValue(PrefixKey, out var value) == true ? value : null;

    /// <summary>Gets the plate number from vehicle plate validation result.</summary>
    public static string? PlateNumber(this ValidationResult result)
        => result.Metadata?.TryGetValue(NumberKey, out var value) == true ? value : null;

    /// <summary>Gets the jurisdiction from STRN validation result.</summary>
    public static string? Jurisdiction(this ValidationResult result)
        => result.Metadata?.TryGetValue(JurisdictionKey, out var value) == true ? value : null;

    /// <summary>Gets the region code from STRN validation result.</summary>
    public static string? RegionCode(this ValidationResult result)
        => result.Metadata?.TryGetValue(RegionCodeKey, out var value) == true ? value : null;

    /// <summary>Gets any metadata value by key.</summary>
    public static string? GetMetadata(this ValidationResult result, string key)
        => result.Metadata?.TryGetValue(key, out var value) == true ? value : null;
}
