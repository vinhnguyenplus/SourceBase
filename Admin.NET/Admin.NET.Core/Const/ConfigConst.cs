// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Configuration constants
/// </summary>
public class ConfigConst
{
    /// <summary>
    /// Demo environment
    /// </summary>
    public const string SysDemoEnv = "sys_demo";

    /// <summary>
    /// default password
    /// </summary>
    public const string SysPassword = "sys_password";

    /// <summary>
    /// Maximum number of incorrect passwords
    /// </summary>
    public const string SysPasswordMaxErrorTimes = "sys_password_max_error_times";

    /// <summary>
    /// Log retention days
    /// </summary>
    public const string SysLogRetentionDays = "sys_log_retention_days";

    /// <summary>
    /// Record operation log
    /// </summary>
    public const string SysOpLog = "sys_oplog";

    /// <summary>
    /// Single device login
    /// </summary>
    public const string SysSingleLogin = "sys_single_login";

    /// <summary>
    /// Login and logout reminder
    /// </summary>
    public const string SysLoginOutReminder = "sys_login_out_reminder";

    /// <summary>
    /// Hide tenants when logging in
    /// </summary>
    public const string SysHideTenantLogin = "sys_hide_tenant_login";

    /// <summary>
    /// Login two-step verification
    /// </summary>
    public const string SysSecondVer = "sys_second_ver";

    /// <summary>
    /// Graphic verification code
    /// </summary>
    public const string SysCaptcha = "sys_captcha";

    /// <summary>
    /// Token expiration time
    /// </summary>
    public const string SysTokenExpire = "sys_token_expire";

    /// <summary>
    /// RefreshToken expiration time
    /// </summary>
    public const string SysRefreshTokenExpire = "sys_refresh_token_expire";

    /// <summary>
    /// Send exception log email
    /// </summary>
    public const string SysErrorMail = "sys_error_mail";

    /// <summary>
    /// Domain login verification
    /// </summary>
    public const string SysDomainLogin = "sys_domain_login";

    // /// <summary>
    // /// Tenant domain name isolation login verification
    // /// </summary>
    // public const string SysTenantHostLogin = "sys_tenant_host_login";

    /// <summary>
    /// Data verification log
    /// </summary>
    public const string SysValidationLog = "sys_validation_log";

    /// <summary>
    /// Administrative region synchronization levels: 1-provincial level, 2-municipal level, 3-district and county level, 4-street level, 5-village level
    /// </summary>
    public const string SysRegionSyncLevel = "sys_region_sync_level";

    /// <summary>
    /// Default group
    /// </summary>
    public const string SysDefaultGroup = "Default";

    /// <summary>
    /// Alipay authorization page address
    /// </summary>
    public const string AlipayAuthPageUrl = "alipay_auth_page_url_";

    // /// <summary>
    // /// System icon
    // /// </summary>
    // public const string SysWebLogo = "sys_web_logo";
    //
    // /// <summary>
    // /// System main title
    // /// </summary>
    // public const string SysWebTitle = "sys_web_title";
    //
    // /// <summary>
    // /// System subtitle
    // /// </summary>
    // public const string SysWebViceTitle = "sys_web_viceTitle";
    //
    // /// <summary>
    // /// System description
    // /// </summary>
    // public const string SysWebViceDesc = "sys_web_viceDesc";
    //
    // /// <summary>
    // /// Watermark content
    // /// </summary>
    // public const string SysWebWatermark = "sys_web_watermark";
    //
    // /// <summary>
    // /// Copyright statement
    // /// </summary>
    // public const string SysWebCopyright = "sys_web_copyright";
    //
    // /// <summary>
    // /// ICP registration number
    // /// </summary>
    // public const string SysWebIcp = "sys_web_icp";
    //
    // /// <summary>
    // ///ICP address
    // /// </summary>
    // public const string SysWebIcpUrl = "sys_web_icpUrl";
}