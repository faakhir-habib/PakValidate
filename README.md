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
    Console.WriteLine(result.Metadata["Gender"]);   // Male
    Console.WriteLine(result.Metadata["Province"]);  // Punjab
    Console.WriteLine(result.Metadata["Formatted"]); // 35202-1234567-1
}

// Mobile number with carrier detection
var mobile = Pak.Mobile.Validate("03001234567");
Console.WriteLine(mobile.Metadata["Carrier"]);            // Jazz
Console.WriteLine(mobile.Metadata["InternationalFormat"]); // +923001234567

// IBAN with bank identification
var iban = Pak.Iban.Validate("PK36SCBL0000001123456702");
Console.WriteLine(iban.Metadata["BankName"]); // Standard Chartered Pakistan

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

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
