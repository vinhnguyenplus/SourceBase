// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System dictionary type table seed data
/// </summary>
public class SysDictTypeSeedData : ISqlSugarEntitySeedData<SysDictType>
{
    /// <summary>
    /// Seed data
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysDictType> HasData()
    {
        return new[]
        {
            new SysDictType{ Id=1300000000111, Name="Code generation control type", Code="code_gen_effect_type", SysFlag=YesNoEnum.Y, OrderNo=100, Remark="Code generation control type", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000121, Name="Code generation query type", Code="code_gen_query_type", SysFlag=YesNoEnum.Y, OrderNo=101, Remark="Code generation query type", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000131, Name="Code generation .NET types", Code="code_gen_net_type", SysFlag=YesNoEnum.Y, OrderNo=102, Remark="Code generation .NET types", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000141, Name="Code generation method", Code="code_gen_create_type", SysFlag=YesNoEnum.Y, OrderNo=103, Remark="Code generation method", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000151, Name="Code Generation Base Class", Code="code_gen_base_class", SysFlag=YesNoEnum.Y, OrderNo=104, Remark="Code Generation Base Class", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000161, Name="Code generation print type", Code="code_gen_print_type", SysFlag=YesNoEnum.Y, OrderNo=105, Remark="Code generation print type", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-12-04 00:00:00") },
            new SysDictType{ Id=1300000000171, Name="Institution type", Code="org_type", SysFlag=YesNoEnum.Y, OrderNo=201, Remark="Institution type", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
        };
    }
}