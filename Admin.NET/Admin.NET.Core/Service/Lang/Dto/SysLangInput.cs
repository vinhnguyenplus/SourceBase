// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Language basic input parameters
/// </summary>
public class SysLangBaseInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public virtual long? Id { get; set; }

    /// <summary>
    /// Language name
    /// </summary>
    [Required(ErrorMessage = "Language name cannot be empty")]
    public virtual string Name { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    public virtual string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    [Required(ErrorMessage = "ISO Language code cannot be empty")]
    public virtual string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    [Required(ErrorMessage = "The URL language code cannot be empty")]
    public virtual string UrlCode { get; set; }

    /// <summary>
    /// writing direction
    /// </summary>
    [Required(ErrorMessage = "Writing direction cannot be empty")]
    public virtual DirectionEnum Direction { get; set; }

    /// <summary>
    /// date format
    /// </summary>
    [Required(ErrorMessage = "Date format cannot be empty")]
    public virtual string DateFormat { get; set; }

    /// <summary>
    /// time format
    /// </summary>
    [Required(ErrorMessage = "time formatcannot benull")]
    public virtual string TimeFormat { get; set; }

    /// <summary>
    /// start day of week
    /// </summary>
    [Required(ErrorMessage = "Weekly start day cannot be empty")]
    public virtual WeekEnum? WeekStart { get; set; }

    /// <summary>
    /// Grouping symbols
    /// </summary>
    [Required(ErrorMessage = "Group symbol cannot be empty")]
    public virtual string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    [Required(ErrorMessage = "The decimal point symbol cannot be empty")]
    public virtual string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    public virtual string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    [Required(ErrorMessage = "Whether to enable cannot be empty")]
    public virtual bool? Active { get; set; }
}

/// <summary>
/// Multi-language paging query input parameters
/// </summary>
public class PageSysLangInput : BasePageInput
{
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
    /// Whether to enable
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Select primary key list
    /// </summary>
    public List<long> SelectKeyList { get; set; }
}

