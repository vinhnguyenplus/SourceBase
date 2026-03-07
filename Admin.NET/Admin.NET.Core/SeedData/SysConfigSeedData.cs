// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System configuration table seed data
/// </summary>
public class SysConfigSeedData : ISqlSugarEntitySeedData<SysConfig>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysConfig> HasData()
    {
        return new[]
        {
            new SysConfig{ Id=1300000000101, Name="Demonstration Environment", Code=ConfigConst.SysDemoEnv, Value="False", SysFlag=YesNoEnum.Y, Remark="Demonstration Environment", OrderNo=10, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000111, Name="default password", Code=ConfigConst.SysPassword, Value="Admin.NET++010101", SysFlag=YesNoEnum.Y, Remark="default password", OrderNo=20, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000121, Name="Maximum number of incorrect passwords", Code=ConfigConst.SysPasswordMaxErrorTimes, Value="5", SysFlag=YesNoEnum.Y, Remark="Maximum number of incorrect password entries allowed", OrderNo=30, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000131, Name="Log retention days", Code=ConfigConst.SysLogRetentionDays, Value="180", SysFlag=YesNoEnum.Y, Remark="Log retention days (days)", OrderNo=40, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000141, Name="Record operation log", Code=ConfigConst.SysOpLog, Value="True", SysFlag=YesNoEnum.Y, Remark="Whether to record operation logs", OrderNo=50, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000151, Name="Single device login", Code=ConfigConst.SysSingleLogin, Value="False", SysFlag=YesNoEnum.Y, Remark="Whether to enable single device login", OrderNo=60, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000152, Name="Login and Logout Reminder", Code=ConfigConst.SysLoginOutReminder, Value="False", SysFlag=YesNoEnum.Y, Remark="Whether to enable login and logout reminders", OrderNo=60, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000161, Name="Login Two-Factor Authentication", Code=ConfigConst.SysSecondVer, Value="False", SysFlag=YesNoEnum.Y, Remark="Whether to enable two-step login verification", OrderNo=70, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000171, Name="Graphic verification code", Code=ConfigConst.SysCaptcha, Value="True", SysFlag=YesNoEnum.Y, Remark="Whether to enable graphic verification code", OrderNo=80, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000172, Name="Hide tenant when logging in", Code=ConfigConst.SysHideTenantLogin, Value="True", SysFlag=YesNoEnum.Y, Remark="Hide tenant when logging in", OrderNo=90, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000181, Name="Token expiration time", Code=ConfigConst.SysTokenExpire, Value="10080", SysFlag=YesNoEnum.Y, Remark="Token expiration time (minutes)", OrderNo=100, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000191, Name="RefreshToken expiration time", Code=ConfigConst.SysRefreshTokenExpire, Value="20160", SysFlag=YesNoEnum.Y, Remark="Refresh token expiration time (minutes) (generally the validity period of refresh_token > 2 * the validity period of access_token)", OrderNo=110, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000201, Name="Send exception log email", Code=ConfigConst.SysErrorMail, Value="False", SysFlag=YesNoEnum.Y, Remark="Whether to send exception log emails", OrderNo=120, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000211, Name="Domain login verification", Code=ConfigConst.SysDomainLogin, Value="False", SysFlag=YesNoEnum.Y, Remark="Whether to enable domain login verification", OrderNo=130, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000221, Name="Data Validation Log", Code=ConfigConst.SysValidationLog, Value="True", SysFlag=YesNoEnum.Y, Remark="Whether data verification log", OrderNo=140, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000231, Name="Administrative region synchronization level", Code=ConfigConst.SysRegionSyncLevel, Value="3", SysFlag=YesNoEnum.Y, Remark="Administrative Region Synchronization Levels 1-Provincial, 2-City, 3-District/County, 4-Street, 5-Village", OrderNo=150, GroupCode=ConfigConst.SysDefaultGroup, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}