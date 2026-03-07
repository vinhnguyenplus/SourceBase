// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Plugin.ApprovalFlow.Service;

/// <summary>
/// Approval flow basic input parameters
/// </summary>
public class ApprovalFlowBaseInput
{
    /// <summary>
    /// No
    /// </summary>
    public virtual string? Code { get; set; }

    /// <summary>
    /// name
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// form
    /// </summary>
    public virtual string? FormJson { get; set; }

    /// <summary>
    /// process
    /// </summary>
    public virtual string? FlowJson { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public virtual string? Remark { get; set; }

    /// <summary>
    /// creation time
    /// </summary>
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// Update time
    /// </summary>
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreatorId
    /// </summary>
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// Creator name
    /// </summary>
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// Modifier ID
    /// </summary>
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// Modifier name
    /// </summary>
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// Creator department ID
    /// </summary>
    public virtual long? CreateOrgId { get; set; }

    /// <summary>
    /// Creator department name
    /// </summary>
    public virtual string? CreateOrgName { get; set; }

    /// <summary>
    /// soft delete
    /// </summary>
    public virtual bool IsDelete { get; set; }
}

/// <summary>
/// Approval flow paging query input parameters
/// </summary>
public class ApprovalFlowInput : BasePageInput
{
    /// <summary>
    /// No
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// Add input parameters to the approval flow
/// </summary>
public class AddApprovalFlowInput : ApprovalFlowBaseInput
{
    /// <summary>
    /// soft delete
    /// </summary>
    [Required(ErrorMessage = "Soft delete cannot be empty")]
    public override bool IsDelete { get; set; }
}

/// <summary>
/// Approval flow delete input parameters
/// </summary>
public class DeleteApprovalFlowInput : BaseIdInput
{
}

/// <summary>
/// Approval flow update input parameters
/// </summary>
public class UpdateApprovalFlowInput : ApprovalFlowBaseInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public long Id { get; set; }
}

/// <summary>
/// Approval flow primary key query input parameters
/// </summary>
public class QueryByIdApprovalFlowInput : DeleteApprovalFlowInput
{
}