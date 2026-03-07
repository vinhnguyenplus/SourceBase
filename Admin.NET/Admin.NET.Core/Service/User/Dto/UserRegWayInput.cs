// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Registration plan paging query input parameters
/// </summary>
public class PageUserRegWayInput : BasePageInput
{
    /// <summary>
    /// Scheme name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// TenantId
    /// </summary>
    public long TenantId { get; set; }
}

/// <summary>
/// Add input parameters to the registration plan
/// </summary>
public class AddUserRegWayInput : SysUserRegWay
{
    /// <summary>
    /// Scheme name
    /// </summary>
    [Required(ErrorMessage = "Scheme name cannot be empty")]
    [MaxLength(32, ErrorMessage = "Plan NameCharacterlengthcannot exceed32")]
    public override string Name { get; set; }

    /// <summary>
    /// Account type
    /// </summary>
    [Enum(ErrorMessage = "The account type is incorrect")]
    public override AccountTypeEnum AccountType { get; set; }

    /// <summary>
    /// Role
    /// </summary>
    [Required(ErrorMessage = "Role cannot be empty")]
    public override long RoleId { get; set; }

    /// <summary>
    /// mechanism
    /// </summary>
    [Required(ErrorMessage = "Organization cannot be empty")]
    public override long OrgId { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    [Required(ErrorMessage = "Position cannot be empty")]
    public override long PosId { get; set; }
}

/// <summary>
/// Registration scheme update input parameters
/// </summary>
public class UpdateUserRegWayInput : AddUserRegWayInput
{
    /// <summary>
    /// Primary keyId
    /// </summary>
    [Required(ErrorMessage = "Primary key Id cannot be empty")]
    public override long Id { get; set; }
}