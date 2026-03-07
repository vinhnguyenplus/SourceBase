// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Cache related constants
/// </summary>
public class CacheConst
{
    /// <summary>
    /// User permission cache (button collection)
    /// </summary>
    public const string KeyUserButton = "sys_user_button:";

    /// <summary>
    /// User organization cache
    /// </summary>
    public const string KeyUserOrg = "sys_user_org:";

    /// <summary>
    /// Role maximum data range cache
    /// </summary>
    public const string KeyRoleMaxDataScope = "sys_role_maxDataScope:";

    /// <summary>
    /// Online user caching
    /// </summary>
    public const string KeyUserOnline = "sys_user_online:";

    /// <summary>
    /// Graphical verification code cache
    /// </summary>
    public const string KeyVerCode = "sys_verCode:";

    /// <summary>
    /// Mobile phone verification code cache
    /// </summary>
    public const string KeyPhoneVerCode = "sys_phoneVerCode:";

    /// <summary>
    /// Password error count cache
    /// </summary>
    public const string KeyPasswordErrorTimes = "sys_password_error_times:";

    /// <summary>
    /// Tenant cache
    /// </summary>
    public const string KeyTenant = "sys_tenant";

    /// <summary>
    /// constant drop down box
    /// </summary>
    public const string KeyConst = "sys_const:";

    /// <summary>
    /// All cached keyword sets
    /// </summary>
    public const string KeyAll = "sys_keys";

    /// <summary>
    /// SqlSugar second level cache
    /// </summary>
    public const string SqlSugar = "sys_sqlSugar:";

    /// <summary>
    /// Open interface identity cache
    /// </summary>
    public const string KeyOpenAccess = "sys_open_access:";

    /// <summary>
    /// Open interface identity random number cache
    /// </summary>
    public const string KeyOpenAccessNonce = "sys_open_access_nonce:";

    /// <summary>
    /// Login blacklist
    /// </summary>
    public const string KeyBlacklist = "sys_blacklist:";

    /// <summary>
    /// System configuration cache
    /// </summary>
    public const string KeyConfig = "sys_config:";

    /// <summary>
    /// System tenant configuration cache
    /// </summary>
    public const string KeyTenantConfig = "sys_tenant_config:";

    /// <summary>
    /// System user configuration cache
    /// </summary>
    public const string KeyUserConfig = "sys_user_config:";

    /// <summary>
    /// System dictionary cache
    /// </summary>
    public const string KeyDict = "sys_dict:";

    /// <summary>
    /// System tenant dictionary cache
    /// </summary>
    public const string KeyTenantDict = "sys_tenant_dict:";

    /// <summary>
    /// Repeated requests for (idempotent) dictionary cache
    /// </summary>
    public const string KeyIdempotent = "sys_idempotent:";

    /// <summary>
    /// Excel temporary file cache
    /// </summary>
    public const string KeyExcelTemp = "sys_excel_temp:";

    /// <summary>
    /// System update command log cache
    /// </summary>
    public const string KeySysUpdateLog = "sys_update_log";

    /// <summary>
    /// System update interval tag cache
    /// </summary>
    public const string KeySysUpdateInterval = "sys_update_interval";
}