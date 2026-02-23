# 🇵🇰 PakValidate

A comprehensive .NET validation library for Pakistani data formats. Validate CNIC, mobile numbers, NTN, IBAN, postal codes, landline numbers, vehicle plates, and STRN — with rich metadata extraction.

[![NuGet](https://img.shields.io/nuget/v/PakValidate.svg)](https://www.nuget.org/packages/PakValidate)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

| Validator | Validates | Extracts |
|-----------|-----------|----------|
| **CNIC** | 13-digit national identity card | Gender, Province, Locality Code |
| **Mobile** | Pakistani mobile numbers (all formats) | Carrier (Jazz/Telenor/Zong/Ufone), E.164 format |
| **NTN** | National Tax Number (FBR) | Standard vs CNIC-based type |
| **IBAN** | Pakistani bank accounts (MOD-97) | Bank name, Account number |
| **Postal Code** | 5-digit postal codes | Region/City |
| **Landline** | City landline numbers | Area code, City |
| **Vehicle Plate** | Registration plates | Registration city |
| **STRN** | Sales Tax Registration Number | RTO Jurisdiction |

## Installation

```bash
# Core library
dotnet add package PakValidate

# FluentValidation extensions (optional)
dotnet add package PakValidate.FluentValidation

# Data Annotations attributes (optional)
dotnet add package PakValidate.DataAnnotations
```

## Quick Start

```csharp
using PakValidate;

// Simple validation
bool isValid = Pak.Cnic.IsValid("35202-1234567-1"); // true

// Rich validation with metadata
var result = Pak.Cnic.Validate("35202-1234567-1");
if (result.IsValid)
{
    Console.WriteLine(result.Gender());    // Male
    Console.WriteLine(result.Province());  // Punjab
    Console.WriteLine(result.Formatted()); // 35202-1234567-1
}

// Mobile number with carrier detection
var mobile = Pak.Mobile.Validate("03001234567");
Console.WriteLine(mobile.Carrier());       // Jazz

// IBAN with bank identification
var iban = Pak.Iban.Validate("PK36SCBL0000001123456702");
Console.WriteLine(iban.BankName());        // Standard Chartered Pakistan

// Implicit bool conversion
if (Pak.Mobile.Validate(phoneNumber))
{
    // Valid!
}
```

## Validation Approaches

Choose the approach that best fits your needs:

### 1. Direct API (Core Package)

```csharp
using PakValidate;

var result = Pak.Cnic.Validate("35202-1234567-1");
result.ThrowIfInvalid();                              // Throw if invalid
var gender = result.Gender();                         // Extract metadata

// Batch validation
var batch = Pak.ValidateAll(
    (nameof(model.Cnic), () => Pak.Cnic.Validate(model.Cnic)),
    (nameof(model.Mobile), () => Pak.Mobile.Validate(model.Mobile))
);
foreach (var (field, error) in batch.GetErrors())
    Console.WriteLine($"{field}: {error}");
```

### 2. FluentValidation

```csharp
using PakValidate.FluentValidation;
using FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.IdCard).NotEmpty().IsValidCnic();
        RuleFor(x => x.PhoneNumber).IsValidPakistaniMobile();
        RuleFor(x => x.BankAccount).IsValidPakistaniIban();
    }
}

// Use in controller/service
var validator = new UserValidator();
var result = validator.Validate(user);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
}
```

**Available methods:** `.IsValidCnic()`, `.IsValidPakistaniMobile()`, `.IsValidNtn()`, `.IsValidPakistaniIban()`, `.IsValidPakistaniPostalCode()`, `.IsValidPakistaniLandline()`, `.IsValidPakistaniVehiclePlate()`, `.IsValidStrn()`, `.IsValidPakistaniPhone()`

### 3. Data Annotations

```csharp
using PakValidate.DataAnnotations;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Required]
    [PakCnic]
    public string IdCard { get; set; }

    [PakMobile]
    public string? PhoneNumber { get; set; }

    [PakIban]
    public string? BankAccount { get; set; }
}

// Validate
var context = new ValidationContext(user);
var results = new List<ValidationResult>();
if (!Validator.TryValidateObject(user, context, results, validateAllProperties: true))
{
    foreach (var error in results)
        ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
}
```

**Available attributes:** `[PakCnic]`, `[PakMobile]`, `[PakNtn]`, `[PakIban]`, `[PakPostalCode]`, `[PakLandline]`, `[PakVehiclePlate]`, `[PakStrn]`

> **Note:** Attributes return `true` for null. Use `[Required]` separately for mandatory fields.

## Validators

### CNIC

```csharp
var result = Pak.Cnic.Validate("35202-1234567-1");
// Accepts: 35202-1234567-1, 3520212345671

// Metadata: Gender, Province, LocalityCode, Formatted
Console.WriteLine(result.Gender());     // Male
Console.WriteLine(result.Province());   // Punjab
```

**Province codes:** 1=KP, 2=FATA/Merged, 3=Punjab, 4=Sindh, 5=Balochistan, 6=Islamabad, 7=GB, 8=AJK

### Mobile Number

```csharp
var result = Pak.Mobile.Validate("03001234567");
// Accepts: 03001234567, 0300-1234567, +923001234567, 923001234567

// Metadata: Carrier, LocalFormat, InternationalFormat, E164, Prefix
Console.WriteLine(result.Carrier());              // Jazz
Console.WriteLine(result.InternationalFormat());  // +923001234567
```

**Carriers:** Jazz, Telenor, Zong, Ufone, SCO

### NTN

```csharp
var result = Pak.Ntn.Validate("1234567-8");
// Accepts: Standard (1234567-8) or CNIC-based (13-digit)

// Metadata: Type, Formatted
```

### IBAN

```csharp
var result = Pak.Iban.Validate("PK36SCBL0000001123456702");

// Metadata: BankCode, BankName, AccountNumber, CheckDigits, Formatted
Console.WriteLine(result.BankName());  // Standard Chartered Pakistan
```

**Supports:** 25+ Pakistani banks (HBL, UBL, MCB, Meezan, Allied, Bank Alfalah, etc.)

### Postal Code

```csharp
var result = Pak.PostalCode.Validate("44000");
// Metadata: Region, RegionPrefix

Console.WriteLine(result.Region());  // Islamabad
```

### Landline

```csharp
var result = Pak.Landline.Validate("051-1234567");
// Accepts: 021-12345678, 051-1234567, +92-51-1234567

// Metadata: AreaCode, City, LocalFormat, InternationalFormat
```

### Vehicle Plate

```csharp
var result = Pak.VehiclePlate.Validate("LEA-1234");
// Accepts: LEA-1234, RI-5678, ISB-123, G-1234 (including government plates)

// Metadata: Prefix, Number, RegistrationCity, Formatted
```

### STRN

```csharp
var result = Pak.Strn.Validate("1312345678901");
// 13-digit Sales Tax Registration Number

// Metadata: Jurisdiction, RegionCode, Formatted
```

## Extension Methods

### Metadata Access

All validation results support property-style metadata access:

```csharp
var result = Pak.Cnic.Validate("35202-1234567-1");

string? gender = result.Gender();      // Cleaner than result["Gender"]
string? province = result.Province();
string? formatted = result.Formatted();

// Works with any validator
var mobile = Pak.Mobile.Validate("03001234567");
var carrier = mobile.Carrier();
var e164 = mobile.E164();

// Generic access for any metadata
string? value = result.GetMetadata("CustomKey");
```

### Error Handling

```csharp
var result = Pak.Cnic.Validate(cnic);

// Throw if invalid
result.ThrowIfInvalid();

// Check if invalid (inverse of IsValid)
if (result.IsInvalid())
    Console.WriteLine(result.ErrorMessage);

// Get error with default
string error = result.GetErrorOrDefault("Invalid input");
```

### Result Mapping

Transform validation results using functional patterns:

```csharp
// Extract metadata on success
string? gender = Pak.Cnic.Validate(cnic).Map(
    r => r.Gender(),
    error => null
);

// Complex transformation
var mobileInfo = Pak.Mobile.Validate(phone).Map(
    r => new { Carrier = r.Carrier(), Format = r.LocalFormat() },
    error => null
);

// Execute side effects
Pak.Cnic.Validate(cnic).Match(
    r => Console.WriteLine($"Valid: {r.Gender()}"),
    error => Console.WriteLine($"Invalid: {error}")
);
```

### Batch Validation

Validate multiple fields and collect all errors:

```csharp
var batch = Pak.ValidateAll(
    (nameof(model.Cnic), () => Pak.Cnic.Validate(model.Cnic)),
    (nameof(model.Mobile), () => Pak.Mobile.Validate(model.Mobile)),
    (nameof(model.Iban), () => Pak.Iban.Validate(model.Iban))
);

// Check all results
if (batch.IsValid) { /* all passed */ }
if (batch.IsInvalid()) { /* at least one failed */ }

// Get specific field error
var cnicError = batch.GetError(nameof(model.Cnic));

// Iterate all errors
foreach (var (field, error) in batch.GetErrors())
    Console.WriteLine($"{field}: {error}");

// Throw if any failed
batch.ThrowIfInvalid();
```

**Returns:**
- `IsValid` — true if all validations passed
- `Errors` — dictionary of failed fields (only failures)
- `Results` — all validation results by field name

### Custom Property Names

Batch validation works with any property naming:

```csharp
public class User
{
    public string IdCard { get; set; }      // Different name!
    public string PhoneNumber { get; set; }
    public string BankAccount { get; set; }
}

var batch = Pak.ValidateAll(
    (nameof(User.IdCard), () => Pak.Cnic.Validate(user.IdCard)),
    (nameof(User.PhoneNumber), () => Pak.Mobile.Validate(user.PhoneNumber)),
    (nameof(User.BankAccount), () => Pak.Iban.Validate(user.BankAccount))
);

// Errors keyed by YOUR property names
if (!batch.IsValid)
{
    foreach (var (field, error) in batch.GetErrors())
        ModelState.AddModelError(field, error);
}
```

## ValidationResult

Every `Validate()` call returns a `ValidationResult`:

```csharp
public class ValidationResult
{
    bool IsValid { get; }              // Pass/fail
    string? ErrorMessage { get; }      // Error if invalid
    string? Sanitized { get; }         // Cleaned input (digits only)
    IReadOnlyDictionary<string, string> Metadata { get; } // Extracted data

    // Implicit bool conversion
    public static implicit operator bool(ValidationResult result);
}
```

## Supported Frameworks

- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET 9.0
- .NET 10.0

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
