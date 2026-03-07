// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Framework entity base class ID
/// </summary>
public abstract class EntityBaseId
{
    /// <summary>
    /// SnowflakeId
    /// </summary>
    [SugarColumn(ColumnName = "Id", ColumnDescription = "Primary keyId", IsPrimaryKey = true, IsIdentity = false)]
    public virtual long Id { get; set; }
}

/// <summary>
/// Framework entity base class
/// </summary>
[SugarIndex("index_{table}_CT", nameof(CreateTime), OrderByType.Asc)]
public abstract class EntityBase : EntityBaseId
{
    /// <summary>
    /// creation time
    /// </summary>
    [SugarColumn(ColumnDescription = "Creation Time", IsNullable = true, IsOnlyIgnoreUpdate = true)]
    public virtual DateTime CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    [SugarColumn(ColumnDescription = "Update Time")]
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    [OwnerUser]
    [SugarColumn(ColumnDescription = "CreatorId", IsOnlyIgnoreUpdate = true)]
    public virtual long? CreateUserId { get; set; }

    ///// <summary>
    /////Creator
    ///// </summary>
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //[Navigate(NavigateType.OneToOne, nameof(CreateUserId))]
    //public virtual SysUser CreateUser { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    [SugarColumn(ColumnDescription = "Creator name", Length = 64, IsOnlyIgnoreUpdate = true)]
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// Modifier ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Modifier ID")]
    public virtual long? UpdateUserId { get; set; }

    ///// <summary>
    /////Modifier
    ///// </summary>
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //[Navigate(NavigateType.OneToOne, nameof(UpdateUserId))]
    //public virtual SysUser UpdateUser { get; set; }

    /// <summary>
    /// Modifier name
    /// </summary>
    [SugarColumn(ColumnDescription = "Modifier name", Length = 64)]
    public virtual string? UpdateUserName { get; set; }
}

/// <summary>
/// Framework entity base class (remove flag)
/// </summary>
[SugarIndex("index_{table}_D", nameof(IsDelete), OrderByType.Asc)]
[SugarIndex("index_{table}_DT", nameof(DeleteTime), OrderByType.Asc)]
public abstract class EntityBaseDel : EntityBase, IDeletedFilter
{
    /// <summary>
    /// soft delete
    /// </summary>
    [SugarColumn(ColumnDescription = "soft delete")]
    public virtual bool IsDelete { get; set; } = false;

    /// <summary>
    /// soft delete time
    /// </summary>
    [SugarColumn(ColumnDescription = "soft delete time")]
    public virtual DateTime? DeleteTime { get; set; }
}

/// <summary>
/// Institutional entity base class (data permissions)
/// </summary>
public abstract class EntityBaseOrg : EntityBase, IOrgIdFilter
{
    /// <summary>
    /// InstitutionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Organization ID", IsNullable = true)]
    public virtual long OrgId { get; set; }

    ///// <summary>
    /////Creator department ID
    ///// </summary>
    //[SugarColumn(ColumnDescription = "Creator DepartmentId", IsOnlyIgnoreUpdate = true)]
    //public virtual long? CreateOrgId { get; set; }

    ///// <summary>
    /////Creator department
    ///// </summary>
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //[Navigate(NavigateType.OneToOne, nameof(CreateOrgId))]
    //public virtual SysOrg CreateOrg { get; set; }

    ///// <summary>
    /////Creator department name
    ///// </summary>
    //[SugarColumn(ColumnDescription = "Creator Department Name", Length = 64, IsOnlyIgnoreUpdate = true)]
    //public virtual string? CreateOrgName { get; set; }
}

/// <summary>
/// Institutional entity base class (data permissions, deletion flag)
/// </summary>
public abstract class EntityBaseOrgDel : EntityBaseDel, IOrgIdFilter
{
    /// <summary>
    /// InstitutionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Organization ID", IsNullable = true)]
    public virtual long OrgId { get; set; }
}

/// <summary>
/// Tenant entity base class
/// </summary>
public abstract class EntityBaseTenant : EntityBase, ITenantIdFilter
{
    /// <summary>
    /// TenantId
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant ID", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// Tenant entity base class (remove flag)
/// </summary>
public abstract class EntityBaseTenantDel : EntityBaseDel, ITenantIdFilter
{
    /// <summary>
    /// TenantId
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant ID", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// Tenant entity base class ID
/// </summary>
public abstract class EntityBaseTenantId : EntityBaseId, ITenantIdFilter
{
    /// <summary>
    /// TenantId
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant ID", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// Tenant Organization Entity Base Class (Data Permissions)
/// </summary>
public abstract class EntityBaseTenantOrg : EntityBaseOrg, ITenantIdFilter
{
    /// <summary>
    /// TenantId
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant ID", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// Tenant organization entity base class (data permissions, deletion flag)
/// </summary>
public abstract class EntityBaseTenantOrgDel : EntityBaseOrgDel, ITenantIdFilter
{
    /// <summary>
    /// TenantId
    /// </summary>
    [SugarColumn(ColumnDescription = "Tenant ID", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}