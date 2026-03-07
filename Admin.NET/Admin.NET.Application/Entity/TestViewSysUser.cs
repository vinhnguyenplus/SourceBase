// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// User table view (must add IgnoreTable to prevent it from being generated as a table)
/// </summary>
[SugarTable(null, "UserTable View"), IgnoreTable]
public class TestViewSysUser : EntityBase, ISqlSugarView
{
    /// <summary>
    /// account
    /// </summary>
    [SugarColumn(ColumnDescription = "Account number")]
    public virtual string Account { get; set; }

    /// <summary>
    /// real name
    /// </summary>
    [SugarColumn(ColumnDescription = "Real Name")]
    public virtual string RealName { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [SugarColumn(ColumnDescription = "Nickname")]
    public string? NickName { get; set; }

    /// <summary>
    /// Organization name
    /// </summary>
    [SugarColumn(ColumnDescription = "Organization name")]
    public string? OrgName { get; set; }

    /// <summary>
    /// Job title
    /// </summary>
    [SugarColumn(ColumnDescription = "Job title")]
    public string? PosName { get; set; }

    /// <summary>
    /// Query instance
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public string GetQueryableSqlString(SqlSugarScopeProvider db)
    {
        return db.Queryable<SysUser>()
            .LeftJoin<SysOrg>((u, a) => u.OrgId == a.Id)
            .LeftJoin<SysPos>((u, a, b) => u.PosId == b.Id)
            .Select((u, a, b) => new TestViewSysUser
            {
                Id = u.Id,
                Account = u.Account,
                RealName = u.RealName,
                NickName = u.NickName,
                OrgName = a.Name,
                PosName = b.Name,
            }).ToMappedSqlString();
    }
}