// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Currently logged in user
/// </summary>
public class UserManager : IScoped
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// User ID
    /// </summary>
    public long UserId => (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.UserId)?.Value).ToLong();

    /// <summary>
    /// Tenant ID
    /// </summary>
    public long TenantId => (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.TenantId)?.Value).ToLong();

    /// <summary>
    /// User account
    /// </summary>
    public string Account => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.Account)?.Value;

    /// <summary>
    /// real name
    /// </summary>
    public string RealName => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.RealName)?.Value;

    /// <summary>
    /// Account type
    /// </summary>
    public AccountTypeEnum? AccountType => int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.AccountType)?.Value, out var val) ? (AccountTypeEnum?)val : null;

    /// <summary>
    /// Is it a super administrator?
    /// </summary>
    public bool SuperAdmin => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString();

    /// <summary>
    /// Is it a system administrator?
    /// </summary>
    public bool SysAdmin => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SysAdmin).ToString();

    /// <summary>
    /// Organization ID
    /// </summary>
    public long OrgId => (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.OrgId)?.Value).ToLong();

    /// <summary>
    /// WeChatOpenId
    /// </summary>
    public string OpenId => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.OpenId)?.Value;

    public string LangCode => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.LangCode)?.Value ?? "zh_CN";

    public UserManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}