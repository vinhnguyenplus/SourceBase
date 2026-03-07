// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System tenant table seed data
/// </summary>
public class SysTenantSeedData : ISqlSugarEntitySeedData<SysTenant>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysTenant> HasData()
    {
        var defaultDbConfig = App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0];
        var userList = new SysUserSeedData().HasData().ToList();
        var admin = userList.First(u => u.Account == "Admin.NET");
        return new[]
        {
            new SysTenant{ Id=SqlSugarConst.DefaultTenantId, OrgId=SqlSugarConst.DefaultTenantId, UserId=admin.Id, Host="gitee.com", TenantType=TenantTypeEnum.Id, DbType=defaultDbConfig.DbType, Connection=defaultDbConfig.ConnectionString, ConfigId=SqlSugarConst.MainConfigId, Logo="/upload/logo.png", Title="Admin.NET", ViceTitle="Admin.NET", ViceDesc=".NET General Permission Development Framework Standing on the Shoulders of Giants", Watermark="Admin.NET", Copyright="Copyright \u00a9 2021-present Admin.NET All rights reserved.", Icp="Provincial ICP No. 12345678", IcpUrl="https:// beian.miit.gov.cn", Remark="System Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}