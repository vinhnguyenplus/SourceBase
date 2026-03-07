// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

[SugarTable(null, "Translation Table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(EntityName), OrderByType.Asc)]
[SugarIndex("index_{table}_F", nameof(FieldName), OrderByType.Asc)]
public class SysLangText : EntityBase
{
    /// <summary>
    /// Name of the entity to which it belongs
    /// </summary>
    [SugarColumn(ColumnDescription = "Name of the affiliated entity")]
    public string EntityName { get; set; }

    /// <summary>
    /// Owning entity ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Associated Entity ID")]
    public long EntityId { get; set; }

    /// <summary>
    /// Field name
    /// </summary>
    [SugarColumn(ColumnDescription = "Field Name")]
    public string FieldName { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    [SugarColumn(ColumnDescription = "Language code")]
    public string LangCode { get; set; }

    /// <summary>
    /// Multilingual content
    /// </summary>
    [SugarColumn(ColumnDescription = "Translate content")]
    public string Content { get; set; }
}