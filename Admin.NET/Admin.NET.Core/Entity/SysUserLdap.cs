// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user domain configuration table
/// </summary>
[SugarTable(null, "System User Domain Configuration Table")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(Account), OrderByType.Asc)]
[SugarIndex("index_{table}_U", nameof(UserId), OrderByType.Asc)]
public class SysUserLdap : EntityBaseTenantId
{
    /// <summary>
    /// UserId
    /// </summary>
    [SugarColumn(ColumnDescription = "UserId")]
    public long UserId { get; set; }

    /// <summary>
    /// Domain account
    /// AD domain corresponds to sAMAccountName
    /// Ldap corresponds to uid
    /// </summary>
    [SugarColumn(ColumnDescription = "Domain account", Length = 32)]
    [Required]
    public string Account { get; set; }

    /// <summary>
    /// domain username
    /// </summary>
    [SugarColumn(ColumnDescription = "DomainUsername", Length = 32)]
    public string UserName { get; set; }

    /// <summary>
    /// Corresponds to EmployeeId (used for data import control)
    /// </summary>
    [SugarColumn(ColumnDescription = "Corresponds to EmployeeId", Length = 32)]
    public string? EmployeeId { get; set; }

    /// <summary>
    /// organization code
    /// </summary>
    [SugarColumn(ColumnDescription = "organization code", Length = 64)]
    public string? DeptCode { get; set; }

    /// <summary>
    /// Last set password time
    /// </summary>
    [SugarColumn(ColumnDescription = "Last set password time")]
    public DateTime? PwdLastSetTime { get; set; }

    /// <summary>
    /// Mail
    /// </summary>
    [SugarColumn(ColumnDescription = "organization code", Length = 64)]
    public string? Mail { get; set; }

    /// <summary>
    /// Check if the account has expired
    /// </summary>
    [SugarColumn(ColumnDescription = "Check if the account has expired")]
    public bool AccountExpiresFlag { get; set; } = false;

    /// <summary>
    /// Whether the password setting never expires
    /// </summary>
    [SugarColumn(ColumnDescription = "Whether the password is set to never expire")]
    public bool DontExpiresFlag { get; set; } = false;

    /// <summary>
    /// DN
    /// </summary>
    [SugarColumn(ColumnDescription = "DN", Length = 512)]
    public string Dn { get; set; }
}