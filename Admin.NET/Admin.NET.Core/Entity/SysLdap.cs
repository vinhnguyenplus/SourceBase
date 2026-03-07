// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System domain login information configuration table
/// </summary>
[SugarTable(null, "System Domain Login Information Configuration Table")]
[SysTable]
public class SysLdap : EntityBaseTenantDel
{
    /// <summary>
    /// Host
    /// </summary>
    [SugarColumn(ColumnDescription = "Host", Length = 128)]
    [Required]
    public virtual string Host { get; set; }

    /// <summary>
    /// port
    /// </summary>
    [SugarColumn(ColumnDescription = "port")]
    public virtual int Port { get; set; }

    /// <summary>
    /// User search benchmark
    /// </summary>
    [SugarColumn(ColumnDescription = "User search benchmark", Length = 128)]
    [Required]
    public virtual string BaseDn { get; set; }

    /// <summary>
    /// Bind DN (user with administrative rights restrictions)
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind DN", Length = 128)]
    [Required]
    public virtual string BindDn { get; set; }

    /// <summary>
    /// Bind password (user password with administrative rights restrictions)
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind password", Length = 512)]
    [Required]
    public virtual string BindPass { get; set; }

    /// <summary>
    /// User filter rules
    /// </summary>
    [SugarColumn(ColumnDescription = "User filtering rules", Length = 128)]
    [Required]
    public virtual string AuthFilter { get; set; } = "sAMAccountName=%s";

    /// <summary>
    /// Ldap version
    /// </summary>
    [SugarColumn(ColumnDescription = "LDAP Version")]
    public int Version { get; set; }

    /// <summary>
    /// Bind domain account field attribute value
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind domain account field attribute value", Length = 32)]
    [Required]
    public virtual string BindAttrAccount { get; set; } = "sAMAccountName";

    /// <summary>
    /// Bind user EmployeeId attribute value
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind the user's EmployeeId property value", Length = 32)]
    [Required]
    public virtual string BindAttrEmployeeId { get; set; } = "EmployeeId";

    /// <summary>
    /// Bind Code attribute value
    /// </summary>
    [SugarColumn(ColumnDescription = "Bind the object's Code property value", Length = 64)]
    [Required]
    public virtual string BindAttrCode { get; set; } = "objectGUID";

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}