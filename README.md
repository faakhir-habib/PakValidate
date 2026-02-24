# 🇵🇰 PakValidate

A comprehensive .NET validation library for Pakistani data formats. Validate CNIC, mobile numbers, NTN, IBAN, postal codes, landline numbers, vehicle plates, and STRN — with rich metadata extraction.

[![NuGet](https://img.shields.io/nuget/v/PakValidate.svg)](https://www.nuget.org/packages/PakValidate)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CI/CD](https://github.com/faakhir-habib/PakValidate/actions/workflows/ci.yml/badge.svg)](https://github.com/faakhir-habib/PakValidate/actions/workflows/ci.yml)

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
Console.WriteLine(mobile.Carrier());            // Jazz
Console.WriteLine(mobile.InternationalFormat()); // +923001234567

// IBAN with bank identification
var iban = Pak.Iban.Validate("PK36SCBL0000001123456702");
Console.WriteLine(iban.BankName()); // Standard Chartered Pakistan

// Implicit bool conversion
if (Pak.Mobile.Validate(phoneNumber))
{
    // Valid!
}
```

## Validators

### CNIC

```csharp
// Accepts: 35202-1234567-1, 3520212345671
var result = Pak.Cnic.Validate("35202-1234567-1");
// Metadata: Gender, Province, LocalityCode, Formatted

string formatted = Pak.Cnic.Format("3520212345671"); // 35202-1234567-1
```

**Province codes:** 1=KP, 2=FATA/Merged, 3=Punjab, 4=Sindh, 5=Balochistan, 6=Islamabad, 7=GB, 8=AJK

### Mobile Number

```csharp
// Accepts: 03001234567, 0300-1234567, +923001234567, 923001234567
var result = Pak.Mobile.Validate("03001234567");
// Metadata: Carrier, LocalFormat, InternationalFormat, E164, Prefix

string? carrier = Pak.Mobile.GetCarrier("03451234567"); // Telenor
string? intl = Pak.Mobile.ToInternational("03001234567"); // +923001234567
```

**Supported carriers:** Jazz, Telenor, Zong, Ufone, SCO

### NTN (National Tax Number)

```csharp
// Standard: 1234567-8 | CNIC-based: 3520212345671
var result = Pak.Ntn.Validate("1234567-8");
// Metadata: Type (Standard/CNIC-based), Formatted
```

### IBAN

```csharp
// Format: PK## XXXX ################
var result = Pak.Iban.Validate("PK36SCBL0000001123456702");
// Metadata: BankCode, BankName, AccountNumber, CheckDigits, Formatted

string? bank = Pak.Iban.GetBankName("PK36SCBL0000001123456702"); // Standard Chartered Pakistan
```

**Supports 25+ Pakistani banks** including HBL, UBL, MCB, Meezan, Allied, Bank Alfalah, and more.

### Postal Code

```csharp
var result = Pak.PostalCode.Validate("44000");
// Metadata: Region (Islamabad), RegionPrefix

string? region = Pak.PostalCode.GetRegion("75500"); // Karachi
```

### Landline

```csharp
// Accepts: 021-12345678, 051-1234567, +92-51-1234567
var result = Pak.Landline.Validate("051-1234567");
// Metadata: AreaCode, City, LocalFormat, InternationalFormat
```

### Vehicle Registration Plate

```csharp
// Accepts: LEA-1234, RI-5678, ISB-123, G-1234
var result = Pak.VehiclePlate.Validate("LEA-1234");
// Metadata: Prefix, Number, RegistrationCity, Formatted
```

### STRN (Sales Tax Registration Number)

```csharp
var result = Pak.Strn.Validate("1312345678901");
// Metadata: Jurisdiction (RTO Islamabad), RegionCode, Formatted
```

## FluentValidation Extensions

```bash
dotnet add package PakValidate.FluentValidation
```

```csharp
using PakValidate.FluentValidation;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Cnic)
            .NotEmpty()
            .IsValidCnic();

        RuleFor(x => x.Phone)
            .IsValidPakistaniMobile();

        RuleFor(x => x.Iban)
            .IsValidPakistaniIban();

        RuleFor(x => x.Ntn)
            .IsValidNtn();

        RuleFor(x => x.PostalCode)
            .IsValidPakistaniPostalCode();

        RuleFor(x => x.VehiclePlate)
            .IsValidPakistaniVehiclePlate();

        RuleFor(x => x.Strn)
            .IsValidStrn();

        // Validates both mobile and landline
        RuleFor(x => x.AnyPhone)
            .IsValidPakistaniPhone();
    }
}
```

## Data Annotations

```bash
dotnet add package PakValidate.DataAnnotations
```

Use `[PakCnic]`, `[PakMobile]`, `[PakIban]`, and other validation attributes directly on model properties:

```csharp
using PakValidate.DataAnnotations;

public class Customer
{
    [Required]
    [PakCnic]
    public string Cnic { get; set; }

    [PakMobile]
    public string? PhoneNumber { get; set; }

    [PakIban]
    public string? BankAccount { get; set; }

    [PakPostalCode]
    public string? PostalCode { get; set; }

    [PakVehiclePlate]
    public string? VehiclePlate { get; set; }

    [PakNtn]
    public string? NTN { get; set; }

    [PakLandline]
    public string? Landline { get; set; }

    [PakStrn]
    public string? STRN { get; set; }
}

