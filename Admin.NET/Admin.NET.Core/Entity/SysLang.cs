// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

[SugarTable(null, "Language Settings")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
public class SysLang : EntityBase
{
    /// <summary>
    /// Language name
    /// </summary>
    [SugarColumn(ColumnDescription = "Language name")]
    public string Name { get; set; }

    /// <summary>
    /// Language code (such as zh-CN)
    /// </summary>
    [SugarColumn(ColumnDescription = "Language code")]
    public string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    [SugarColumn(ColumnDescription = "ISO language code")]
    public string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    [SugarColumn(ColumnDescription = "URL language code")]
    public string UrlCode { get; set; }

    /// <summary>
    /// Writing direction (1=left to right, 2=right to left)
    /// </summary>
    [SugarColumn(ColumnDescription = "Writing direction", DefaultValue = "1")]
    public DirectionEnum Direction { get; set; } = DirectionEnum.Ltr;

    /// <summary>
    /// Date format (such as YYYY-MM-DD)
    /// </summary>
    [SugarColumn(ColumnDescription = "date format")]
    public string DateFormat { get; set; }

    /// <summary>
    /// Time format (such as HH:MM:SS)
    /// </summary>
    [SugarColumn(ColumnDescription = "time format")]
    public string TimeFormat { get; set; }

    /// <summary>
    /// The starting day of the week (e.g. 0=Sunday, 1=Monday)
    /// </summary>
    [SugarColumn(ColumnDescription = "Start day of the week", DefaultValue = "7")]
    public WeekEnum WeekStart { get; set; } = WeekEnum.Sunday;

    /// <summary>
    /// Grouping symbols (such as ,)
    /// </summary>
    [SugarColumn(ColumnDescription = "Grouping symbols")]
    public string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    [SugarColumn(ColumnDescription = "Decimal point symbol")]
    public string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    [SugarColumn(ColumnDescription = "thousands separator")]
    public string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    [SugarColumn(ColumnDescription = "Enable or not")]
    public bool Active { get; set; }
}