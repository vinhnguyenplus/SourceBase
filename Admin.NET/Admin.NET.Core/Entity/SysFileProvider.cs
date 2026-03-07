// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;

namespace Admin.NET.Core;

/// <summary>
/// System file storage provider table
/// </summary>
[SugarTable(null, "System file storage provider table")]
[SysTable]
[SugarIndex("index_{table}_BucketName", nameof(BucketName), OrderByType.Asc)]
[SugarIndex("index_{table}_IsEnable", nameof(IsEnable), OrderByType.Desc)]
[SugarIndex("index_{table}_IsDefault", nameof(IsDefault), OrderByType.Desc)]
public partial class SysFileProvider : EntityBaseTenant
{
    /// <summary>
    /// Storage provider (Minio, QCloud, Aliyun, etc.)
    /// </summary>
    [SugarColumn(ColumnDescription = "storage provider", Length = 16)]
    [Required, MaxLength(16)]
    public virtual string Provider { get; set; }

    /// <summary>
    /// bucket name
    /// </summary>
    [SugarColumn(ColumnDescription = "bucket name", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string BucketName { get; set; }

    /// <summary>
    /// Access key (fill in Alibaba Cloud (Aliyun)/Minio: AccessKey, Tencent Cloud (QCloud): SecretId)
    /// </summary>
    [SugarColumn(ColumnDescription = "Access Key", Length = 128)]
    [MaxLength(128)]
    public virtual string? AccessKey { get; set; }

    /// <summary>
    /// key
    /// </summary>
    [SugarColumn(ColumnDescription = "key", Length = 128)]
    [MaxLength(128)]
    public virtual string? SecretKey { get; set; }

    /// <summary>
    /// area
    /// </summary>
    [SugarColumn(ColumnDescription = "Region", Length = 64)]
    [MaxLength(64)]
    public virtual string? Region { get; set; }

    /// <summary>
    /// Endpoint address (fill in the endpoint/Api address of Alibaba Cloud (Aliyun)/Minio:, the AppId of Tencent Cloud (QCloud):)
    /// </summary>
    [SugarColumn(ColumnDescription = "Endpoint Address", Length = 256)]
    [MaxLength(256)]
    public virtual string? Endpoint { get; set; }

    /// <summary>
    /// Whether to enable HTTPS
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether to enable HTTPS")]
    public virtual bool? IsEnableHttps { get; set; } = true;

    /// <summary>
    /// Whether to enable caching
    /// </summary>
    [SugarColumn(ColumnDescription = "Enable cache?")]
    public virtual bool? IsEnableCache { get; set; } = true;

    /// <summary>
    /// Whether to enable
    /// </summary>
    [SugarColumn(ColumnDescription = "Enable or not")]
    public virtual bool? IsEnable { get; set; } = true;

    /// <summary>
    /// Whether to use the default provider
    /// </summary>
    [SugarColumn(ColumnDescription = "Is the default provider")]
    public virtual bool? IsDefault { get; set; } = false;

    /// <summary>
    /// Custom domain name
    /// </summary>
    [SugarColumn(ColumnDescription = "Custom Domain", Length = 256)]
    [MaxLength(256)]
    public virtual string? SinceDomain { get; set; }

    /// <summary>
    /// sequence number
    /// </summary>
    [SugarColumn(ColumnDescription = "sequence number")]
    public virtual int? OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 512)]
    [MaxLength(512)]
    public virtual string? Remark { get; set; }

    /// <summary>
    /// Get display name
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public virtual string DisplayName => $"{Provider}-{BucketName}";

    /// <summary>
    /// Get configuration key name
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public virtual string ConfigKey => $"{Provider}_{BucketName}_{Id}";
}