// In a controller or service:
var customer = new Customer { Cnic = "35202-1234567-1" };
var context = new ValidationContext(customer);
var results = new List<ValidationResult>();
bool isValid = Validator.TryValidateObject(customer, context, results, validateAllProperties: true);
```

**Note:** Attributes return `true` for null values. Use `[Required]` separately if the field is mandatory.

## Batch Validation

Validate multiple fields at once with `Pak.ValidateAll()`:

```csharp
public class Customer
{
    public string Cnic { get; set; }
    public string Mobile { get; set; }
    public string Iban { get; set; }
    public string Ntn { get; set; }
}

var customer = new Customer
{
    Cnic = "35202-1234567-1",
    Mobile = "03001234567",
    Iban = "PK36SCBL0000001123456702",
    Ntn = "1234567-8"
};

// Validate all fields at once
var batch = Pak.ValidateAll(
    (nameof(Customer.Cnic), () => Pak.Cnic.Validate(customer.Cnic)),
    (nameof(Customer.Mobile), () => Pak.Mobile.Validate(customer.Mobile)),
    (nameof(Customer.Iban), () => Pak.Iban.Validate(customer.Iban)),
    (nameof(Customer.Ntn), () => Pak.Ntn.Validate(customer.Ntn))
);

// Check results
if (batch.IsValid)
{
    Console.WriteLine("All validations passed!");
}
else
{
    foreach (var (field, error) in batch.GetErrors())
    {
        Console.WriteLine($"{field}: {error}");
    }
}

// Access specific field error
var cnicError = batch.GetError(nameof(Customer.Cnic));
if (cnicError != null)
{
    Console.WriteLine($"CNIC validation failed: {cnicError}");
}
```

**Returns:** `BatchValidationResult` with:
- `IsValid` — true if all validations passed
- `Errors` — dictionary of failed fields and their error messages (only failures)
- `Results` — all validation results by field name

## Extension Methods for Metadata

Instead of accessing metadata via dictionary syntax, use extension methods for a cleaner, property-style API:

```csharp
using PakValidate; // Includes ValidationResultExtensions

var result = Pak.Cnic.Validate("35202-1234567-1");

// Property-style access (recommended)
string? gender = result.Gender();      // "Male"
string? province = result.Province();  // "Punjab"
string? formatted = result.Formatted(); // "35202-1234567-1"

// Mobile numbers
var mobile = Pak.Mobile.Validate("03001234567");
string? carrier = mobile.Carrier();                // "Jazz"
string? e164 = mobile.E164();                      // "+923001234567"
string? localFormat = mobile.LocalFormat();        // "03001234567"

// IBAN
var iban = Pak.Iban.Validate("PK36SCBL0000001123456702");
string? bankName = iban.BankName();        // "Standard Chartered Pakistan"
string? bankCode = iban.BankCode();        // "SCBL"
string? accountNumber = iban.AccountNumber(); // "0000001123456702"

// Postal codes
var postal = Pak.PostalCode.Validate("44000");
string? region = postal.Region();        // "Islamabad"
string? prefix = postal.RegionPrefix();  // "44"

// Vehicle plates
var plate = Pak.VehiclePlate.Validate("LEA-1234");
string? city = plate.City();                  // "Lahore"
string? regCity = plate.RegistrationCity();   // "Lahore"
string? platePrefix = plate.PlatePrefix();    // "LEA"
string? plateNumber = plate.PlateNumber();    // "1234"

// Any metadata by key
string? custom = result.GetMetadata("CustomKey");
```

All extension methods return `string?` (nullable) and safely handle missing metadata keys.

## Error Handling Extensions

Clean error handling with dedicated methods:

```csharp
var result = Pak.Cnic.Validate(cnic);

// Throw exception if invalid
result.ThrowIfInvalid();  // Throws ValidationException if not valid

// Check if invalid (inverse of IsValid)
if (result.IsInvalid())
{
    Console.WriteLine("Validation failed");
}

// Get error message with default
string error = result.GetErrorOrDefault("No errors found");
```

## Result Mapping Extensions

Process validation results using functional patterns:

```csharp
// Map: Transform result to new value
string? gender = Pak.Cnic.Validate(cnic).Map(
    r => r.Gender(),              // On success, extract gender
    error => null                 // On failure, return null
);

// Map with complex return type
var mobileInfo = Pak.Mobile.Validate(phone).Map(
    r => new { Carrier = r.Carrier(), Format = r.LocalFormat() },
    error => null
);

// Match: Execute actions based on validation result
Pak.Cnic.Validate(cnic).Match(
    r => Console.WriteLine($"Valid: {r.Gender()}"),
    error => Console.WriteLine($"Invalid: {error}")
);
```

## Batch Validation Extensions

Enhanced batch validation error handling:

```csharp
var batch = Pak.ValidateAll(
    (nameof(model.Cnic), () => Pak.Cnic.Validate(model.Cnic)),
    (nameof(model.Mobile), () => Pak.Mobile.Validate(model.Mobile))
);

// Get specific field error
string? cnicError = batch.GetError(nameof(model.Cnic));

// Iterate all errors with tuples
foreach (var (field, error) in batch.GetErrors())
    Console.WriteLine($"{field}: {error}");

// Throw if any validation failed
batch.ThrowIfInvalid();  // Throws ValidationException with field list

// Check if invalid
if (batch.IsInvalid())
    Console.WriteLine("Some fields failed validation");
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
