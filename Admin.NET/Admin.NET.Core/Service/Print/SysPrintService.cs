// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System printing template service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 305)]
public class SysPrintService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysPrint> _sysPrintRep;
    private readonly UserManager _userManager;

    public SysPrintService(SqlSugarRepository<SysPrint> sysPrintRep, UserManager userManager)
    {
        _sysPrintRep = sysPrintRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Get a list of printable templates 🖨️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get print template list")]
    public async Task<SqlSugarPagedList<SysPrint>> Page(PagePrintInput input)
    {
        return await _sysPrintRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get the printable template 🖨️
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [DisplayName("Get printing template")]
    public async Task<SysPrint> GetPrint(string name)
    {
        return await _sysPrintRep.GetFirstAsync(u => u.Name == name);
    }

    /// <summary>
    /// Add printing template 🖨️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("increasePrint template")]
    public async Task AddPrint(AddPrintInput input)
    {
        var isExist = await _sysPrintRep.IsAnyAsync(u => u.Name == input.Name);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1800);

        await _sysPrintRep.InsertAsync(input.Adapt<SysPrint>());
    }

    /// <summary>
    /// Update printing template 🖨️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update printing template")]
    public async Task UpdatePrint(UpdatePrintInput input)
    {
        var isExist = await _sysPrintRep.IsAnyAsync(u => u.Name == input.Name && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1800);

        await _sysPrintRep.AsUpdateable(input.Adapt<SysPrint>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete print template 🖨️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete print template")]
    public async Task DeletePrint(DeletePrintInput input)
    {
        await _sysPrintRep.DeleteAsync(u => u.Id == input.Id);
    }
}