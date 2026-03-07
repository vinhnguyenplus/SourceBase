// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Code generation field configuration table
/// </summary>
[SugarTable(null, "Code Generation Field Configuration Table")]
[SysTable]
public partial class SysCodeGenConfig : EntityBase
{
    /// <summary>
    /// Code generation main table ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Main table ID")]
    public long CodeGenId { get; set; }

    /// <summary>
    /// Database field name
    /// </summary>
    [SugarColumn(ColumnDescription = "Field name", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string ColumnName { get; set; }

    /// <summary>
    /// primary key
    /// </summary>
    [SugarColumn(ColumnDescription = "Primary Key", Length = 8)]
    [MaxLength(8)]
    public string? ColumnKey { get; set; }

    /// <summary>
    /// Entity attribute name
    /// </summary>
    [SugarColumn(ColumnDescription = "Attributename", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string PropertyName { get; set; }

    /// <summary>
    /// Field data length
    /// </summary>
    [SugarColumn(ColumnDescription = "Field data length", DefaultValue = "0")]
    public int ColumnLength { get; set; }

    /// <summary>
    /// Field description
    /// </summary>
    [SugarColumn(ColumnDescription = "FieldDescription", Length = 128)]
    [MaxLength(128)]
    public string? ColumnComment { get; set; }

    /// <summary>
    /// Type in database (physical type)
    /// </summary>
    [SugarColumn(ColumnDescription = "DataWarehouseinType", Length = 64)]
    [MaxLength(64)]
    public string? DataType { get; set; }

    /// <summary>
    /// .NET data types
    /// </summary>
    [SugarColumn(ColumnDescription = "NET Data Types", Length = 64)]
    [MaxLength(64)]
    public string? NetType { get; set; }

    /// <summary>
    /// Field data default value
    /// </summary>
    [SugarColumn(ColumnDescription = "Default value")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Action type (dictionary)
    /// </summary>
    [SugarColumn(ColumnDescription = "Type of effect", Length = 64)]
    [MaxLength(64)]
    public string? EffectType { get; set; }

    /// <summary>
    /// Foreign key library identifier
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key library identifier", Length = 20)]
    [MaxLength(20)]
    public string? FkConfigId { get; set; }

    /// <summary>
    /// Foreign key entity name
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key entity name", Length = 64)]
    [MaxLength(64)]
    public string? FkEntityName { get; set; }

    /// <summary>
    /// Foreign key table name
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key table name", Length = 128)]
    [MaxLength(128)]
    public string? FkTableName { get; set; }

    /// <summary>
    /// Foreign key display field
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key display field", Length = 64)]
    [MaxLength(64)]
    public string? FkDisplayColumns { get; set; }

    /// <summary>
    /// Foreign key link field
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key link field", Length = 64)]
    [MaxLength(64)]
    public string? FkLinkColumnName { get; set; }

    /// <summary>
    /// Foreign key display field .NET type
    /// </summary>
    [SugarColumn(ColumnDescription = "Foreign key display field .NET type", Length = 64)]
    [MaxLength(64)]
    public string? FkColumnNetType { get; set; }

    /// <summary>
    /// parent field
    /// </summary>
    [SugarColumn(ColumnDescription = "Parent field", Length = 128)]
    [MaxLength(128)]
    public string? PidColumn { get; set; }

    /// <summary>
    /// dictionary encoding
    /// </summary>
    [SugarColumn(ColumnDescription = "Dictionary Encoding", Length = 64)]
    [MaxLength(64)]
    public string? DictTypeCode { get; set; }

    /// <summary>
    /// Query method
    /// </summary>
    [SugarColumn(ColumnDescription = "Query method", Length = 16)]
    [MaxLength(16)]
    public string? QueryType { get; set; }

    /// <summary>
    /// Is it a query condition?
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it a query condition?", Length = 8)]
    [MaxLength(8)]
    public string? WhetherQuery { get; set; }

    /// <summary>
    /// Whether the list is indented (dictionary)
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether the list is indented", Length = 8)]
    [MaxLength(8)]
    public string? WhetherRetract { get; set; }

    /// <summary>
    /// Is it required (dictionary)
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it required?", Length = 8)]
    [MaxLength(8)]
    public string? WhetherRequired { get; set; }

    /// <summary>
    /// Is it sortable (dictionary)
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it sortable?", Length = 8)]
    [MaxLength(8)]
    public string? WhetherSortable { get; set; }

    /// <summary>
    /// List display
    /// </summary>
    [SugarColumn(ColumnDescription = "List Display", Length = 8)]
    [MaxLength(8)]
    public string? WhetherTable { get; set; }

    /// <summary>
    /// Additions and changes
    /// </summary>
    [SugarColumn(ColumnDescription = "Additions and changes", Length = 8)]
    [MaxLength(8)]
    public string? WhetherAddUpdate { get; set; }

    /// <summary>
    /// import
    /// </summary>
    [SugarColumn(ColumnDescription = "import", Length = 8)]
    [MaxLength(8)]
    public string? WhetherImport { get; set; }

    /// <summary>
    /// Is it a common field?
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it a common field?", Length = 8)]
    [MaxLength(8)]
    public string? WhetherCommon { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public int OrderNo { get; set; } = 100;
}