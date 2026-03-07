// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System tenant table
/// </summary>
[SugarTable(null, "System tenant table")]
[SysTable]
public partial class SysTenant : EntityBase
{
    /// <summary>
    /// Tenant user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Rent-controlled User ID")]
    public virtual long UserId { get; set; }

    /// <summary>
    /// InstitutionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Organization ID")]
    public virtual long OrgId { get; set; }

    /// <summary>
    /// domain name
    /// </summary>
    [SugarColumn(ColumnDescription = "domain name", Length = 128)]
    [MaxLength(128)]
    public virtual string? Host { get; set; }

    /// <summary>
    /// Tenant type
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant Type")]
    public virtual TenantTypeEnum TenantType { get; set; }

    /// <summary>
    /// Database type
    /// </summary>
    [SugarColumn(ColumnDescription = "Database type")]
    public virtual SqlSugar.DbType DbType { get; set; }

    /// <summary>
    /// Database connection
    /// </summary>
    [SugarColumn(ColumnDescription = "Database connection", Length = 256)]
    [MaxLength(256)]
    public virtual string? Connection { get; set; }

    /// <summary>
    /// Database ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Database ID", Length = 64)]
    [MaxLength(64)]
    public virtual string? ConfigId { get; set; }

    /// <summary>
    /// Connect/separate reading and writing from the library
    /// </summary>
    [SugarColumn(ColumnDescription = "Slave connection / read-write separation", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public virtual string? SlaveConnections { get; set; }

    /// <summary>
    /// Enable registration
    /// </summary>
    [SugarColumn(ColumnDescription = "Enable registration feature")]
    public virtual YesNoEnum? EnableReg { get; set; } = YesNoEnum.N;

    /// <summary>
    /// Default registration scheme ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Default registration scheme")]
    public virtual long? RegWayId { get; set; }

    /// <summary>
    /// icon
    /// </summary>
    [SugarColumn(ColumnDescription = "icon", Length = 256), MaxLength(256)]
    public virtual string? Logo { get; set; }

    /// <summary>
    /// title
    /// </summary>
    [SugarColumn(ColumnDescription = "title", Length = 32), MaxLength(32)]
    public virtual string? Title { get; set; }

    /// <summary>
    /// subtitle
    /// </summary>
    [SugarColumn(ColumnDescription = "Subtitle", Length = 32), MaxLength(32)]
    public virtual string? ViceTitle { get; set; }

    /// <summary>
    /// Sub-description
    /// </summary>
    [SugarColumn(ColumnDescription = "Sub-description", Length = 64), MaxLength(64)]
    public virtual string? ViceDesc { get; set; }

    /// <summary>
    /// watermark
    /// </summary>
    [SugarColumn(ColumnDescription = "Watermark", Length = 32), MaxLength(32)]
    public virtual string? Watermark { get; set; }

    /// <summary>
    /// Copyright information
    /// </summary>
    [SugarColumn(ColumnDescription = "Copyright information", Length = 64), MaxLength(64)]
    public virtual string? Copyright { get; set; }

    /// <summary>
    /// ICP registration number
    /// </summary>
    [SugarColumn(ColumnDescription = "ICP Filing Number", Length = 32), MaxLength(32)]
    public virtual string? Icp { get; set; }

    /// <summary>
    /// ICP address
    /// </summary>
    [SugarColumn(ColumnDescription = "ICP address", Length = 32), MaxLength(32)]
    public virtual string? IcpUrl { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public virtual int OrderNo { get; set; } = 100;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 128)]
    [MaxLength(128)]
    public virtual string? Remark { get; set; }

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public virtual StatusEnum Status { get; set; } = StatusEnum.Enable;
}