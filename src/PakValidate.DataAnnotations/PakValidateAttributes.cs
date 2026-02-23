using System.ComponentModel.DataAnnotations;
using PakValidate.Validators;

namespace PakValidate.DataAnnotations;

/// <summary>
/// Validates that a property is a valid Pakistani CNIC (Computerized National Identity Card).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakCnicAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakCnicAttribute class.</summary>
    public PakCnicAttribute()
        : base("The field {0} must be a valid Pakistani CNIC (e.g., 35202-1234567-1).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true; // Use [Required] separately for required fields
        if (value is not string str) return false;
        return CnicValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani mobile number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakMobileAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakMobileAttribute class.</summary>
    public PakMobileAttribute()
        : base("The field {0} must be a valid Pakistani mobile number (e.g., 03001234567).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return MobileValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani NTN (National Tax Number).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakNtnAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakNtnAttribute class.</summary>
    public PakNtnAttribute()
        : base("The field {0} must be a valid Pakistani NTN (e.g., 1234567-8).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return NtnValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani IBAN (International Bank Account Number).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakIbanAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakIbanAttribute class.</summary>
    public PakIbanAttribute()
        : base("The field {0} must be a valid Pakistani IBAN (e.g., PK36SCBL0000001123456702).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return IbanValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani postal code.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakPostalCodeAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakPostalCodeAttribute class.</summary>
    public PakPostalCodeAttribute()
        : base("The field {0} must be a valid Pakistani postal code (5 digits).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return PostalCodeValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani landline number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakLandlineAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakLandlineAttribute class.</summary>
    public PakLandlineAttribute()
        : base("The field {0} must be a valid Pakistani landline number (e.g., 051-1234567).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return LandlineValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani vehicle registration plate.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakVehiclePlateAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakVehiclePlateAttribute class.</summary>
    public PakVehiclePlateAttribute()
        : base("The field {0} must be a valid Pakistani vehicle registration plate (e.g., LEA-1234).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return VehiclePlateValidator.IsValid(str);
    }
}

/// <summary>
/// Validates that a property is a valid Pakistani STRN (Sales Tax Registration Number).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PakStrnAttribute : ValidationAttribute
{
    /// <summary>Initializes a new instance of the PakStrnAttribute class.</summary>
    public PakStrnAttribute()
        : base("The field {0} must be a valid Pakistani STRN (13 digits).") { }

    /// <summary>Validates the specified value.</summary>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not string str) return false;
        return StrnValidator.IsValid(str);
    }
}
