using PakValidate.Validators;

namespace PakValidate;

/// <summary>
/// PakValidate — A comprehensive validation library for Pakistani data formats.
/// Provides a single entry point for all validators.
/// 
/// Usage:
///   var result = Pak.Cnic.Validate("35202-1234567-1");
///   if (result.IsValid) Console.WriteLine(result.Metadata["Gender"]); // Male
///   
///   // Quick checks
///   bool valid = Pak.Mobile.IsValid("03001234567");
///   string? carrier = Pak.Mobile.GetCarrier("03001234567"); // Jazz
/// </summary>
public static class Pak
{
    /// <summary>CNIC validation (13-digit national identity card).</summary>
    public static class Cnic
    {
        /// <inheritdoc cref="CnicValidator.Validate"/>
        public static ValidationResult Validate(string? cnic) => CnicValidator.Validate(cnic);

        /// <inheritdoc cref="CnicValidator.IsValid"/>
        public static bool IsValid(string? cnic) => CnicValidator.IsValid(cnic);

        /// <inheritdoc cref="CnicValidator.Format"/>
        public static string? Format(string? cnic) => CnicValidator.Format(cnic);
    }

    /// <summary>Mobile number validation with carrier detection.</summary>
    public static class Mobile
    {
        /// <inheritdoc cref="MobileValidator.Validate"/>
        public static ValidationResult Validate(string? mobile) => MobileValidator.Validate(mobile);

        /// <inheritdoc cref="MobileValidator.IsValid"/>
        public static bool IsValid(string? mobile) => MobileValidator.IsValid(mobile);

        /// <inheritdoc cref="MobileValidator.GetCarrier"/>
        public static string? GetCarrier(string? mobile) => MobileValidator.GetCarrier(mobile);

        /// <inheritdoc cref="MobileValidator.ToInternational"/>
        public static string? ToInternational(string? mobile) => MobileValidator.ToInternational(mobile);
    }

    /// <summary>NTN (National Tax Number) validation.</summary>
    public static class Ntn
    {
        /// <inheritdoc cref="NtnValidator.Validate"/>
        public static ValidationResult Validate(string? ntn) => NtnValidator.Validate(ntn);

        /// <inheritdoc cref="NtnValidator.IsValid"/>
        public static bool IsValid(string? ntn) => NtnValidator.IsValid(ntn);

        /// <inheritdoc cref="NtnValidator.Format"/>
        public static string? Format(string? ntn) => NtnValidator.Format(ntn);
    }

    /// <summary>IBAN validation with bank identification (MOD-97 verified).</summary>
    public static class Iban
    {
        /// <inheritdoc cref="IbanValidator.Validate"/>
        public static ValidationResult Validate(string? iban) => IbanValidator.Validate(iban);

        /// <inheritdoc cref="IbanValidator.IsValid"/>
        public static bool IsValid(string? iban) => IbanValidator.IsValid(iban);

        /// <inheritdoc cref="IbanValidator.GetBankName"/>
        public static string? GetBankName(string? iban) => IbanValidator.GetBankName(iban);
    }

    /// <summary>Postal code validation with region detection.</summary>
    public static class PostalCode
    {
        /// <inheritdoc cref="PostalCodeValidator.Validate"/>
        public static ValidationResult Validate(string? postalCode) => PostalCodeValidator.Validate(postalCode);

        /// <inheritdoc cref="PostalCodeValidator.IsValid"/>
        public static bool IsValid(string? postalCode) => PostalCodeValidator.IsValid(postalCode);

        /// <inheritdoc cref="PostalCodeValidator.GetRegion"/>
        public static string? GetRegion(string? postalCode) => PostalCodeValidator.GetRegion(postalCode);
    }

    /// <summary>Landline number validation with city detection.</summary>
    public static class Landline
    {
        /// <inheritdoc cref="LandlineValidator.Validate"/>
        public static ValidationResult Validate(string? landline) => LandlineValidator.Validate(landline);

        /// <inheritdoc cref="LandlineValidator.IsValid"/>
        public static bool IsValid(string? landline) => LandlineValidator.IsValid(landline);

        /// <inheritdoc cref="LandlineValidator.GetCity"/>
        public static string? GetCity(string? landline) => LandlineValidator.GetCity(landline);
    }

    /// <summary>Vehicle registration plate validation.</summary>
    public static class VehiclePlate
    {
        /// <inheritdoc cref="VehiclePlateValidator.Validate"/>
        public static ValidationResult Validate(string? plate) => VehiclePlateValidator.Validate(plate);

        /// <inheritdoc cref="VehiclePlateValidator.IsValid"/>
        public static bool IsValid(string? plate) => VehiclePlateValidator.IsValid(plate);

        /// <inheritdoc cref="VehiclePlateValidator.GetCity"/>
        public static string? GetCity(string? plate) => VehiclePlateValidator.GetCity(plate);
    }

    /// <summary>STRN (Sales Tax Registration Number) validation.</summary>
    public static class Strn
    {
        /// <inheritdoc cref="StrnValidator.Validate"/>
        public static ValidationResult Validate(string? strn) => StrnValidator.Validate(strn);

        /// <inheritdoc cref="StrnValidator.IsValid"/>
        public static bool IsValid(string? strn) => StrnValidator.IsValid(strn);

        /// <inheritdoc cref="StrnValidator.Format"/>
        public static string? Format(string? strn) => StrnValidator.Format(strn);
    }
}
