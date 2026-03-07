// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Translation table basic input parameters
/// </summary>
public class SysLangTextBaseInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public virtual long? Id { get; set; }

    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [Required(ErrorMessage = "The name of the entity to which it belongs cannot be empty.")]
    public virtual string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [Required(ErrorMessage = "The entity ID cannot be empty")]
    public virtual long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [Required(ErrorMessage = "Field name cannot be empty")]
    public virtual string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    public virtual string LangCode { get; set; }

    /// <summary>
    /// Translate content
    /// </summary>
    [Required(ErrorMessage = "Translation content cannot be empty")]
    public virtual string Content { get; set; }
}

/// <summary>
/// Translation table paging query input parameters
/// </summary>
public class PageSysLangTextInput : BasePageInput
{
    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    public long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    public string LangCode { get; set; }

    /// <summary>
    /// Translate content
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Select primary key list
    /// </summary>
    public List<long> SelectKeyList { get; set; }
}

/// <summary>
/// Add input parameters to translation table
/// </summary>
public class AddSysLangTextInput
{
    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [Required(ErrorMessage = "The name of the entity to which it belongs cannot be empty.")]
    [MaxLength(255, ErrorMessage = "The character length of the entity name cannot exceed 255")]
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [Required(ErrorMessage = "The entity ID cannot be empty")]
    public long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [Required(ErrorMessage = "Field name cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the field name cannot exceed 255 characters")]
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language code character length cannot exceed 255")]
    public string LangCode { get; set; }

    /// <summary>
    /// Translate content
    /// </summary>
    [Required(ErrorMessage = "Translation content cannot be empty")]
    public string Content { get; set; }
}

/// <summary>
/// Translation table input parameters
/// </summary>
public class ListSysLangTextInput
{
    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [Required(ErrorMessage = "The name of the entity to which it belongs cannot be empty.")]
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [Required(ErrorMessage = "The entity ID cannot be empty")]
    public long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [Required(ErrorMessage = "Field name cannot be empty")]
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    public string LangCode { get; set; }
}

/// <summary>
/// Translation table delete input parameter
/// </summary>
public class DeleteSysLangTextInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long? Id { get; set; }
}

/// <summary>
/// Translation table update input parameters
/// </summary>
public class UpdateSysLangTextInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long? Id { get; set; }

    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [Required(ErrorMessage = "The name of the entity to which it belongs cannot be empty.")]
    [MaxLength(255, ErrorMessage = "The character length of the entity name cannot exceed 255")]
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [Required(ErrorMessage = "The entity ID cannot be empty")]
    public long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [Required(ErrorMessage = "Field name cannot be empty")]
    [MaxLength(255, ErrorMessage = "The length of the field name cannot exceed 255 characters")]
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [Required(ErrorMessage = "Language code cannot be empty")]
    [MaxLength(255, ErrorMessage = "The language code character length cannot exceed 255")]
    public string LangCode { get; set; }

    /// <summary>
    /// Translate content
    /// </summary>
    [Required(ErrorMessage = "Translation content cannot be empty")]
    public string Content { get; set; }
}

/// <summary>
/// Translation table primary key query input parameters
/// </summary>
public class QueryByIdSysLangTextInput : DeleteSysLangTextInput
{
}

/// <summary>
/// Translate table data into entities
/// </summary>
[ExcelImporter(SheetIndex = 1, IsOnlyErrorRows = true)]
public class ImportSysLangTextInput : BaseImportInput
{
    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [ImporterHeader(Name = "*Affiliated Entity Name")]
    [ExporterHeader("*Affiliated Entity Name", Format = "", Width = 25, IsBold = true)]
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [ImporterHeader(Name = "*Official entity ID")]
    [ExporterHeader("*Official entity ID", Format = "", Width = 25, IsBold = true)]
    public long? EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [ImporterHeader(Name = "*Field Name")]
    [ExporterHeader("*Field Name", Format = "", Width = 25, IsBold = true)]
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [ImporterHeader(Name = "*Language Code")]
    [ExporterHeader("*Language Code", Format = "", Width = 25, IsBold = true)]
    public string LangCode { get; set; }

    /// <summary>
    /// Translate content
    /// </summary>
    [ImporterHeader(Name = "*Translation content")]
    [ExporterHeader("*Translation content", Format = "", Width = 25, IsBold = true)]
    public string Content { get; set; }
}

/// <summary>
///
/// </summary>
public class AiTranslateTextInput
{
    /// <summary>
    /// original
    /// </summary>
    public string OriginalText { get; set; }

    /// <summary>
    /// target language
    /// </summary>
    public string TargetLang { get; set; }
}