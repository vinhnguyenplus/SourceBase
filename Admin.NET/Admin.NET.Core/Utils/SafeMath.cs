// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.Globalization;

namespace Admin.NET.Core;

using System;

/// <summary>
/// Safe basic mathematical operation method class
/// </summary>
public static class SafeMath
{
    /// <summary>
    /// safe addition
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <param name="precision">Keep decimal places</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="throwOnError">Whether to throw an exception</param>
    /// <returns></returns>
    public static T Add<T>(object left, object right, int precision = 2, T defaultValue = default, bool throwOnError = true) where T : struct, IComparable, IConvertible, IFormattable
    {
        return PerformOperation(left, right, (a, b) => a + b, precision, defaultValue, throwOnError);
    }

    /// <summary>
    /// safe subtraction
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <param name="precision">Keep decimal places</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="throwOnError">Whether to throw an exception</param>
    public static T Sub<T>(object left, object right, int precision = 2, T defaultValue = default, bool throwOnError = true) where T : struct, IComparable, IConvertible, IFormattable
    {
        return PerformOperation(left, right, (a, b) => a - b, precision, defaultValue, throwOnError);
    }

    /// <summary>
    /// safe multiplication
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <param name="precision">Keep decimal places</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="throwOnError">Whether to throw an exception</param>
    public static T Mult<T>(object left, object right, int precision = 2, T defaultValue = default, bool throwOnError = true) where T : struct, IComparable, IConvertible, IFormattable
    {
        return PerformOperation(left, right, (a, b) => a * b, precision, defaultValue, throwOnError);
    }

    /// <summary>
    /// safe division
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <param name="precision">Keep decimal places</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="throwOnDivideByZero">Whether to throw divide by zero exception</param>
    public static T Div<T>(object left, object right, int precision = 2, T defaultValue = default, bool throwOnDivideByZero = true) where T : struct, IComparable, IConvertible, IFormattable
    {
        return PerformOperation(left, right, (a, b) =>
        {
            if (b != 0) return a / b;
            if (throwOnDivideByZero) throw new DivideByZeroException("The divisor cannot be 0");
            return SafeConvert<decimal>(defaultValue);
        }, precision, defaultValue, throwOnDivideByZero);
    }

    /// <summary>
    /// safe type conversion
    /// </summary>
    /// <param name="value">data source</param>
    /// <param name="defaultValue">default value</param>
    public static T SafeConvert<T>(object value, T defaultValue = default) where T : struct, IComparable, IConvertible, IFormattable
    {
        if (value == null) return defaultValue;
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// perform mathematical operations
    /// </summary>
    private static T PerformOperation<T>(object left, object right, Func<decimal, decimal, decimal> operation, int precision, T defaultValue, bool throwOnError) where T : struct, IComparable, IConvertible, IFormattable
    {
        try
        {
            decimal leftValue = ConvertToDecimal(left);
            decimal rightValue = ConvertToDecimal(right);

            decimal result = operation(leftValue, rightValue);
            return SafeConvert(Math.Round(result, precision, MidpointRounding.AwayFromZero), defaultValue);
        }
        catch
        {
            if (throwOnError) throw;
            return defaultValue;
        }
    }

    /// <summary>
    /// Convert input value to decimal
    /// </summary>
    public static decimal ConvertToDecimal(object value)
    {
        return value switch
        {
            null => 0m,
            int intValue => intValue,
            float floatValue => (decimal)floatValue,
            double doubleValue => (decimal)doubleValue,
            decimal decimalValue => decimalValue,
            long longValue => longValue,
            short shortValue => shortValue,
            byte byteValue => byteValue,
            string stringValue when decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue) => parsedValue, // Try to parse the string
            _ => throw new InvalidCastException($"Unsupported type: {value.GetType().Name}")
        };
    }
}