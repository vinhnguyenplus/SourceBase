// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System user registration solution service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 490)]
public class SysUserRegWayService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysUserRegWay> _sysUserRegWayRep;
    private readonly UserManager _userManager;

    public SysUserRegWayService(SqlSugarRepository<SysUserRegWay> sysUserRegWayRep, UserManager userManager)
    {
        _sysUserRegWayRep = sysUserRegWayRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Check the registration plan list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Query the list of registration plans")]
    [ApiDescriptionSettings(Name = "List"), HttpPost]
    public async Task<List<UserRegWayOutput>> List(PageUserRegWayInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _sysUserRegWayRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.Name.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .LeftJoin<SysRole>((u, a) => u.RoleId == a.Id)
            .LeftJoin<SysOrg>((u, a, b) => u.OrgId == b.Id)
            .LeftJoin<SysPos>((u, a, b, c) => u.PosId == c.Id)
            .Select((u, a, b, c) => new UserRegWayOutput
            {
                RoleName = a.Name,
                OrgName = b.Name,
                PosName = c.Name,
            }, true);
        return await query.OrderBuilder(input).ToListAsync();
    }

    /// <summary>
    /// Add registration plan ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Add registration plan")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddUserRegWayInput input)
    {
        var entity = input.Adapt<SysUserRegWay>();
        if (await _sysUserRegWayRep.IsAnyAsync(u => u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D2101);

        await CheckData(input);
        return await _sysUserRegWayRep.InsertAsync(entity) ? entity.Id : 0;
    }

    /// <summary>
    /// Update registration plan ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Update registration plan")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateUserRegWayInput input)
    {
        if (await _sysUserRegWayRep.IsAnyAsync(u => u.Id != input.Id && u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D2101);

        await CheckData(input);
        await _sysUserRegWayRep.AsUpdateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// Check data
    /// </summary>
    /// <param name="input"></param>
    [NonAction]
    public async Task CheckData(AddUserRegWayInput input)
    {
        // Check if foreign key data exists
        if (!await _sysUserRegWayRep.Context.Queryable<SysRole>().AnyAsync(u => u.Id == input.RoleId)) throw Oops.Oh(ErrorCodeEnum.D1036);
        if (!await _sysUserRegWayRep.Context.Queryable<SysOrg>().AnyAsync(u => u.Id == input.OrgId)) throw Oops.Oh(ErrorCodeEnum.D2011);
        if (!await _sysUserRegWayRep.Context.Queryable<SysPos>().AnyAsync(u => u.Id == input.PosId)) throw Oops.Oh(ErrorCodeEnum.D6003);

        // Registration of super administrators and system administrators is prohibited
        if (input.AccountType is AccountTypeEnum.SysAdmin or AccountTypeEnum.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D1037);
    }

    /// <summary>
    /// Delete registration plan ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Delete registration plan")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(BaseIdInput input)
    {
        var entity = await _sysUserRegWayRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        // Close related tenant registration function
        await _sysUserRegWayRep.Context.Updateable(new SysTenant { EnableReg = YesNoEnum.N, RegWayId = null })
            .UpdateColumns(u => new { u.EnableReg, u.RegWayId })
            .Where(u => u.RegWayId == input.Id)
            .ExecuteCommandAsync();

        // Delete plan
        await _sysUserRegWayRep.DeleteAsync(entity);
    }
}