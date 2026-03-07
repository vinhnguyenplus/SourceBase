// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Alipay authorization record form
/// </summary>
[SugarTable(null, "Alipay Authorization Records Form")]
[SysTable]
[SugarIndex("index_{table}_U", nameof(UserId), OrderByType.Asc)]
[SugarIndex("index_{table}_T", nameof(OpenId), OrderByType.Asc)]
public class SysAlipayAuthInfo : EntityBase
{
    /// <summary>
    /// MerchantAppId
    /// </summary>
    [SugarColumn(ColumnDescription = "Merchant AppId", Length = 64)]
    public string? AppId { get; set; }

    /// <summary>
    /// Open ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Open ID", Length = 64)]
    public string? OpenId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    [SugarColumn(ColumnDescription = "User ID", Length = 64)]
    public string? UserId { get; set; }

    /// <summary>
    /// gender
    /// </summary>
    [SugarColumn(ColumnDescription = "gender", Length = 8)]
    public GenderEnum Gender { get; set; }

    /// <summary>
    /// age
    /// </summary>
    [SugarColumn(ColumnDescription = "age", Length = 16)]
    public int Age { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [SugarColumn(ColumnDescription = "Mobile phone number", Length = 32)]
    public string Mobile { get; set; }

    /// <summary>
    /// display name
    /// </summary>
    [SugarColumn(ColumnDescription = "Display Name", Length = 128)]
    public string DisplayName { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [SugarColumn(ColumnDescription = "Nickname", Length = 64)]
    public string NickName { get; set; }

    /// <summary>
    /// username
    /// </summary>
    [SugarColumn(ColumnDescription = "Username", Length = 64)]
    public string UserName { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    [SugarColumn(ColumnDescription = "Avatar", Length = 512)]
    public string? Avatar { get; set; }

    /// <summary>
    /// Mail
    /// </summary>
    [SugarColumn(ColumnDescription = "Email", Length = 128)]
    public string? Email { get; set; }

    /// <summary>
    /// User ethnicity
    /// </summary>
    [SugarColumn(ColumnDescription = "User ethnicity", Length = 32)]
    public string? UserNation { get; set; }

    /// <summary>
    /// Taobao ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Taobao ID", Length = 64)]
    public string? TaobaoId { get; set; }

    /// <summary>
    /// Telephone
    /// </summary>
    [SugarColumn(ColumnDescription = "Telephone", Length = 32)]
    public string? Phone { get; set; }

    /// <summary>
    /// Birthday
    /// </summary>
    [SugarColumn(ColumnDescription = "Birthday", Length = 32)]
    public string? PersonBirthday { get; set; }

    /// <summary>
    /// Profession
    /// </summary>
    [SugarColumn(ColumnDescription = "Profession", Length = 64)]
    public string? Profession { get; set; }

    /// <summary>
    /// province
    /// </summary>
    [SugarColumn(ColumnDescription = "Province", Length = 64)]
    public string? Province { get; set; }

    /// <summary>
    /// User status
    /// </summary>
    [SugarColumn(ColumnDescription = "User status", Length = 32)]
    public string? UserStatus { get; set; }

    /// <summary>
    /// Educational qualifications
    /// </summary>
    [SugarColumn(ColumnDescription = "Educational background", Length = 32)]
    public string? Degree { get; set; }

    /// <summary>
    /// User type
    /// </summary>
    [SugarColumn(ColumnDescription = "UserType", Length = 32)]
    public string? UserType { get; set; }

    /// <summary>
    /// post code
    /// </summary>
    [SugarColumn(ColumnDescription = "Postal code", Length = 16)]
    public string? Zip { get; set; }

    /// <summary>
    /// address
    /// </summary>
    [SugarColumn(ColumnDescription = "address", Length = 256)]
    public string? Address { get; set; }
}