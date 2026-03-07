// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Language output parameters
/// </summary>
public class SysLangDto
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Language name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    public string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    public string UrlCode { get; set; }

    /// <summary>
    /// writing direction
    /// </summary>
    public DirectionEnum Direction { get; set; }

    /// <summary>
    /// date format
    /// </summary>
    public string DateFormat { get; set; }

    /// <summary>
    /// time format
    /// </summary>
    public string TimeFormat { get; set; }

    /// <summary>
    /// start day of week
    /// </summary>
    public WeekEnum WeekStart { get; set; }

    /// <summary>
    /// Grouping symbols
    /// </summary>
    public string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    public string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    public string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    public string? CreateUserName { get; set; }

    /// <summary>
    /// Modifier ID
    /// </summary>
    public long? UpdateUserId { get; set; }

    /// <summary>
    /// Modifier name
    /// </summary>
    public string? UpdateUserName { get; set; }
}