/// <summary>
/// Add input parameters for multiple languages
/// </summary>
public class AddSysLangInput
{
    /// <summary>
    /// Language name
    /// </summary>
    [Required(ErrorMessage = "Language name cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language name character length cannot exceed 255")]
    public string Name { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language code character length cannot exceed 255")]
    public string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    [Required(ErrorMessage = "ISO Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "ISO language code character length cannot exceed 255")]
    public string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    [Required(ErrorMessage = "The URL language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "URL language code character length cannot exceed 255")]
    public string UrlCode { get; set; }

    /// <summary>
    /// writing direction
    /// </summary>
    [Required(ErrorMessage = "Writing direction cannot be empty")]
    public DirectionEnum Direction { get; set; }

    /// <summary>
    /// date format
    /// </summary>
    [Required(ErrorMessage = "Date format cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the date format characters cannot exceed 255")]
    public string DateFormat { get; set; }

    /// <summary>
    /// time format
    /// </summary>
    [Required(ErrorMessage = "time formatcannot benull")]
    [MaxLength(255, ErrorMessage = "The length of time format characters cannot exceed 255")]
    public string TimeFormat { get; set; }

    /// <summary>
    /// start day of week
    /// </summary>
    [Required(ErrorMessage = "Weekly start day cannot be empty")]
    public WeekEnum? WeekStart { get; set; }

    /// <summary>
    /// Grouping symbols
    /// </summary>
    [Required(ErrorMessage = "Group symbol cannot be empty")]
    [MaxLength(255, ErrorMessage = "The grouping symbol character length cannot exceed 255")]
    public string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    [Required(ErrorMessage = "The decimal point symbol cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the decimal point character cannot exceed 255")]
    public string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    [MaxLength(255, ErrorMessage = "thousands separatorCharacterlengthcannot exceed255")]
    public string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    [Required(ErrorMessage = "Whether to enable cannot be empty")]
    public bool? Active { get; set; }
}

/// <summary>
/// Delete input parameters in multiple languages
/// </summary>
public class DeleteSysLangInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long? Id { get; set; }
}

/// <summary>
/// Multi-language update input parameters
/// </summary>
public class UpdateSysLangInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long? Id { get; set; }

    /// <summary>
    /// Language name
    /// </summary>
    [Required(ErrorMessage = "Language name cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language name character length cannot exceed 255")]
    public string Name { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language code character length cannot exceed 255")]
    public string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    [Required(ErrorMessage = "ISO Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "ISO language code character length cannot exceed 255")]
    public string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    [Required(ErrorMessage = "The URL language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "URL language code character length cannot exceed 255")]
    public string UrlCode { get; set; }

    /// <summary>
    /// writing direction
    /// </summary>
    [Required(ErrorMessage = "Writing direction cannot be empty")]
    public DirectionEnum Direction { get; set; }

    /// <summary>
    /// date format
    /// </summary>
    [Required(ErrorMessage = "Date format cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the date format characters cannot exceed 255")]
    public string DateFormat { get; set; }

    /// <summary>
    /// time format
    /// </summary>
    [Required(ErrorMessage = "time formatcannot benull")]
    [MaxLength(255, ErrorMessage = "The length of time format characters cannot exceed 255")]
    public string TimeFormat { get; set; }

    /// <summary>
    /// start day of week
    /// </summary>
    [Required(ErrorMessage = "Weekly start day cannot be empty")]
    public WeekEnum? WeekStart { get; set; }

    /// <summary>
    /// Grouping symbols
    /// </summary>
    [Required(ErrorMessage = "Group symbol cannot be empty")]
    [MaxLength(255, ErrorMessage = "The grouping symbol character length cannot exceed 255")]
    public string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    [Required(ErrorMessage = "The decimal point symbol cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the decimal point character cannot exceed 255")]
    public string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    [MaxLength(255, ErrorMessage = "thousands separatorCharacterlengthcannot exceed255")]
    public string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    [Required(ErrorMessage = "Whether to enable cannot be empty")]
    public bool? Active { get; set; }
}

/// <summary>
/// Multilingual primary key query input parameters
/// </summary>
public class QueryByIdSysLangInput : DeleteSysLangInput
{
}

/// <summary>
/// Multilingual data import entities
/// </summary>
[ExcelImporter(SheetIndex = 1, IsOnlyErrorRows = true)]
public class ImportSysLangInput : BaseImportInput
{
    /// <summary>
    /// Language name
    /// </summary>
    [ImporterHeader(Name = "*Language name")]
    [ExporterHeader("*Language name", Format = "", Width = 25, IsBold = true)]
    public string Name { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [ImporterHeader(Name = "*Language Code")]
    [ExporterHeader("*Language Code", Format = "", Width = 25, IsBold = true)]
    public string Code { get; set; }

    /// <summary>
    /// ISO language code
    /// </summary>
    [ImporterHeader(Name = "*ISO language code")]
    [ExporterHeader("*ISO language code", Format = "", Width = 25, IsBold = true)]
    public string IsoCode { get; set; }

    /// <summary>
    /// URL language code
    /// </summary>
    [ImporterHeader(Name = "*URL language code")]
    [ExporterHeader("*URL language code", Format = "", Width = 25, IsBold = true)]
    public string UrlCode { get; set; }

    /// <summary>
    /// writing direction
    /// </summary>
    [ImporterHeader(Name = "*Writing direction")]
    [ExporterHeader("*Writing direction", Format = "", Width = 25, IsBold = true)]
    public DirectionEnum Direction { get; set; }

    /// <summary>
    /// date format
    /// </summary>
    [ImporterHeader(Name = "*Date Format")]
    [ExporterHeader("*Date Format", Format = "", Width = 25, IsBold = true)]
    public string DateFormat { get; set; }

    /// <summary>
    /// time format
    /// </summary>
    [ImporterHeader(Name = "*Time Format")]
    [ExporterHeader("*Time Format", Format = "", Width = 25, IsBold = true)]
    public string TimeFormat { get; set; }

    /// <summary>
    /// start day of week
    /// </summary>
    [ImporterHeader(Name = "*Start day of each week")]
    [ExporterHeader("*Start day of each week", Format = "", Width = 25, IsBold = true)]
    public WeekEnum? WeekStart { get; set; }

    /// <summary>
    /// Grouping symbols
    /// </summary>
    [ImporterHeader(Name = "*Group symbol")]
    [ExporterHeader("*Group symbol", Format = "", Width = 25, IsBold = true)]
    public string Grouping { get; set; }

    /// <summary>
    /// decimal point symbol
    /// </summary>
    [ImporterHeader(Name = "*Decimal point symbol")]
    [ExporterHeader("*Decimal point symbol", Format = "", Width = 25, IsBold = true)]
    public string DecimalPoint { get; set; }

    /// <summary>
    /// Thousand separator
    /// </summary>
    [ImporterHeader(Name = "thousands separator")]
    [ExporterHeader("thousands separator", Format = "", Width = 25, IsBold = true)]
    public string? ThousandsSep { get; set; }

    /// <summary>
    /// Whether to enable
    /// </summary>
    [ImporterHeader(Name = "*Whether to enable")]
    [ExporterHeader("*Whether to enable", Format = "", Width = 25, IsBold = true)]
    public bool? Active { get; set; }
}