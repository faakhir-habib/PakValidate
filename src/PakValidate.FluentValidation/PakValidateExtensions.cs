using FluentValidation;
using PakValidate.Validators;

namespace PakValidate.FluentValidation;

/// <summary>
/// FluentValidation extensions for Pakistani data validation.
/// 
/// Usage:
///   public class CustomerValidator : AbstractValidator&lt;Customer&gt;
///   {
///       public CustomerValidator()
///       {
///           RuleFor(x => x.Cnic).IsValidCnic();
///           RuleFor(x => x.Phone).IsValidPakistaniMobile();
///           RuleFor(x => x.Iban).IsValidPakistaniIban();
///           RuleFor(x => x.Ntn).IsValidNtn();
///       }
///   }
/// </summary>
public static class PakValidateExtensions
{
    /// <summary>
    /// Validates that the property is a valid Pakistani CNIC number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidCnic<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true; // Use .NotEmpty() separately for required
            return CnicValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani CNIC (e.g., 35202-1234567-1).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani mobile number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniMobile<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return MobileValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani mobile number (e.g., 03001234567).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani NTN.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidNtn<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return NtnValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani NTN (e.g., 1234567-8).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani IBAN.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniIban<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return IbanValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani IBAN (e.g., PK36SCBL0000001123456702).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani postal code.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniPostalCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return PostalCodeValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani postal code (5 digits).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani landline number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniLandline<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return LandlineValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani landline number (e.g., 051-1234567).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani vehicle registration plate.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniVehiclePlate<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return VehiclePlateValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani vehicle plate (e.g., LEA-1234).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani STRN.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidStrn<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return StrnValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani STRN (13 digits).");
    }

    /// <summary>
    /// Validates that the property is a valid Pakistani phone number (mobile OR landline).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidPakistaniPhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value =>
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return MobileValidator.IsValid(value) || LandlineValidator.IsValid(value);
        }).WithMessage("'{PropertyName}' must be a valid Pakistani phone number.");
    }
}
