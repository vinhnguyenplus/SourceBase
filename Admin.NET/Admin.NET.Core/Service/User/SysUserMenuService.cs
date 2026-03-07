// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System user menu quick navigation service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 445)]
public class SysUserMenuService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysUserMenu> _sysUserMenuRep;
    private readonly UserManager _userManager;

    public SysUserMenuService(SqlSugarRepository<SysUserMenu> sysUserMenuRep, UserManager userManager)
    {
        _sysUserMenuRep = sysUserMenuRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Favorite menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("favorite menu")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task AddUserMenu(UserMenuInput input)
    {
        await _sysUserMenuRep.DeleteAsync(u => u.UserId == _userManager.UserId);

        if (input.MenuIdList == null || input.MenuIdList.Count == 0) return;
        var menus = input.MenuIdList.Select(u => new SysUserMenu
        {
            UserId = _userManager.UserId,
            MenuId = u
        }).ToList();
        await _sysUserMenuRep.InsertRangeAsync(menus);
    }

    /// <summary>
    /// Cancel favorite menu 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteUserMenu"), HttpPost]
    [DisplayName("Unfavorite Menu")]
    public async Task DeleteUserMenu(UserMenuInput input)
    {
        await _sysUserMenuRep.DeleteAsync(u => u.UserId == _userManager.UserId && input.MenuIdList.Contains(u.MenuId));
    }

    /// <summary>
    /// Get the current user’s favorite menu collection 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the collection of menus favorited by the current user")]
    public async Task<List<MenuOutput>> GetUserMenuList()
    {
        var sysUserMenuList = await _sysUserMenuRep.AsQueryable()
            .Includes(u => u.SysMenu)
            .Where(u => u.UserId == _userManager.UserId).ToListAsync();
        return sysUserMenuList.Where(u => u.SysMenu != null).Select(u => u.SysMenu).ToList().Adapt<List<MenuOutput>>();
    }

    /// <summary>
    /// Get the collection of menu IDs collected by the current user 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the current user's collection of menu IDs")]
    public async Task<List<long>> GetUserMenuIdList()
    {
        return await _sysUserMenuRep.AsQueryable()
            .Where(u => u.UserId == _userManager.UserId).Select(u => u.MenuId).ToListAsync();
    }

    /// <summary>
    /// Delete the specified user's favorite menu
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task DeleteUserMenuList(long userId)
    {
        await _sysUserMenuRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Delete favorite menus in batches
    /// </summary>
    /// <param name="ids"></param>
    [NonAction]
    public async Task DeleteMenuList(List<long> ids)
    {
        if (ids == null || ids.Count == 0) return;
        await _sysUserMenuRep.DeleteAsync(u => ids.Contains(u.MenuId));
    }
}