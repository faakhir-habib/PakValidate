using FluentAssertions;
using PakValidate;
using PakValidate.Validators;
using Xunit;

namespace PakValidate.Tests;

#region CNIC Tests

public class CnicValidatorTests
{
    [Theory]
    [InlineData("35202-1234567-1")]
    [InlineData("3520212345671")]
    [InlineData("61101-1234567-3")]
    [InlineData("42201-9876543-0")]
    [InlineData("17301-5555555-7")]
    [InlineData("  35202-1234567-1  ")] // with whitespace
    public void Validate_ValidCnic_ReturnsSuccess(string cnic)
    {
        Pak.Cnic.Validate(cnic).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "CNIC is required.")]
    [InlineData("", "CNIC is required.")]
    [InlineData("   ", "CNIC is required.")]
    [InlineData("1234", "CNIC must be 13 digits")]
    [InlineData("12345678901234", "CNIC must be 13 digits")]
    [InlineData("abcde-fghijkl-m", "CNIC must be 13 digits")]
    [InlineData("1111111111111", "CNIC cannot contain all identical digits")]
    [InlineData("0000000000000", "CNIC cannot contain all identical digits")]
    public void Validate_InvalidCnic_ReturnsFailure(string? cnic, string expectedErrorContains)
    {
        var result = Pak.Cnic.Validate(cnic);
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain(expectedErrorContains);
    }

    [Theory]
    [InlineData("35202-1234567-1", "Male")]
    [InlineData("35202-1234567-3", "Male")]
    [InlineData("35202-1234567-5", "Male")]
    [InlineData("35202-1234567-0", "Female")]
    [InlineData("35202-1234567-2", "Female")]
    [InlineData("35202-1234567-4", "Female")]
    public void Validate_ExtractsGenderCorrectly(string cnic, string expectedGender)
    {
        Pak.Cnic.Validate(cnic).Metadata["Gender"].Should().Be(expectedGender);
    }

    [Theory]
    [InlineData("35202-1234567-1", "Punjab")]
    [InlineData("42201-1234567-1", "Sindh")]
    [InlineData("61101-1234567-1", "Islamabad")]
    [InlineData("17301-1234567-1", "Khyber Pakhtunkhwa")]
    [InlineData("51001-1234567-1", "Balochistan")]
    [InlineData("21001-1234567-1", "FATA / Merged Areas")]
    [InlineData("71001-1234567-1", "Gilgit-Baltistan")]
    [InlineData("81001-1234567-1", "Azad Jammu & Kashmir")]
    public void Validate_ExtractsProvinceCorrectly(string cnic, string expectedProvince)
    {
        Pak.Cnic.Validate(cnic).Metadata["Province"].Should().Be(expectedProvince);
    }

    [Fact]
    public void Format_ReturnsFormattedCnic()
    {
        Pak.Cnic.Format("3520212345671").Should().Be("35202-1234567-1");
        Pak.Cnic.Format("35202-1234567-1").Should().Be("35202-1234567-1");
    }

    [Fact]
    public void Format_ReturnsNullForInvalid()
    {
        Pak.Cnic.Format("invalid").Should().BeNull();
        Pak.Cnic.Format(null).Should().BeNull();
    }

    [Fact]
    public void IsValid_ReturnsBool()
    {
        Pak.Cnic.IsValid("35202-1234567-1").Should().BeTrue();
        Pak.Cnic.IsValid("invalid").Should().BeFalse();
    }

    [Fact]
    public void Sanitized_ReturnsDigitsOnly()
    {
        Pak.Cnic.Validate("35202-1234567-1")!.Sanitized.Should().Be("3520212345671");
    }

    [Fact]
    public void ImplicitBoolConversion_Works()
    {
        ValidationResult valid = Pak.Cnic.Validate("35202-1234567-1");
        ValidationResult invalid = Pak.Cnic.Validate("bad");

        (valid ? true : false).Should().BeTrue();
        (invalid ? true : false).Should().BeFalse();
    }
}

#endregion

#region Mobile Tests

public class MobileValidatorTests
{
    [Theory]
    [InlineData("03001234567")]
    [InlineData("0300-1234567")]
    [InlineData("+923001234567")]
    [InlineData("923001234567")]
    [InlineData("03451234567")]
    [InlineData("03101234567")]
    [InlineData("03331234567")]
    [InlineData("  0300-1234567  ")] // whitespace
    [InlineData("0300 1234567")]     // space separator
    public void Validate_ValidMobile_ReturnsSuccess(string mobile)
    {
        Pak.Mobile.Validate(mobile).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]
    [InlineData("04001234567")]        // 04xx is not valid mobile
    [InlineData("030012345678")]       // 12 digits — too many
    [InlineData("0300123456")]         // 10 digits — too few
    [InlineData("abcdefghijk")]
    [InlineData("021-12345678")]       // landline, not mobile
    public void Validate_InvalidMobile_ReturnsFailure(string? mobile)
    {
        Pak.Mobile.Validate(mobile).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("03001234567", "Jazz")]
    [InlineData("03011234567", "Jazz")]
    [InlineData("03211234567", "Jazz")]
    [InlineData("03451234567", "Telenor")]
    [InlineData("03401234567", "Telenor")]
    [InlineData("03101234567", "Zong")]
    [InlineData("03151234567", "Zong")]
    [InlineData("03331234567", "Ufone")]
    [InlineData("03301234567", "Ufone")]
    [InlineData("03551234567", "SCO")]
    public void GetCarrier_ReturnsCorrectCarrier(string mobile, string expectedCarrier)
    {
        Pak.Mobile.GetCarrier(mobile).Should().Be(expectedCarrier);
    }

