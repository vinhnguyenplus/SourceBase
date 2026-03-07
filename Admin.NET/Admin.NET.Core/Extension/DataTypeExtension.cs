// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Basic data type extension (as a supplement to NewLife.Core's Utility)
/// </summary>
public static class DataTypeExtension
{
    /// <summary>Convert to SByte integer, return the default value if the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static sbyte ToSByte(this object value, sbyte defaultValue = default)
    {
        if (value is sbyte num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (sbyte.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }

    /// <summary>Convert to Byte integer, return the default value if the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static byte ToByte(this object value, byte defaultValue = default)
    {
        if (value is byte num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (byte.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }

    /// <summary>Convert to Int16 integer, return the default value if the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static short ToInt16(this object value, short defaultValue = default)
    {
        if (value is short num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (short.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }

    /// <summary>Convert to UInt16 integer, return the default value when the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static ushort ToUInt16(this object value, ushort defaultValue = default)
    {
        if (value is ushort num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (ushort.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }

    /// <summary>Convert to UInt32 integer, return the default value if the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static uint ToUInt32(this object value, uint defaultValue = default)
    {
        if (value is uint num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (uint.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }

    /// <summary>Convert to UInt64 integer, return the default value if the conversion fails.</summary>
    /// <remarks></remarks>
    /// <param name="value">Object to be converted</param>
    /// <param name="defaultValue">default value. Used when the object to be converted is invalid</param>
    /// <returns></returns>
    public static ulong ToUInt64(this object value, ulong defaultValue = default)
    {
        if (value is ulong num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        if (ulong.TryParse(value.ToString(), out var result))
            return result;
        else
            return defaultValue;
    }
}