// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Data desensitization feature (supports custom desensitization positions and desensitization characters)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DataMaskAttribute : Attribute
{
    /// <summary>
    /// Desensitization starting position (starting from 0)
    /// </summary>
    private int StartIndex { get; }

    /// <summary>
    /// Desensitization length
    /// </summary>
    private int Length { get; }

    /// <summary>
    /// Desensitizing characters (default *)
    /// </summary>
    private char MaskChar { get; set; } = '*';

    /// <summary>
    /// Whether to retain the original length (default true)
    /// </summary>
    private bool KeepLength { get; set; } = true;

    public DataMaskAttribute(int startIndex, int length)
    {
        if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        StartIndex = startIndex;
        Length = length;
    }

    /// <summary>
    /// Perform desensitization
    /// </summary>
    public string Mask(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= StartIndex)
            return input;

        var maskedLength = Math.Min(Length, input.Length - StartIndex);
        var maskStr = new string(MaskChar, KeepLength ? maskedLength : Math.Min(4, maskedLength));

        return input.Substring(0, StartIndex) + maskStr +
               (StartIndex + maskedLength < input.Length ?
                   input.Substring(StartIndex + maskedLength) : "");
    }
}