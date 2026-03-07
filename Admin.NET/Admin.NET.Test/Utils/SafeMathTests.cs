// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;
using Xunit;

namespace Admin.NET.Test.Utils;

public class SafeMathTests
{
    [Fact]
    public void Add_IntAndDouble_ReturnsCorrectResult()
    {
        // Arrange
        int left = 10;
        double right = 20.5;

        // Act
        var result = SafeMath.Add<int>(left, right, precision: 2);

        // Assert
        Assert.Equal(30, result); // 10 + 20.5 = 30.5, rounded to 30
    }

    [Fact]
    public void Add_StringAndDecimal_ReturnsCorrectResult()
    {
        // Arrange
        string left = "15.75";
        decimal right = 4.25m;

        // Act
        var result = SafeMath.Add<decimal>(left, right, precision: 2);

        // Assert
        Assert.Equal(20.00m, result); // 15.75 + 4.25 = 20.00
    }

    [Fact]
    public void Sub_DoubleAndInt_ReturnsCorrectResult()
    {
        // Arrange
        double left = 50.75;
        int right = 25;

        // Act
        var result = SafeMath.Sub<double>(left, right, precision: 2);

        // Assert
        Assert.Equal(25.75, result); // 50.75 - 25 = 25.75
    }

    [Fact]
    public void Mult_DecimalAndFloat_ReturnsCorrectResult()
    {
        // Arrange
        decimal left = 10.5m;
        float right = 2.0f;

        // Act
        var result = SafeMath.Mult<decimal>(left, right, precision: 2);

        // Assert
        Assert.Equal(21.00m, result); // 10.5 * 2.0 = 21.00
    }

    [Fact]
    public void Div_IntAndInt_ReturnsCorrectResult()
    {
        // Arrange
        int left = 10;
        int right = 3;

        // Act
        var result = SafeMath.Div<double>(left, right, precision: 4);

        // Assert
        Assert.Equal(3.3333, result); // 10 / 3 = 3.3333
    }

    [Fact]
    public void Div_ByZero_ReturnsDefaultValue()
    {
        // Arrange
        int left = 10;
        int right = 0;

        // Act
        int result = SafeMath.Div(left, right, defaultValue: -1, throwOnDivideByZero: false);

        // Assert
        Assert.Equal(-1, result); // Divisor is 0, returns the default value -1
    }

    [Fact]
    public void Div_ByZero_ThrowsException()
    {
        // Arrange
        int left = 10;
        int right = 0;

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() =>
        {
            SafeMath.Div<double>(left, right, throwOnDivideByZero: true);
        });
    }

    [Fact]
    public void SafeConvert_StringToInt_ReturnsCorrectResult()
    {
        // Arrange
        string value = "42";

        // Act
        int result = SafeMath.SafeConvert(value, defaultValue: -1);

        // Assert
        Assert.Equal(42, result); // String "42" converted to int 42
    }

    [Fact]
    public void SafeConvert_InvalidString_ReturnsDefaultValue()
    {
        // Arrange
        string value = "invalid";

        // Act
        int result = SafeMath.SafeConvert(value, defaultValue: -1);

        // Assert
        Assert.Equal(-1, result); // Conversion failed, returning default value -1
    }

    [Fact]
    public void ConvertToDecimal_Int_ReturnsCorrectResult()
    {
        // Arrange
        int value = 42;

        // Act
        decimal result = SafeMath.ConvertToDecimal(value);

        // Assert
        Assert.Equal(42m, result); // Convert int 42 to decimal 42m
    }

    [Fact]
    public void ConvertToDecimal_String_ReturnsCorrectResult()
    {
        // Arrange
        string value = "42.75";

        // Act
        decimal result = SafeMath.ConvertToDecimal(value);

        // Assert
        Assert.Equal(42.75m, result); // The string "42.75" is converted to decimal 42.75m
    }

    [Fact]
    public void ConvertToDecimal_InvalidString_ReturnsZero()
    {
        // Arrange
        string value = "invalid";

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => SafeMath.ConvertToDecimal(value));
    }

    [Fact]
    public void Add_LeftNull_ReturnsDefaultValue()
    {
        // Arrange
        object left = null;
        int right = 20;

        // Act
        int result = SafeMath.Add<int>(left, right);

        // Assert
        Assert.Equal(20, result); // left operand is null
    }

    [Fact]
    public void Add_RightNull_ReturnsDefaultValue()
    {
        // Arrange
        int left = 10;
        object right = null;

        // Act
        var result = SafeMath.Add<int>(left, right);

        // Assert
        Assert.Equal(10, result); // The right operand is null
    }

    [Fact]
    public void Sub_LeftNull_ReturnsDefaultValue()
    {
        // Arrange
        object left = null;
        int right = 20;

        // Act
        int result = SafeMath.Sub<int>(left, right);

        // Assert
        Assert.Equal(-20, result); // left operand is null
    }

    [Fact]
    public void Sub_RightNull_ReturnsDefaultValue()
    {
        // Arrange
        int left = 10;
        object right = null;

        // Act
        var result = SafeMath.Sub<int>(left, right);

        // Assert
        Assert.Equal(10, result); // The right operand is null
    }

    [Fact]
    public void Mult_LeftNull_ReturnsDefaultValue()
    {
        // Arrange
        object left = null;
        int right = 20;

        // Act
        int result = SafeMath.Mult<int>(left, right);

        // Assert
        Assert.Equal(0, result); // left operand is null
    }

    [Fact]
    public void Mult_RightNull_ReturnsDefaultValue()
    {
        // Arrange
        int left = 10;
        object right = null;

        // Act
        int result = SafeMath.Mult<int>(left, right);

        // Assert
        Assert.Equal(0, result); // The right operand is null
    }

    [Fact]
    public void Div_LeftNull_ReturnsDefaultValue()
    {
        // Arrange
        object left = null;
        int right = 20;

        // Act
        int result = SafeMath.Div<int>(left, right);

        // Assert
        Assert.Equal(0, result); // left operand is null
    }

    [Fact]
    public void Div_RightNull_ReturnsDefaultValue()
    {
        // Arrange
        int left = 10;
        object right = null;

        // Act
        Assert.Throws<DivideByZeroException>(() =>
        {
            int result = SafeMath.Div<int>(left, right);
            // Assert
            Assert.Equal(-1, result); // If the right operand is null, the default value -1 is returned.
        });
    }

    [Fact]
    public void SafeConvert_NullInput_ReturnsDefaultValue()
    {
        // Arrange
        object value = null;

        // Act
        int result = SafeMath.SafeConvert<int>(value, defaultValue: -1);

        // Assert
        Assert.Equal(-1, result); // If the input is null, the default value -1 is returned.
    }

    [Fact]
    public void ConvertToDecimal_NullInput_ReturnsZero()
    {
        // Arrange
        object value = null;

        // Act
        decimal result = SafeMath.ConvertToDecimal(value);

        // Assert
        Assert.Equal(0m, result); // If the input is null, the default value 0m is returned.
    }
}