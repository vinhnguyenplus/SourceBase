// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System job service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 460)]
public class SysPosService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysPos> _sysPosRep;
    private readonly SysUserExtOrgService _sysUserExtOrgService;

    public SysPosService(UserManager userManager,
        SqlSugarRepository<SysPos> sysPosRep,
        SysUserExtOrgService sysUserExtOrgService)
    {
        _userManager = userManager;
        _sysPosRep = sysPosRep;
        _sysUserExtOrgService = sysUserExtOrgService;
    }

    /// <summary>
    /// Get job listings 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get job list")]
    public async Task<List<SysPos>> GetList([FromQuery] PosInput input)
    {
        return await _sysPosRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .Mapper(u =>
            {
                u.UserList = _sysPosRep.Context.Queryable<SysUser>()
                    .Where(a => a.PosId == u.Id || SqlFunc.Subqueryable<SysUserExtOrg>()
                        .Where(t => a.Id == t.UserId && t.PosId == u.Id).Any())
                    .ToList();
            })
            .ToListAsync();
    }

    /// <summary>
    /// Add positions 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add position")]
    public async Task AddPos(AddPosInput input)
    {
        if (await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code)) throw Oops.Oh(ErrorCodeEnum.D6000);

        await _sysPosRep.InsertAsync(input.Adapt<SysPos>());
    }

    /// <summary>
    /// Update job 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update Position")]
    public async Task UpdatePos(UpdatePosInput input)
    {
        if (await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D6000);

        var sysPos = await _sysPosRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D6003);
        if (!_userManager.SuperAdmin && sysPos.CreateUserId != _userManager.UserId) throw Oops.Oh(ErrorCodeEnum.D6002);

        await _sysPosRep.AsUpdateable(input.Adapt<SysPos>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete job 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete position")]
    public async Task DeletePos(DeletePosInput input)
    {
        var sysPos = await _sysPosRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D6003);
        if (!_userManager.SuperAdmin && sysPos.CreateUserId != _userManager.UserId) throw Oops.Oh(ErrorCodeEnum.D6002);

        // If there is a user in the position, deletion is prohibited.
        var hasPosEmp = await _sysPosRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.PosId == input.Id);
        if (hasPosEmp) throw Oops.Oh(ErrorCodeEnum.D6001);

        // If there are users in the affiliated positions, deletion is prohibited.
        var hasExtPosEmp = await _sysUserExtOrgService.HasUserPos(input.Id);
        if (hasExtPosEmp) throw Oops.Oh(ErrorCodeEnum.D6001);

        // If there is a binding registration plan, deletion is prohibited.
        var hasUserRegWay = await _sysPosRep.Context.Queryable<SysUserRegWay>().AnyAsync(u => u.PosId == input.Id);
        if (hasUserRegWay) throw Oops.Oh(ErrorCodeEnum.D6004);

        await _sysPosRep.DeleteAsync(u => u.Id == input.Id);
    }
}