    [Fact]
    public void GetCarrier_ReturnsNullForInvalid()
    {
        Pak.Mobile.GetCarrier("invalid").Should().BeNull();
    }

    [Theory]
    [InlineData("03001234567", "+923001234567")]
    [InlineData("0300-1234567", "+923001234567")]
    [InlineData("+923001234567", "+923001234567")]
    public void ToInternational_FormatsCorrectly(string input, string expected)
    {
        Pak.Mobile.ToInternational(input).Should().Be(expected);
    }

    [Fact]
    public void Validate_ExtractsAllMetadata()
    {
        var result = Pak.Mobile.Validate("03001234567");
        result.Metadata["LocalFormat"].Should().Be("03001234567");
        result.Metadata["InternationalFormat"].Should().Be("+923001234567");
        result.Metadata["E164"].Should().Be("+923001234567");
        result.Metadata["Prefix"].Should().Be("0300");
        result.Metadata["Carrier"].Should().Be("Jazz");
    }
}

#endregion

#region NTN Tests

public class NtnValidatorTests
{
    [Theory]
    [InlineData("1234567-8")]
    [InlineData("12345678")]
    [InlineData("3520212345671")]  // CNIC-based
    [InlineData("9876543-2")]
    [InlineData("  1234567-8  ")] // whitespace
    public void Validate_ValidNtn_ReturnsSuccess(string ntn)
    {
        Pak.Ntn.Validate(ntn).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1234567")]         // only 7 digits, no check digit
    [InlineData("123456789")]       // 9 digits
    [InlineData("11111111")]        // all same digits
    [InlineData("abcdefgh")]
    [InlineData("12345-678")]       // wrong dash position
    [InlineData("1111111111111")]   // all-same CNIC-based
    public void Validate_InvalidNtn_ReturnsFailure(string? ntn)
    {
        Pak.Ntn.Validate(ntn).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DetectsStandardType()
    {
        Pak.Ntn.Validate("12345678")!.Metadata["Type"].Should().Be("Standard");
    }

    [Fact]
    public void Validate_DetectsCnicBasedType()
    {
        Pak.Ntn.Validate("3520212345671")!.Metadata["Type"].Should().Be("CNIC-based");
    }

    [Fact]
    public void Format_WorksCorrectly()
    {
        Pak.Ntn.Format("12345678").Should().Be("1234567-8");
        Pak.Ntn.Format("1234567-8").Should().Be("1234567-8");
        Pak.Ntn.Format("invalid").Should().BeNull();
    }
}

#endregion

#region IBAN Tests

public class IbanValidatorTests
{
    // These IBANs have been verified with correct MOD-97 check digits
    [Theory]
    [InlineData("PK36SCBL0000001123456702")]  // Standard Chartered
    [InlineData("PK66HABB0000001234567890")]  // HBL
    [InlineData("PK75MUCB0000009876543210")]  // MCB
    [InlineData("PK16MEZN0000001111222233")]  // Meezan
    [InlineData("PK30NBPA0000005555666677")]  // NBP
    [InlineData("PK54UNIL0000004444333322")]  // UBL
    [InlineData("PK85ALFH0000001000200030")]  // Alfalah
    [InlineData("pk36scbl0000001123456702")]   // lowercase
    [InlineData("PK36 SCBL 0000 0011 2345 6702")] // with spaces
    public void Validate_ValidIban_ReturnsSuccess(string iban)
    {
        Pak.Iban.Validate(iban).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("GB82WEST12345698765432")]   // UK IBAN
    [InlineData("PK12345")]                   // too short
    [InlineData("PKXXSCBL0000001123456702")] // non-digit check digits
    [InlineData("PK00SCBL0000001123456702")] // wrong check digits (MOD-97 fails)
    [InlineData("PK99SCBL0000001123456702")] // wrong check digits
    [InlineData("DE89370400440532013000")]   // German IBAN
    public void Validate_InvalidIban_ReturnsFailure(string? iban)
    {
        Pak.Iban.Validate(iban).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("PK36SCBL0000001123456702", "Standard Chartered Pakistan")]
    [InlineData("PK66HABB0000001234567890", "Habib Bank Limited")]
    [InlineData("PK75MUCB0000009876543210", "MCB Bank Limited")]
    [InlineData("PK16MEZN0000001111222233", "Meezan Bank Limited")]
    [InlineData("PK30NBPA0000005555666677", "National Bank of Pakistan")]
    [InlineData("PK54UNIL0000004444333322", "United Bank Limited")]
    [InlineData("PK85ALFH0000001000200030", "Alfalah Bank Limited")]
    public void Validate_IdentifiesBank(string iban, string expectedBank)
    {
        var result = Pak.Iban.Validate(iban);
        result.IsValid.Should().BeTrue();
        result.Metadata["BankName"].Should().Be(expectedBank);
    }

    [Fact]
    public void GetBankName_Works()
    {
        Pak.Iban.GetBankName("PK36SCBL0000001123456702").Should().Be("Standard Chartered Pakistan");
        Pak.Iban.GetBankName("invalid").Should().BeNull();
    }

    [Fact]
    public void Validate_ExtractsFormattedIban()
    {
        var result = Pak.Iban.Validate("PK36SCBL0000001123456702");
        result.Metadata["Formatted"].Should().Be("PK36 SCBL 0000 0011 2345 6702");
        result.Metadata["BankCode"].Should().Be("SCBL");
        result.Metadata["CheckDigits"].Should().Be("36");
        result.Metadata["AccountNumber"].Should().Be("0000001123456702");
    }
}

#endregion

#region Postal Code Tests

public class PostalCodeValidatorTests
{
    [Theory]
    [InlineData("44000")]
    [InlineData("75500")]
    [InlineData("54000")]
    [InlineData("10000")]
    [InlineData("80000")]
    [InlineData(" 44000 ")] // whitespace
    public void Validate_ValidPostalCode_ReturnsSuccess(string code)
    {
        Pak.PostalCode.Validate(code).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234")]     // 4 digits
    [InlineData("123456")]   // 6 digits
    [InlineData("ABCDE")]    // letters
    [InlineData("00001")]    // below range
    [InlineData("09999")]    // below range
    [InlineData("99999")]    // above range
    public void Validate_InvalidPostalCode_ReturnsFailure(string? code)
    {
        Pak.PostalCode.Validate(code).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("44000", "Islamabad")]
    [InlineData("75500", "Karachi")]
    [InlineData("54000", "Lahore (extended)")]
    [InlineData("80000", "Quetta")]
    [InlineData("10000", "Peshawar")]
    public void GetRegion_ReturnsCorrectRegion(string code, string expected)
    {
        Pak.PostalCode.GetRegion(code).Should().Be(expected);
    }

    [Fact]
    public void GetRegion_ReturnsNullForInvalid()
    {
        Pak.PostalCode.GetRegion("invalid").Should().BeNull();
    }

    [Fact]
    public void Validate_ExtractsRegionPrefix()
    {
        Pak.PostalCode.Validate("44000")!.Metadata["RegionPrefix"].Should().Be("44");
    }
}

#endregion

#region Landline Tests

public class LandlineValidatorTests
{
    [Theory]
    [InlineData("021-12345678")]       // Karachi (3-digit code, 8-digit subscriber)
    [InlineData("02112345678")]        // Karachi no dash
    [InlineData("051-1234567")]        // Islamabad (3-digit code, 7-digit subscriber)
    [InlineData("0511234567")]         // Islamabad no dash
    [InlineData("042-12345678")]       // Lahore
    [InlineData("+92-21-12345678")]    // International Karachi
    [InlineData("+92511234567")]        // International Islamabad (collapsed)
    [InlineData("92-51-1234567")]       // No + international
    public void Validate_ValidLandline_ReturnsSuccess(string number)
    {
        Pak.Landline.Validate(number).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("abcdefghijk")]
    [InlineData("051-123")]           // subscriber too short
    [InlineData("021-1234567890123")] // subscriber too long
    public void Validate_InvalidLandline_ReturnsFailure(string? number)
    {
        Pak.Landline.Validate(number).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("021-12345678", "Karachi")]
    [InlineData("051-1234567", "Islamabad / Rawalpindi")]
    [InlineData("042-12345678", "Lahore")]
    [InlineData("091-1234567", "Peshawar")]
    [InlineData("081-1234567", "Quetta")]
    public void Validate_DetectsCity(string number, string expectedCity)
    {
        var result = Pak.Landline.Validate(number);
        result.IsValid.Should().BeTrue();
        result.Metadata["City"].Should().Be(expectedCity);
    }

    [Fact]
    public void Validate_GeneratesInternationalFormat()
    {
        var result = Pak.Landline.Validate("051-1234567");
        result.Metadata["InternationalFormat"].Should().Be("+92511234567");
    }
}

#endregion

#region Vehicle Plate Tests

public class VehiclePlateValidatorTests
{
    [Theory]
    [InlineData("LEA-1234")]
    [InlineData("RI-5678")]
    [InlineData("ISB-123")]
    [InlineData("ISB-1234")]
    [InlineData("G-1234")]      // Government
    [InlineData("GS-123")]      // Government Senate
    [InlineData("DN-1234")]     // Diplomatic
    [InlineData("lea-1234")]    // lowercase
    [InlineData("LEA 1234")]    // space separator
    [InlineData("K-1234")]      // Karachi single letter
    public void Validate_ValidPlate_ReturnsSuccess(string plate)
    {
        Pak.VehiclePlate.Validate(plate).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]           // no letters
    [InlineData("ABCDE-123")]      // 5 letters — too many
    [InlineData("LEA-12")]         // only 2 digits (non-government needs 3+)
    [InlineData("LEA-1")]          // only 1 digit
    [InlineData("A-12")]           // non-government with only 2 digits
    [InlineData("LEA-123456")]     // 6 digits — too many
    public void Validate_InvalidPlate_ReturnsFailure(string? plate)
    {
        Pak.VehiclePlate.Validate(plate).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_GovernmentPlateAllowsTwoDigits()
    {
        // Government plates allow minimum 2 digits
        Pak.VehiclePlate.Validate("G-12").IsValid.Should().BeTrue();
        Pak.VehiclePlate.Validate("GS-99").IsValid.Should().BeTrue();
        Pak.VehiclePlate.Validate("DN-50").IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("LEA-1234", "Lahore")]
    [InlineData("RI-5678", "Rawalpindi")]
    [InlineData("ISB-123", "Islamabad")]
    [InlineData("K-1234", "Karachi")]
    [InlineData("G-1234", "Government (Federal)")]
    public void Validate_DetectsRegistrationCity(string plate, string expectedCity)
    {
        var result = Pak.VehiclePlate.Validate(plate);
        result.IsValid.Should().BeTrue();
        result.Metadata["RegistrationCity"].Should().Be(expectedCity);
    }

    [Fact]
    public void Validate_DetectsGovernmentType()
    {
        var result = Pak.VehiclePlate.Validate("G-1234");
        result.Metadata["Type"].Should().Be("Government/Diplomatic");
    }

    [Fact]
    public void Validate_FormatsCorrectly()
    {
        Pak.VehiclePlate.Validate("lea 1234")!.Metadata["Formatted"].Should().Be("LEA-1234");
    }
}

#endregion

#region STRN Tests

public class StrnValidatorTests
{
    [Theory]
    [InlineData("1312345678901")]
    [InlineData("0612345678901")]
    [InlineData("0112345678901")]
    [InlineData("  1312345678901  ")] // whitespace
    [InlineData("13-1234-5678-901")] // with dashes
    public void Validate_ValidStrn_ReturnsSuccess(string strn)
    {
        Pak.Strn.Validate(strn).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("12345678901234")] // 14 digits
    [InlineData("1111111111111")]  // all same
    [InlineData("0000000000000")]  // all zeros
    [InlineData("abcdefghijklm")]
    public void Validate_InvalidStrn_ReturnsFailure(string? strn)
    {
        Pak.Strn.Validate(strn).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("1312345678901", "RTO Islamabad")]
    [InlineData("0612345678901", "RTO Lahore (Zone-I)")]
    [InlineData("0112345678901", "RTO Karachi (Zone-I)")]
    [InlineData("1512345678901", "RTO Quetta")]
    public void Validate_DetectsJurisdiction(string strn, string expectedJurisdiction)
    {
        var result = Pak.Strn.Validate(strn);
        result.IsValid.Should().BeTrue();
        result.Metadata["Jurisdiction"].Should().Be(expectedJurisdiction);
    }

    [Fact]
    public void Format_WorksCorrectly()
    {
        Pak.Strn.Format("1312345678901").Should().Be("13-1234-5678-901");
        Pak.Strn.Format("invalid").Should().BeNull();
    }
}

#endregion

#region ValidationResult Tests

public class ValidationResultTests
{
    [Fact]
    public void Success_ContainsExpectedData()
    {
        var result = ValidationResult.Success("sanitized", new Dictionary<string, string> { ["key"] = "value" });
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Sanitized.Should().Be("sanitized");
        result.Metadata["key"].Should().Be("value");
    }

    [Fact]
    public void Failure_ContainsExpectedData()
    {
        var result = ValidationResult.Failure("something went wrong");
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("something went wrong");
        result.Sanitized.Should().BeNull();
        result.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void ToString_ReturnsReadableString()
    {
        var success = ValidationResult.Success("12345");
        var failure = ValidationResult.Failure("bad input");

        success.ToString().Should().Contain("Valid");
        failure.ToString().Should().Contain("Invalid");
        failure.ToString().Should().Contain("bad input");
    }

    [Fact]
    public void ImplicitBool_WorksInConditions()
    {
        ValidationResult success = ValidationResult.Success("ok");
        ValidationResult failure = ValidationResult.Failure("fail");

        bool successBool = success;
        bool failureBool = failure;

        successBool.Should().BeTrue();
        failureBool.Should().BeFalse();
    }
}

#endregion

#region Cross-Validator Edge Cases

public class CrossValidatorEdgeCaseTests
{
    [Fact]
    public void AllValidators_HandleNullGracefully()
    {
        Pak.Cnic.Validate(null).IsValid.Should().BeFalse();
        Pak.Mobile.Validate(null).IsValid.Should().BeFalse();
        Pak.Ntn.Validate(null).IsValid.Should().BeFalse();
        Pak.Iban.Validate(null).IsValid.Should().BeFalse();
        Pak.PostalCode.Validate(null).IsValid.Should().BeFalse();
        Pak.Landline.Validate(null).IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate(null).IsValid.Should().BeFalse();
        Pak.Strn.Validate(null).IsValid.Should().BeFalse();
    }

    [Fact]
    public void AllValidators_HandleEmptyStringGracefully()
    {
        Pak.Cnic.Validate("").IsValid.Should().BeFalse();
        Pak.Mobile.Validate("").IsValid.Should().BeFalse();
        Pak.Ntn.Validate("").IsValid.Should().BeFalse();
        Pak.Iban.Validate("").IsValid.Should().BeFalse();
        Pak.PostalCode.Validate("").IsValid.Should().BeFalse();
        Pak.Landline.Validate("").IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate("").IsValid.Should().BeFalse();
        Pak.Strn.Validate("").IsValid.Should().BeFalse();
    }

    [Fact]
    public void AllValidators_HandleWhitespaceGracefully()
    {
        Pak.Cnic.Validate("   ").IsValid.Should().BeFalse();
        Pak.Mobile.Validate("   ").IsValid.Should().BeFalse();
        Pak.Ntn.Validate("   ").IsValid.Should().BeFalse();
        Pak.Iban.Validate("   ").IsValid.Should().BeFalse();
        Pak.PostalCode.Validate("   ").IsValid.Should().BeFalse();
        Pak.Landline.Validate("   ").IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate("   ").IsValid.Should().BeFalse();
        Pak.Strn.Validate("   ").IsValid.Should().BeFalse();
    }

    [Fact]
    public void AllIsValid_HandleNullGracefully()
    {
        Pak.Cnic.IsValid(null).Should().BeFalse();
        Pak.Mobile.IsValid(null).Should().BeFalse();
        Pak.Ntn.IsValid(null).Should().BeFalse();
        Pak.Iban.IsValid(null).Should().BeFalse();
        Pak.PostalCode.IsValid(null).Should().BeFalse();
        Pak.Landline.IsValid(null).Should().BeFalse();
        Pak.VehiclePlate.IsValid(null).Should().BeFalse();
        Pak.Strn.IsValid(null).Should().BeFalse();
    }

    [Fact]
    public void Validators_HandleUnicodeInput()
    {
        // Urdu digits or random unicode should fail gracefully
        Pak.Cnic.Validate("٣٥٢٠٢-١٢٣٤٥٦٧-١").IsValid.Should().BeFalse();
        Pak.Mobile.Validate("۰۳۰۰۱۲۳۴۵۶۷").IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("35202-1234567-1\t")]     // tab
    [InlineData("35202-1234567-1\n")]     // newline
    [InlineData("35202-1234567-1\r\n")]   // CRLF
    public void Cnic_HandlesTrailingControlCharacters(string input)
    {
        // Should still be treated as valid after trimming
        // Note: Trim() handles \t, \n, \r
        Pak.Cnic.Validate(input).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Cnic_LeadingZero_NoProvinceMatch()
    {
        // Starts with 0 — no province match, but format is valid
        var result = Pak.Cnic.Validate("01234-5678901-2");
        result.IsValid.Should().BeTrue();
        result.Metadata.Should().NotContainKey("Province");
    }

    [Theory]
    [InlineData("     ")]         // multiple spaces
    [InlineData("---")]           // only dashes
    public void Cnic_RejectsNonDigitStrings(string input)
    {
        Pak.Cnic.Validate(input).IsValid.Should().BeFalse();
    }
}

#endregion

#region Additional Edge Case Tests

public class AdditionalEdgeCaseTests
{
    [Theory]
    [InlineData("Pk36ScBl0000001123456702")]  // mixed case IBAN
    public void Iban_AcceptsMixedCase(string iban)
    {
        Pak.Iban.Validate(iban).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Landline_FourDigitAreaCode_Abbottabad()
    {
        var result = Pak.Landline.Validate("0992-1234567");
        result.IsValid.Should().BeTrue();
        result.Metadata["City"].Should().Be("Abbottabad");
        result.Metadata["AreaCode"].Should().Be("0992");
    }

    [Fact]
    public void VehiclePlate_UnitedNations_Diplomatic()
    {
        var result = Pak.VehiclePlate.Validate("UN-123");
        result.IsValid.Should().BeTrue();
        result.Metadata["RegistrationCity"].Should().Be("United Nations");
        result.Metadata["Type"].Should().Be("Government/Diplomatic");
    }

    [Fact]
    public void Strn_LeadingZeroRegion()
    {
        // RTO Karachi Zone-I has prefix 01
        var result = Pak.Strn.Validate("0112345678901");
        result.IsValid.Should().BeTrue();
        result.Metadata["Jurisdiction"].Should().Be("RTO Karachi (Zone-I)");
    }

    [Theory]
    [InlineData("          ")]   // very long whitespace
    public void AllValidators_RejectLongWhitespace(string input)
    {
        Pak.Cnic.Validate(input).IsValid.Should().BeFalse();
        Pak.Mobile.Validate(input).IsValid.Should().BeFalse();
        Pak.Ntn.Validate(input).IsValid.Should().BeFalse();
        Pak.Iban.Validate(input).IsValid.Should().BeFalse();
        Pak.PostalCode.Validate(input).IsValid.Should().BeFalse();
        Pak.Landline.Validate(input).IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate(input).IsValid.Should().BeFalse();
        Pak.Strn.Validate(input).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("---")]
    [InlineData("- - -")]
    public void AllValidators_RejectDashOnlyStrings(string input)
    {
        Pak.Cnic.Validate(input).IsValid.Should().BeFalse();
        Pak.Mobile.Validate(input).IsValid.Should().BeFalse();
        Pak.Ntn.Validate(input).IsValid.Should().BeFalse();
        Pak.Iban.Validate(input).IsValid.Should().BeFalse();
        Pak.PostalCode.Validate(input).IsValid.Should().BeFalse();
        Pak.Landline.Validate(input).IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate(input).IsValid.Should().BeFalse();
        Pak.Strn.Validate(input).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validators_HandleExtremelyLongInput()
    {
        var longString = new string('1', 10000);
        Pak.Cnic.Validate(longString).IsValid.Should().BeFalse();
        Pak.Mobile.Validate(longString).IsValid.Should().BeFalse();
        Pak.Ntn.Validate(longString).IsValid.Should().BeFalse();
        Pak.Iban.Validate(longString).IsValid.Should().BeFalse();
        Pak.PostalCode.Validate(longString).IsValid.Should().BeFalse();
        Pak.Landline.Validate(longString).IsValid.Should().BeFalse();
        Pak.VehiclePlate.Validate(longString).IsValid.Should().BeFalse();
        Pak.Strn.Validate(longString).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("0092-300-1234567")]     // 0092 prefix with dashes
    [InlineData("(+92)3001234567")]       // parenthesized country code
    public void Mobile_AcceptsCountryCodeVariations(string mobile)
    {
        Pak.Mobile.Validate(mobile).IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetCity_Landline_ReturnsNullForInvalid()
    {
        Pak.Landline.GetCity("invalid").Should().BeNull();
        Pak.Landline.GetCity(null).Should().BeNull();
    }

    [Fact]
    public void GetCity_VehiclePlate_ReturnsNullForInvalid()
    {
        Pak.VehiclePlate.GetCity("invalid").Should().BeNull();
        Pak.VehiclePlate.GetCity(null).Should().BeNull();
    }
}

#endregion

#region Batch Validation Tests

public class BatchValidationTests
{
    [Fact]
    public void ValidateAll_AllValid_ReturnsSuccess()
    {
        var batch = Pak.ValidateAll(
            ("Cnic", () => Pak.Cnic.Validate("35202-1234567-1")),
            ("Mobile", () => Pak.Mobile.Validate("03001234567")),
            ("Iban", () => Pak.Iban.Validate("PK36SCBL0000001123456702"))
        );

        batch.IsValid.Should().BeTrue();
        batch.Errors.Should().BeEmpty();
        batch.Results.Should().HaveCount(3);
    }

    [Fact]
    public void ValidateAll_SomeInvalid_ReturnsFailure()
    {
        var batch = Pak.ValidateAll(
            ("Cnic", () => Pak.Cnic.Validate("35202-1234567-1")),
            ("Mobile", () => Pak.Mobile.Validate("invalid")),
            ("Iban", () => Pak.Iban.Validate("invalid"))
        );

        batch.IsValid.Should().BeFalse();
        batch.Errors.Should().HaveCount(2);
        batch.Errors.Should().ContainKeys("Mobile", "Iban");
        batch.Results.Should().HaveCount(3);
    }

    [Fact]
    public void ValidateAll_PopulatesErrorMessages()
    {
        var batch = Pak.ValidateAll(
            ("Cnic", () => Pak.Cnic.Validate(null)),
            ("Mobile", () => Pak.Mobile.Validate(""))
        );

        batch.Errors["Cnic"].Should().Contain("required");
        batch.Errors["Mobile"].Should().Contain("required");
    }

    [Fact]
    public void ValidateAll_ImplicitBoolConversion_Works()
    {
        var validBatch = Pak.ValidateAll(
            ("Cnic", () => Pak.Cnic.Validate("35202-1234567-1"))
        );
        var invalidBatch = Pak.ValidateAll(
            ("Cnic", () => Pak.Cnic.Validate("invalid"))
        );

        (validBatch ? true : false).Should().BeTrue();
        (invalidBatch ? true : false).Should().BeFalse();
    }

    [Fact]
    public void ValidateAll_Empty_ReturnsSuccess()
    {
        var batch = Pak.ValidateAll();
        batch.IsValid.Should().BeTrue();
        batch.Errors.Should().BeEmpty();
    }
}

#endregion

#region Data Annotations Tests

public class DataAnnotationsTests
{
    [Theory]
    [InlineData("35202-1234567-1", true)]
    [InlineData("3520212345671", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)] // Null is allowed; use [Required] separately
    public void PakCnicAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakCnicAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("03001234567", true)]
    [InlineData("0300-1234567", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakMobileAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakMobileAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("1234567-8", true)]
    [InlineData("35202-1234567-1", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakNtnAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakNtnAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("PK36SCBL0000001123456702", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakIbanAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakIbanAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("44000", true)]
    [InlineData("75500", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakPostalCodeAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakPostalCodeAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("051-1234567", true)]
    [InlineData("021-12345678", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakLandlineAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakLandlineAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("LEA-1234", true)]
    [InlineData("ISB-123", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakVehiclePlateAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakVehiclePlateAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("1312345678901", true)]
    [InlineData("invalid", false)]
    [InlineData(null, true)]
    public void PakStrnAttribute_Validates(string? value, bool expectedValid)
    {
        var attr = new PakValidate.DataAnnotations.PakStrnAttribute();
        attr.IsValid(value).Should().Be(expectedValid);
    }

    [Fact]
    public void DataAnnotationsAttributes_RejectNonStringValues()
    {
        var attr = new PakValidate.DataAnnotations.PakCnicAttribute();
        attr.IsValid(12345).Should().BeFalse();
        attr.IsValid(true).Should().BeFalse();
    }
}

#endregion

#region Error Handling Extensions Tests

public class ValidationResultErrorHandlingTests
{
    [Fact]
    public void ThrowIfInvalid_ValidResult_DoesNotThrow()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);

        Action act = () => result.ThrowIfInvalid();
        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfInvalid_InvalidResult_ThrowsValidationException()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);

        Action act = () => result.ThrowIfInvalid();
        act.Should().Throw<ValidationException>()
            .WithMessage("*13 digits*");
    }

    [Fact]
    public void IsInvalid_ValidResult_ReturnsFalse()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);

        result.IsInvalid().Should().BeFalse();
    }

    [Fact]
    public void IsInvalid_InvalidResult_ReturnsTrue()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);

        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void GetErrorOrDefault_ValidResult_ReturnsDefaultMessage()
    {
        const string validCnic = "35202-1234567-1";
        const string defaultMessage = "No errors";
        var result = Pak.Cnic.Validate(validCnic);

        var error = result.GetErrorOrDefault(defaultMessage);
        error.Should().Be(defaultMessage);
    }

    [Fact]
    public void GetErrorOrDefault_InvalidResult_ReturnsActualErrorMessage()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);

        var error = result.GetErrorOrDefault("Default");
        error.Should().Contain("13 digits");
    }

    [Fact]
    public void GetErrorOrDefault_WithoutParameter_UsesDefaultValue()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);

        var error = result.GetErrorOrDefault();
        error.Should().Be("Invalid");
    }
}

#endregion

#region Result Mapping Extensions Tests

public class ValidationResultMappingTests
{
    [Fact]
    public void Map_ValidResult_ExecutesOnSuccessFunction()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);

        var mapped = result.Map(
            r => r.Gender(),
            _ => null
        );

        mapped.Should().Be("Male");
    }

    [Fact]
    public void Map_InvalidResult_ExecutesOnFailureFunction()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);
        const string errorMessage = "Custom error";

        var mapped = result.Map(
            _ => "Success",
            error => errorMessage
        );

        mapped.Should().Be(errorMessage);
    }

