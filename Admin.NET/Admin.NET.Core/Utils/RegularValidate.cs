// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Regular check
/// </summary>
public static class RegularValidate
{
    /// <summary>
    /// Verify password rules
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool ValidatePassword(string password)
    {
        var regex = new Regex(@"
(?=.*[0-9])                     #MustincludeNumber
(?=.*[a-z])                     #Mustincludelowercase
(?=.*[A-Z])                     #MustincludeUppercase
(?=([\x21-\x7e]+)[^a-zA-Z0-9])  #MustincludeSpecial characters
.{8,30}                         #tofew8character，at most30character
", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        //If the requirement must contain lowercase and uppercase letters, the above (?=.*[a-zA-Z]) should be changed to:
        /*
         * (?=.*[a-z])
         * (?=.*[A-Z])
         */
        return regex.IsMatch(password);
    }
}