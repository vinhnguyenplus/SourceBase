// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// String extension methods
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// String truncation
    /// </summary>
    public static string Truncate(this string str, int maxLength, string ellipsis = "...")
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        if (maxLength <= 0) return string.Empty;
        if (str.Length <= maxLength) return str;

        // Ensure that ellipsis does not cause the string to exceed the maximum length
        int ellipsisLength = ellipsis?.Length ?? 0;
        int truncateLength = Math.Min(maxLength, str.Length - ellipsisLength);
        return str[..truncateLength] + ellipsis;
    }

    /// <summary>
    /// All capital letters of words
    /// </summary>
    public static string ToTitleCase(this string str)
    {
        return string.IsNullOrWhiteSpace(str) ? str : System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    /// <summary>
    /// Check if a substring is contained, ignoring case
    /// </summary>
    public static bool ContainsIgnoreCase(this string str, string substring)
    {
        if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(substring)) return false;
        return str.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determine whether it is JSON data
    /// </summary>
    public static bool IsJson(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        str = str.Trim();
        return (str.StartsWith("{") && str.EndsWith("}")) || (str.StartsWith("[") && str.EndsWith("]"));
    }

    /// <summary>
    /// Determine whether it is HTML data
    /// </summary>
    public static bool IsHtml(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        str = str.Trim();

        // Check if it starts with <!DOCTYPE html> or <html>
        if (str.StartsWith("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase) || str.StartsWith("<html>", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check if HTML tags are included
        return Regex.IsMatch(str, @"<\s*[^>]+>.*<\s*/\s*[^>]+>|<\s*[^>]+\s*/>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// String reverse
    /// </summary>
    public static string Reverse(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;

        // Using Span<char> to improve performance
        Span<char> charSpan = stackalloc char[str.Length];
        for (int i = 0; i < str.Length; i++)
        {
            charSpan[str.Length - 1 - i] = str[i];
        }
        return new string(charSpan);
    }

    /// <summary>
    /// Convert first letter to lower case
    /// </summary>
    public static string ToFirstLetterLowerCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        if (input.Length == 1) return input.ToLower(); // Handle single character strings

        return char.ToLower(input[0]) + input[1..];
    }

    /// <summary>
    /// Render a string, replacing placeholders
    /// </summary>
    /// <param name="template">Template content</param>
    /// <param name="parameters">parameter object</param>
    /// <returns></returns>
    public static string Render(this string template, object parameters)
    {
        if (string.IsNullOrWhiteSpace(template)) return template;

        // Convert arguments to dictionary (ignoring case)
        var paramDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                paramDict[prop.Name] = prop.GetValue(parameters)?.ToString() ?? string.Empty;
            }
        }

        // Replace placeholders using regular expressions
        return Regex.Replace(template, @"\{(\w+)\}", match =>
        {
            string key = match.Groups[1].Value; // Get the key in the placeholder
            return paramDict.TryGetValue(key, out string value) ? value : string.Empty;
        });
    }

    /// <summary>
    /// camelback to underline
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isToUpper"></param>
    /// <returns></returns>
    public static string ToUnderLine(this string str, bool isToUpper = false)
    {
        if (string.IsNullOrEmpty(str) || str.Contains("_"))
        {
            return str;
        }

        int length = str.Length;
        var result = new System.Text.StringBuilder(length + (length / 3));

        result.Append(char.ToLowerInvariant(str[0]));

        int lastIndex = length - 1;

        for (int i = 1; i < length; i++)
        {
            char current = str[i];
            if (!char.IsUpper(current))
            {
                result.Append(current);
                continue;
            }

            bool prevIsLower = char.IsLower(str[i - 1]);
            bool nextIsLower = (i < lastIndex) && char.IsLower(str[i + 1]);

            if (prevIsLower || nextIsLower)
            {
                result.Append('_');
            }

            result.Append((char)(current | 0x20));
        }

        string converted = result.ToString();
        return isToUpper ? converted.ToUpperInvariant() : converted;
    }
}