// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Baidu translation results
/// </summary>
public class BaiDuTranslationResult
{
    /// <summary>
    /// Source language
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// target language
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// Translation results
    /// </summary>
    public List<TransResult> trans_result { get; set; }

    /// <summary>
    /// Error code Normally 0
    /// </summary>
    public string error_code { get; set; } = "0";

    /// <summary>
    /// error message
    /// </summary>
    public string error_msg { get; set; } = String.Empty;
}

/// <summary>
/// Translation results
/// </summary>
public class TransResult
{
    /// <summary>
    /// source character
    /// </summary>
    public string Src { get; set; }

    /// <summary>
    /// target character
    /// </summary>
    public string Dst { get; set; } = string.Empty;
}