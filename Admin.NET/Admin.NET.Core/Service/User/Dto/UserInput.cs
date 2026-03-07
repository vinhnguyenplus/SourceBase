// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Set user status input parameters
/// </summary>
public class UserInput : BaseStatusInput
{
}

/// <summary>
/// Get user paginated list input parameters
/// </summary>
public class PageUserInput : BasePageInput
{
    /// <summary>
    /// TenantId
    /// </summary>
    public long TenantId { get; set; }

    /// <summary>
    /// account
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// Job title
    /// </summary>
    public string PosName { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// The institution ID selected when querying
    /// </summary>
    public long OrgId { get; set; }
}

/// <summary>
/// Add user input parameters
/// </summary>
public class AddUserInput : SysUser
{
    /// <summary>
    /// account
    /// </summary>
    [Required(ErrorMessage = "Account cannot be empty")]
    public override string Account { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    [Required(ErrorMessage = "Real name cannot be empty")]
    public override string RealName { get; set; }

    /// <summary>
    /// domain user
    /// </summary>
    public string DomainAccount { get; set; }

    /// <summary>
    /// role collection
    /// </summary>
    public List<long> RoleIdList { get; set; }

    /// <summary>
    /// Extended Institutional Collection
    /// </summary>
    public List<SysUserExtOrg> ExtOrgIdList { get; set; }
}

/// <summary>
/// Update user input parameters
/// </summary>
public class UpdateUserInput : AddUserInput
{
}

/// <summary>
/// Remove user input parameters
/// </summary>
public class DeleteUserInput : BaseIdInput
{
    /// <summary>
    /// InstitutionId
    /// </summary>
    public long OrgId { get; set; }
}

/// <summary>
/// Reset user password input parameters
/// </summary>
public class ResetPwdUserInput : BaseIdInput
{
}

/// <summary>
/// Modify user password input parameters
/// </summary>
public class ChangePwdInput
{
    /// <summary>
    /// Current Password
    /// </summary>
    [Required(ErrorMessage = "The current password cannot be empty")]
    public string PasswordOld { get; set; }

    /// <summary>
    /// New Password
    /// </summary>
    [Required(ErrorMessage = "New password cannot be empty"), MinLength(5, ErrorMessage = "The password needs to be more than 5 characters")]
    public string PasswordNew { get; set; }
}

/// <summary>
/// Unlock login input parameters
/// </summary>
public class UnlockLoginInput : BaseIdInput
{
}