    [Fact]
    public void Map_WithComplexReturn_ReturnsCorrectly()
    {
        const string validMobile = "03001234567";
        var result = Pak.Mobile.Validate(validMobile);

        var mobileData = result.Map(
            r => new
            {
                Carrier = r.Carrier(),
                Format = r.LocalFormat()
            },
            error => new { Carrier = (string?)null, Format = (string?)null }
        );

        mobileData.Carrier.Should().Be("Jazz");
        mobileData.Format.Should().Be("03001234567");
    }

    [Fact]
    public void Match_ValidResult_ExecutesOnSuccessAction()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);
        bool actionExecuted = false;

        result.Match(
            _ => actionExecuted = true,
            _ => { }
        );

        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public void Match_InvalidResult_ExecutesOnFailureAction()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);
        bool actionExecuted = false;

        result.Match(
            _ => { },
            _ => actionExecuted = true
        );

        actionExecuted.Should().BeTrue();
    }

    [Fact]
    public void Match_CanAccessResultMetadata()
    {
        const string validCnic = "35202-1234567-1";
        var result = Pak.Cnic.Validate(validCnic);
        string? capturedGender = null;

        result.Match(
            r => capturedGender = r.Gender(),
            _ => { }
        );

        capturedGender.Should().Be("Male");
    }

    [Fact]
    public void Match_CanAccessErrorMessage()
    {
        const string invalidCnic = "invalid";
        var result = Pak.Cnic.Validate(invalidCnic);
        string? capturedError = null;

        result.Match(
            _ => { },
            error => capturedError = error
        );

        capturedError.Should().Contain("13 digits");
    }
}

