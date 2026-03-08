// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System WeChat user table
/// </summary>
[SugarTable(null, "System WeChat user table")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(NickName), OrderByType.Asc)]
[SugarIndex("index_{table}_M", nameof(Mobile), OrderByType.Asc)]
public partial class SysWechatUser : EntityBase
{
    /// <summary>
    /// System user ID
    /// </summary>
    [SugarColumn(ColumnDescription = "System User ID")]
    public long UserId { get; set; }

    /// <summary>
    /// system user
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// platform type
    /// </summary>
    [SugarColumn(ColumnDescription = "Platform Type")]
    public PlatformTypeEnum PlatformType { get; set; } = PlatformTypeEnum.WeChatOfficialAccount;

    /// <summary>
    /// OpenId
    /// </summary>
    [SugarColumn(ColumnDescription = "OpenId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string OpenId { get; set; }

    /// <summary>
    /// session key
    /// </summary>
    [SugarColumn(ColumnDescription = "Session key", Length = 256)]
    [MaxLength(256)]
    public string? SessionKey { get; set; }

    /// <summary>
    /// UnionId
    /// </summary>
    [SugarColumn(ColumnDescription = "UnionId", Length = 64)]
    [MaxLength(64)]
    public string? UnionId { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [SugarColumn(ColumnDescription = "Nickname", Length = 64)]
    [MaxLength(64)]
    public string? NickName { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    [SugarColumn(ColumnDescription = "Avatar", Length = 256)]
    [MaxLength(256)]
    public string? Avatar { get; set; }

    /// <summary>
    /// phone number
    /// </summary>
    [SugarColumn(ColumnDescription = "Mobile phone number", Length = 16)]
    [MaxLength(16)]
    public string? Mobile { get; set; }

    /// <summary>
    /// gender
    /// </summary>
    [SugarColumn(ColumnDescription = "gender")]
    public int? Sex { get; set; }

    /// <summary>
    /// language
    /// </summary>
    [SugarColumn(ColumnDescription = "Language", Length = 64)]
    [MaxLength(64)]
    public string? Language { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [SugarColumn(ColumnDescription = "city", Length = 64)]
    [MaxLength(64)]
    public string? City { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    [SugarColumn(ColumnDescription = "Province", Length = 64)]
    [MaxLength(64)]
    public string? Province { get; set; }

    /// <summary>
    /// nation
    /// </summary>
    [SugarColumn(ColumnDescription = "Country", Length = 64)]
    [MaxLength(64)]
    public string? Country { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    [SugarColumn(ColumnDescription = "AccessToken", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// RefreshToken
    /// </summary>
    [SugarColumn(ColumnDescription = "RefreshToken", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiration time
    /// </summary>
    [SugarColumn(ColumnDescription = "ExpiresIn")]
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// User authorization scopes, separated by commas
    /// </summary>
    [SugarColumn(ColumnDescription = "Authorization Scope", Length = 64)]
    [MaxLength(64)]
    public string? Scope { get; set; }
}