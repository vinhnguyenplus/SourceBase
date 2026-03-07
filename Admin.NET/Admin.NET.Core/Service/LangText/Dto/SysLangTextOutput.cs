// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!
namespace Admin.NET.Core;

/// <summary>
/// Translation table output parameters
/// </summary>
public class SysLangTextOutput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    public long EntityId { get; set; }

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

/// <summary>
/// Translate table data into template entities
/// </summary>
public class ExportSysLangTextOutput : ImportSysLangTextInput
{
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    public override string Error { get; set; }
}