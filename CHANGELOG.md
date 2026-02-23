# Changelog

All notable changes to PakValidate will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2026-02-23

### Added

- **Error Handling Extensions** — `ThrowIfInvalid()`, `IsInvalid()`, `GetErrorOrDefault()` for cleaner error handling patterns
- **Result Mapping Extensions** — `Map<T>()` and `Match()` for functional composition and side-effect handling
- **Batch Validation Extensions** — `GetError()`, `GetErrors()` tuple enumeration, enhanced `ThrowIfInvalid()` for batch operations
- **ValidationException Type** — Dedicated exception class thrown by `ThrowIfInvalid()` methods
- **23 New Tests** — Comprehensive test coverage for all new extension methods (total 285 tests)

## [1.1.0] - 2026-02-23

### Added

- **Data Annotations Support** — New `PakValidate.DataAnnotations` NuGet package with validation attributes (`[PakCnic]`, `[PakMobile]`, `[PakIban]`, etc.) for System.ComponentModel.DataAnnotations integration
- **Batch Validation** — `Pak.ValidateAll()` method for validating multiple fields at once with combined results
- **BatchValidationResult Type** — Returns all results plus error dictionary for easy error handling
- **Extension Methods for Metadata** — `result.Gender()`, `result.Carrier()`, `result.Region()`, etc. for property-style metadata access instead of dictionary syntax
- **Metadata Constants** — Private const string keys in ValidationResultExtensions to eliminate magic strings
- **.NET 9.0 and 10.0 Support** — Updated target frameworks; CI/CD now tests on all frameworks (6.0, 7.0, 8.0, 9.0, 10.0)
- **AJK CNIC Code Documentation** — Clarified province code 8 represents Azad Jammu & Kashmir

## [1.0.0] - 2026-02-23

### Added

- **CNIC Validator** — 13-digit NADRA identity card validation with gender, province, and locality extraction
- **Mobile Validator** — Pakistani mobile number validation with carrier detection (Jazz, Telenor, Zong, Ufone, SCO) and E.164 formatting
- **NTN Validator** — National Tax Number validation (standard 8-digit and CNIC-based 13-digit formats)
- **IBAN Validator** — Pakistani IBAN validation with MOD-97 check digit verification and bank identification (25+ banks)
- **Postal Code Validator** — 5-digit postal code validation with region/city mapping
- **Landline Validator** — Landline number validation with area code and city detection
- **Vehicle Plate Validator** — Registration plate validation with city identification and government plate support
- **STRN Validator** — Sales Tax Registration Number validation with RTO jurisdiction detection
- **FluentValidation Extensions** — Drop-in `.IsValidCnic()`, `.IsValidPakistaniMobile()`, etc. for FluentValidation
- **Unified `Pak.*` API** — Single entry point: `Pak.Cnic.Validate()`, `Pak.Mobile.GetCarrier()`, etc.
- **Multi-target** — .NET 6.0, 7.0, and 8.0 support
- **Source Link** — Debugger support for stepping into PakValidate source
- **CI/CD** — GitHub Actions pipeline with multi-framework testing and NuGet publish
