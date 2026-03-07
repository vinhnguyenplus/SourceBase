// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System user table seed data
/// </summary>
[IgnoreUpdateSeed]
public class SysUserSeedData : ISqlSugarEntitySeedData<SysUser>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUser> HasData()
    {
        var encryptPassword = CryptogramUtil.Encrypt(new SysConfigSeedData().HasData().First(u => u.Code == ConfigConst.SysPassword).Value);
        var posList = new SysPosSeedData().HasData().ToList();
        return new[]
        {
            new SysUser{ Id=1300000000101, Account="superAdmin.NET", Password=encryptPassword, NickName="super administrator", RealName="super administrator", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.SuperAdmin, Remark="super administrator", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId=SqlSugarConst.DefaultTenantId },
            new SysUser{ Id=1300000000111, Account="Admin.NET", Password=encryptPassword, NickName="system administrator", RealName="system administrator", Phone="18012345677", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.SysAdmin, Remark="system administrator", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=SqlSugarConst.DefaultTenantId, PosId=posList[0].Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysUser{ Id=1300000000112, Account="TestUser1", Password=encryptPassword, NickName="DepartmentSupervisor", RealName="DepartmentSupervisor", Phone="18012345676", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="DepartmentSupervisor", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=SqlSugarConst.DefaultTenantId + 1, PosId=posList[1].Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysUser{ Id=1300000000113, Account="TestUser2", Password=encryptPassword, NickName="Department staff", RealName="Department staff", Phone="18012345675", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="Department staff", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=SqlSugarConst.DefaultTenantId + 2, PosId=posList[2].Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysUser{ Id=1300000000114, Account="TestUser3", Password=encryptPassword, NickName="Ordinary user", RealName="Ordinary user", Phone="18012345674", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="Ordinary user", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=SqlSugarConst.DefaultTenantId + 3, PosId=posList[3].Id, TenantId=SqlSugarConst.DefaultTenantId },
            new SysUser{ Id=1300000000115, Account="TestUser4", Password=encryptPassword, NickName="Other", RealName="Other", Phone="18012345673", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.Member, Remark="Member", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=SqlSugarConst.DefaultTenantId + 4, PosId=posList[4].Id, TenantId=SqlSugarConst.DefaultTenantId },
        };
    }
}