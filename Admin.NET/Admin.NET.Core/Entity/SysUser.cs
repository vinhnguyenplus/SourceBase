// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user table
/// </summary>
[SugarTable(null, "System User Table")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(Account), OrderByType.Asc)]
[SugarIndex("index_{table}_P", nameof(Phone), OrderByType.Asc)]
public partial class SysUser : EntityBaseTenantOrg
{
    /// <summary>
    /// account
    /// </summary>
    [SugarColumn(ColumnDescription = "Account number", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string Account { get; set; }

    /// <summary>
    /// password
    /// </summary>
    [SugarColumn(ColumnDescription = "password", Length = 512)]
    [MaxLength(512)]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual string Password { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    [SugarColumn(ColumnDescription = "Real Name", Length = 32)]
    [MaxLength(32)]
    public virtual string RealName { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [SugarColumn(ColumnDescription = "Nickname", Length = 32)]
    [MaxLength(32)]
    public string? NickName { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    [SugarColumn(ColumnDescription = "Avatar", Length = 512)]
    [MaxLength(512)]
    public string? Avatar { get; set; }

    /// <summary>
    /// Gender-Male_1, Female_2
    /// </summary>
    [SugarColumn(ColumnDescription = "gender")]
    public GenderEnum Sex { get; set; } = GenderEnum.Male;

    /// <summary>
    /// age
    /// </summary>
    [SugarColumn(ColumnDescription = "age")]
    public int Age { get; set; }

    /// <summary>
    /// date of birth
    /// </summary>
    [SugarColumn(ColumnDescription = "date of birth")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// nationality
    /// </summary>
    [SugarColumn(ColumnDescription = "ethnic group", Length = 32)]
    [MaxLength(32)]
    public string? Nation { get; set; }

    /// <summary>
    /// phone number
    /// </summary>
    [SugarColumn(ColumnDescription = "Mobile phone number", Length = 16)]
    [MaxLength(16)]
    public string? Phone { get; set; }

    /// <summary>
    /// Document type
    /// </summary>
    [SugarColumn(ColumnDescription = "Type of ID")]
    public CardTypeEnum CardType { get; set; }

    /// <summary>
    /// ID number
    /// </summary>
    [SugarColumn(ColumnDescription = "ID number", Length = 32)]
    [MaxLength(32)]
    public string? IdCardNum { get; set; }

    /// <summary>
    /// Mail
    /// </summary>
    [SugarColumn(ColumnDescription = "Email", Length = 64)]
    [MaxLength(64)]
    public string? Email { get; set; }

    /// <summary>
    /// address
    /// </summary>
    [SugarColumn(ColumnDescription = "address", Length = 256)]
    [MaxLength(256)]
    public string? Address { get; set; }

    /// <summary>
    /// Education level
    /// </summary>
    [SugarColumn(ColumnDescription = "Education level")]
    public CultureLevelEnum CultureLevel { get; set; }

    /// <summary>
    /// political outlook
    /// </summary>
    [SugarColumn(ColumnDescription = "political outlook", Length = 16)]
    [MaxLength(16)]
    public string? PoliticalOutlook { get; set; }

    /// <summary>
    /// Graduation school
    /// </summary>
    [SugarColumn(ColumnDescription = "Graduation school", Length = 128)]
    [MaxLength(128)]
    public string? College { get; set; }

    /// <summary>
    /// Office phone
    /// </summary>
    [SugarColumn(ColumnDescription = "Office phone", Length = 16)]
    [MaxLength(16)]
    public string? OfficePhone { get; set; }

    /// <summary>
    /// emergency contact
    /// </summary>
    [SugarColumn(ColumnDescription = "Emergency Contact", Length = 32)]
    [MaxLength(32)]
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Emergency contact number
    /// </summary>
    [SugarColumn(ColumnDescription = "Emergency contact number", Length = 16)]
    [MaxLength(16)]
    public string? EmergencyPhone { get; set; }

    /// <summary>
    /// Emergency contact address
    /// </summary>
    [SugarColumn(ColumnDescription = "UrgentContact address", Length = 256)]
    [MaxLength(256)]
    public string? EmergencyAddress { get; set; }

    /// <summary>
    /// Profile
    /// </summary>
    [SugarColumn(ColumnDescription = "Personal Profile", Length = 512)]
    [MaxLength(512)]
    public string? Introduction { get; set; }

    /// <summary>
    /// sort
    /// </summary>
    [SugarColumn(ColumnDescription = "Sort")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// state
    /// </summary>
    [SugarColumn(ColumnDescription = "state")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnDescription = "Remarks", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }

    /// <summary>
    /// Account type
    /// </summary>
    [SugarColumn(ColumnDescription = "Account Type")]
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.NormalUser;

    ///// <summary>
    ///// Directly affiliated institution ID
    ///// </summary>
    //[SugarColumn(ColumnDescription = "Directly affiliated institution Id")]
    //public long OrgId { get; set; }

    /// <summary>
    /// Directly affiliated organizations
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(OrgId))]
    public SysOrg SysOrg { get; set; }

    /// <summary>
    /// Direct supervisor ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Direct Supervisor ID")]
    public long? ManagerUserId { get; set; }

    /// <summary>
    /// Immediate supervisor
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(ManagerUserId))]
    public SysUser ManagerUser { get; set; }

    /// <summary>
    /// PositionId
    /// </summary>
    [SugarColumn(ColumnDescription = "Job ID")]
    public long PosId { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(PosId))]
    public SysPos SysPos { get; set; }

    /// <summary>
    /// Job number
    /// </summary>
    [SugarColumn(ColumnDescription = "Job number", Length = 32)]
    [MaxLength(32)]
    public string? JobNum { get; set; }

    /// <summary>
    /// Rank
    /// </summary>
    [SugarColumn(ColumnDescription = "Rank", Length = 32)]
    [MaxLength(32)]
    public string? PosLevel { get; set; }

    /// <summary>
    /// job title
    /// </summary>
    [SugarColumn(ColumnDescription = "Professional title", Length = 32)]
    [MaxLength(32)]
    public string? PosTitle { get; set; }

    /// <summary>
    /// Areas of expertise
    /// </summary>
    [SugarColumn(ColumnDescription = "Areas of expertise", Length = 32)]
    [MaxLength(32)]
    public string? Expertise { get; set; }

    /// <summary>
    /// Office area
    /// </summary>
    [SugarColumn(ColumnDescription = "Office area", Length = 32)]
    [MaxLength(32)]
    public string? OfficeZone { get; set; }

    /// <summary>
    /// office
    /// </summary>
    [SugarColumn(ColumnDescription = "Office", Length = 32)]
    [MaxLength(32)]
    public string? Office { get; set; }

    /// <summary>
    /// Joining date
    /// </summary>
    [SugarColumn(ColumnDescription = "Date of Joining")]
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// Latest login IP
    /// </summary>
    [SugarColumn(ColumnDescription = "Latest login IP", Length = 256)]
    [MaxLength(256)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Latest login location
    /// </summary>
    [SugarColumn(ColumnDescription = "Latest login location", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginAddress { get; set; }

    /// <summary>
    /// Latest login time
    /// </summary>
    [SugarColumn(ColumnDescription = "Latest login time")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// Latest login device
    /// </summary>
    [SugarColumn(ColumnDescription = "Latest login devices", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginDevice { get; set; }

    /// <summary>
    /// electronic signature
    /// </summary>
    [SugarColumn(ColumnDescription = "Electronic signature", Length = 512)]
    [MaxLength(512)]
    public string? Signature { get; set; }

    /// <summary>
    /// Language code (such as zh-CN)
    /// </summary>
    [SugarColumn(ColumnDescription = "Language code")]
    public string LangCode { get; set; } = App.GetOptions<LocalizationSettingsOptions>().DefaultCulture;

    /// <summary>
    /// Personalized home page address
    /// </summary>
    [SugarColumn(ColumnDescription = "Personalized homepage address", Length = 512)]
    [MaxLength(512)]
    public string? Homepage { get; set; }

    /// <summary>
    /// Verify the super administrator type. If the account type is super administrator, an error will be reported.
    /// </summary>
    /// <param name="errorMsg">Custom error message</param>
    public void ValidateIsSuperAdminAccountType(ErrorCodeEnum? errorMsg = ErrorCodeEnum.D1014)
    {
        if (AccountType == AccountTypeEnum.SuperAdmin)
        {
            throw Oops.Oh(errorMsg);
        }
    }

    /// <summary>
    /// Verify whether the user IDs are the same. If the user IDs are the same, an error will be reported.
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <param name="errorMsg">Custom error message</param>
    public void ValidateIsUserId(long userId, ErrorCodeEnum? errorMsg = ErrorCodeEnum.D1001)
    {
        if (Id == userId)
        {
            throw Oops.Oh(errorMsg);
        }
    }
}