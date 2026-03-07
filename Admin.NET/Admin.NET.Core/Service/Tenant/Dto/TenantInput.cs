// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

public class TenantInput : BaseIdInput
{
    /// <summary>
    /// state
    /// </summary>
    public StatusEnum Status { get; set; }
}

public class PageTenantInput : BasePageInput
{
    /// <summary>
    /// name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Telephone
    /// </summary>
    public virtual string Phone { get; set; }
}

public class AddTenantInput : TenantOutput
{
    /// <summary>
    /// Tenant name
    /// </summary>
    [Required(ErrorMessage = "Tenant name cannot be empty"), MinLength(2, ErrorMessage = "The tenant name cannot be less than 2 characters")]
    public override string Name { get; set; }

    /// <summary>
    /// Tenant account
    /// </summary>
    [Required(ErrorMessage = "The rental management account cannot be empty"), MinLength(3, ErrorMessage = "The tenancy account number cannot be less than 3 characters")]
    public override string AdminAccount { get; set; }

    /// <summary>
    /// System main title
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Title)", "System main title cannot be empty")]
    public override string Title { get; set; }

    /// <summary>
    /// System subtitle
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(ViceTitle)", "systemSubtitle cannot be empty")]
    public override string ViceTitle { get; set; }

    /// <summary>
    /// System description
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(ViceDesc)", "System description cannot be empty")]
    public override string ViceDesc { get; set; }

    /// <summary>
    /// Copyright statement
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Copyright)", "Copyright description cannot be empty")]
    public override string Copyright { get; set; }

    /// <summary>
    /// ICP registration number
    /// </summary>
    public override string Icp { get; set; }

    /// <summary>
    /// ICP address
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Icp) && string.IsNullOrWhiteSpace(IcpUrl)", "ICP address cannot be empty")]
    public override string IcpUrl { get; set; }

    /// <summary>
    /// Logo image Base64 code
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Logo) && string.IsNullOrWhiteSpace(LogoBase64)", "Icon cannot be empty")]
    public virtual string LogoBase64 { get; set; }

    /// <summary>
    /// Logo file name
    /// </summary>
    public virtual string LogoFileName { get; set; }
}

public class UpdateTenantInput : AddTenantInput
{
}

public class DeleteTenantInput : BaseIdInput
{
}

/// <summary>
/// Tenant menu
/// </summary>
public class TenantMenuInput : BaseIdInput
{
    /// <summary>
    /// Synchronize tenant ID collection
    /// </summary>
    public List<long> TenantIdList { get; set; }

    /// <summary>
    /// MenuId collection
    /// </summary>
    public List<long> MenuIdList { get; set; }
}

public class TenantUserInput
{
    /// <summary>
    /// UserId
    /// </summary>
    public long UserId { get; set; }
}

public class TenantIdInput
{
    /// <summary>
    /// TenantId
    /// </summary>
    public long TenantId { get; set; }
}