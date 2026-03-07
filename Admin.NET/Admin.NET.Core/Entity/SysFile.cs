// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System file table
/// </summary>
[SugarTable(null, "System file table")]
[SysTable]
[SugarIndex("index_{table}_F", nameof(FileName), OrderByType.Asc)]
public partial class SysFile : EntityBaseTenantOrg
{
    /// <summary>
    /// provider
    /// </summary>
    [SugarColumn(ColumnDescription = "Provider", Length = 128)]
    [MaxLength(128)]
    public string? Provider { get; set; }

    /// <summary>
    /// Warehouse name
    /// </summary>
    [SugarColumn(ColumnDescription = "Warehouse Name", Length = 128)]
    [MaxLength(128)]
    public string? BucketName { get; set; }

    /// <summary>
    /// File name (source file name)
    /// </summary>
    [SugarColumn(ColumnDescription = "File name", Length = 128)]
    [MaxLength(128)]
    public string? FileName { get; set; }

    /// <summary>
    /// file suffix
    /// </summary>
    [SugarColumn(ColumnDescription = "file suffix", Length = 16)]
    [MaxLength(16)]
    public string? Suffix { get; set; }

    /// <summary>
    /// storage path
    /// </summary>
    [SugarColumn(ColumnDescription = "Storage Path", Length = 512)]
    [MaxLength(512)]
    public string? FilePath { get; set; }

    /// <summary>
    /// File size KB
    /// </summary>
    [SugarColumn(ColumnDescription = "File size KB")]
    public long SizeKb { get; set; }

    /// <summary>
    /// File size information - calculated
    /// </summary>
    [SugarColumn(ColumnDescription = "File size information", Length = 64)]
    [MaxLength(64)]
    public string? SizeInfo { get; set; }

    /// <summary>
    /// External link address - After uploading to OSS, the external link address is generated to facilitate front-end preview.
    /// </summary>
    [SugarColumn(ColumnDescription = "External link address", Length = 512)]
    [MaxLength(512)]
    public string? Url { get; set; }

    /// <summary>
    /// File MD5
    /// </summary>
    [SugarColumn(ColumnDescription = "File MD5", Length = 128)]
    [MaxLength(128)]
    public string? FileMd5 { get; set; }

    /// <summary>
    /// File category
    /// </summary>
    [SugarColumn(ColumnDescription = "File category", Length = 128)]
    [MaxLength(128)]
    public virtual string? FileType { get; set; }

    /// <summary>
    /// file alias
    /// </summary>
    [SugarColumn(ColumnDescription = "File alias", Length = 128)]
    [MaxLength(128)]
    public string? FileAlias { get; set; }

    /// <summary>
    /// Is it public?
    /// </summary>
    [SugarColumn(ColumnDescription = "Is it public?")]
    public virtual bool IsPublic { get; set; } = false;

    /// <summary>
    /// Business data ID
    /// </summary>
    [SugarColumn(ColumnDescription = "BusinessDataId")]
    public long? DataId { get; set; }
}