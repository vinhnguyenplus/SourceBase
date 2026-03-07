// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// code generation table
/// </summary>
[SugarTable(null, "code generation table")]
[SysTable]
[SugarIndex("index_{table}_B", nameof(BusName), OrderByType.Asc)]
[SugarIndex("index_{table}_T", nameof(TableName), OrderByType.Asc)]
public partial class SysCodeGen : EntityBase
{
    /// <summary>
    /// Author name
    /// </summary>
    [SugarColumn(ColumnDescription = "Author's Name", Length = 32)]
    [MaxLength(32)]
    public string? AuthorName { get; set; }

    /// <summary>
    /// Whether to remove table prefix
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to remove the table prefix", Length = 8)]
    [MaxLength(8)]
    public string? TablePrefix { get; set; }

    /// <summary>
    /// Generation method
    /// </summary>
    [SugarColumn(ColumnDescription = "Generation method", Length = 32)]
    [MaxLength(32)]
    public string? GenerateType { get; set; }

    /// <summary>
    /// library locator name
    /// </summary>
    [SugarColumn(ColumnDescription = "Library Locator Name", Length = 64)]
    [MaxLength(64)]
    public string? ConfigId { get; set; }

    /// <summary>
    /// Library name
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string DbNickName
    {
        get
        {
            try
            {
                var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
                var config = dbOptions.ConnectionConfigs.FirstOrDefault(m => m.ConfigId.ToString() == ConfigId);
                return config.DbNickName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Database name (reserved field)
    /// </summary>
    [SugarColumn(ColumnDescription = "Database library name", Length = 64)]
    [MaxLength(64)]
    public string? DbName { get; set; }

    /// <summary>
    /// Database type
    /// </summary>
    [SugarColumn(ColumnDescription = "Database type", Length = 64)]
    [MaxLength(64)]
    public string? DbType { get; set; }

    /// <summary>
    /// Database link
    /// </summary>
    [SugarColumn(ColumnDescription = "Database link", Length = 256)]
    [MaxLength(256)]
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Database table name
    /// </summary>
    [SugarColumn(ColumnDescription = "Database table name", Length = 128)]
    [MaxLength(128)]
    public string? TableName { get; set; }

    /// <summary>
    /// namespace
    /// </summary>
    [SugarColumn(ColumnDescription = "namespace", Length = 128)]
    [MaxLength(128)]
    public string? NameSpace { get; set; }

    /// <summary>
    /// Business name
    /// </summary>
    [SugarColumn(ColumnDescription = "Business Name", Length = 128)]
    [MaxLength(128)]
    public string? BusName { get; set; }

    /// <summary>
    /// Table unique field configuration
    /// </summary>
    [SugarColumn(ColumnDescription = "Table unique field configuration", Length = 512)]
    [MaxLength(128)]
    public string? TableUniqueConfig { get; set; }

    /// <summary>
    /// Whether to generate a menu
    /// </summary>
    [SugarColumn(ColumnDescription = "Generate menu?")]
    public bool GenerateMenu { get; set; } = true;

    /// <summary>
    /// menu icon
    /// </summary>
    [SugarColumn(ColumnDescription = "Menu icon", Length = 32)]
    public string? MenuIcon { get; set; } = "ele-Menu";

    /// <summary>
    /// Menu encoding
    /// </summary>
    [SugarColumn(ColumnDescription = "Menu encoding")]
    public long? MenuPid { get; set; }

    /// <summary>
    /// Page directory
    /// </summary>
    [SugarColumn(ColumnDescription = "Page directory", Length = 32)]
    public string? PagePath { get; set; }

    /// <summary>
    /// Supported printing types
    /// </summary>
    [SugarColumn(ColumnDescription = "Supported print types", Length = 32)]
    [MaxLength(32)]
    public string? PrintType { get; set; }

    /// <summary>
    /// Print template name
    /// </summary>
    [SugarColumn(ColumnDescription = "Print Template Name", Length = 32)]
    [MaxLength(32)]
    public string? PrintName { get; set; }

    /// <summary>
    /// table unique field list
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public virtual List<TableUniqueConfigItem> TableUniqueList => string.IsNullOrWhiteSpace(TableUniqueConfig) ? null : JSON.Deserialize<List<TableUniqueConfigItem>>(TableUniqueConfig);
}