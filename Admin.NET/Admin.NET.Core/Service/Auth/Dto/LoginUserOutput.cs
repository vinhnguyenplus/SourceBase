// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// User login information
/// </summary>
public class LoginUserOutput
{
    /// <summary>
    /// user id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Account name
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// Telephone
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// ID card
    /// </summary>
    public string IdCardNum { get; set; }

    /// <summary>
    /// Mail
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Account type
    /// </summary>
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.NormalUser;

    /// <summary>
    /// avatar
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// Profile
    /// </summary>
    public string Introduction { get; set; }

    /// <summary>
    /// address
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// electronic signature
    /// </summary>
    public string Signature { get; set; }

    /// <summary>
    /// InstitutionId
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// Institution type
    /// </summary>
    public string OrgType { get; set; }

    /// <summary>
    /// Job title
    /// </summary>
    public string PosName { get; set; }

    /// <summary>
    /// Button permission collection
    /// </summary>
    public List<string> Buttons { get; set; }

    /// <summary>
    /// role collection
    /// </summary>
    public List<long> RoleIds { get; set; }

    /// <summary>
    /// watermark text
    /// </summary>
    public string WatermarkText { get; set; }

    /// <summary>
    /// TenantId
    /// </summary>
    public long? TenantId { get; set; }

    /// <summary>
    /// Tenant ID currently switched to
    /// </summary>
    public long? CurrentTenantId { get; set; }

    /// <summary>
    /// language code
    /// </summary>
    public string LangCode { get; internal set; }
}