#endregion

#region Batch Validation Extensions Tests

public class BatchValidationResultExtensionsTests
{
    [Fact]
    public void GetError_ExistingError_ReturnsErrorMessage()
    {
        const string invalidCnic = "invalid";
        const string validMobile = "03001234567";
        const string cnicFieldName = "Cnic";
        const string mobileFieldName = "Mobile";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(invalidCnic)),
            (mobileFieldName, () => Pak.Mobile.Validate(validMobile))
        );

        var error = batch.GetError(cnicFieldName);
        error.Should().NotBeNullOrEmpty();
        error.Should().Contain("13 digits");
    }

    [Fact]
    public void GetError_NoError_ReturnsNull()
    {
        const string validCnic = "35202-1234567-1";
        const string validMobile = "03001234567";
        const string cnicFieldName = "Cnic";
        const string mobileFieldName = "Mobile";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(validCnic)),
            (mobileFieldName, () => Pak.Mobile.Validate(validMobile))
        );

        var error = batch.GetError(cnicFieldName);
        error.Should().BeNull();
    }

    [Fact]
    public void GetErrors_MultipleErrors_ReturnsAllAsEnumerable()
    {
        const string invalidCnic = "invalid";
        const string invalidMobile = "bad";
        const string cnicFieldName = "Cnic";
        const string mobileFieldName = "Mobile";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(invalidCnic)),
            (mobileFieldName, () => Pak.Mobile.Validate(invalidMobile))
        );

        var errors = batch.GetErrors().ToList();
        errors.Should().HaveCount(2);
        errors.Should().Contain(e => e.Field == cnicFieldName);
        errors.Should().Contain(e => e.Field == mobileFieldName);
    }

    [Fact]
    public void GetErrors_PartialErrors_ReturnsOnlyFailedFields()
    {
        const string validCnic = "35202-1234567-1";
        const string invalidMobile = "bad";
        const string cnicFieldName = "Cnic";
        const string mobileFieldName = "Mobile";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(validCnic)),
            (mobileFieldName, () => Pak.Mobile.Validate(invalidMobile))
        );

        var errors = batch.GetErrors().ToList();
        errors.Should().HaveCount(1);
        errors.Should().AllSatisfy(e => e.Field.Should().Be(mobileFieldName));
    }

    [Fact]
    public void GetErrors_EnablesTupleDeconstruction()
    {
        const string invalidCnic = "invalid";
        const string cnicFieldName = "Cnic";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(invalidCnic))
        );

        var errorList = new List<string>();
        foreach (var (field, error) in batch.GetErrors())
        {
            errorList.Add($"{field}: {error}");
        }

        errorList.Should().HaveCount(1);
        errorList[0].Should().Contain(cnicFieldName);
    }

    [Fact]
    public void ThrowIfInvalid_ValidBatch_DoesNotThrow()
    {
        const string validCnic = "35202-1234567-1";
        const string validMobile = "03001234567";
        const string cnicFieldName = "Cnic";
        const string mobileFieldName = "Mobile";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(validCnic)),
            (mobileFieldName, () => Pak.Mobile.Validate(validMobile))
        );

        Action act = () => batch.ThrowIfInvalid();
        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfInvalid_InvalidBatch_ThrowsValidationException()
    {
        const string invalidCnic = "invalid";
        const string cnicFieldName = "Cnic";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(invalidCnic))
        );

        Action act = () => batch.ThrowIfInvalid();
        act.Should().Throw<ValidationException>()
            .WithMessage("*Cnic*");
    }

    [Fact]
    public void IsInvalid_ValidBatch_ReturnsFalse()
    {
        const string validCnic = "35202-1234567-1";
        const string cnicFieldName = "Cnic";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(validCnic))
        );

        batch.IsInvalid().Should().BeFalse();
    }

    [Fact]
    public void IsInvalid_InvalidBatch_ReturnsTrue()
    {
        const string invalidCnic = "invalid";
        const string cnicFieldName = "Cnic";

        var batch = Pak.ValidateAll(
            (cnicFieldName, () => Pak.Cnic.Validate(invalidCnic))
        );

        batch.IsInvalid().Should().BeTrue();
    }
}

#endregion
