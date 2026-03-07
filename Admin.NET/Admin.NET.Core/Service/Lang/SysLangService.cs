// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// Language Services 🧩
/// </summary>
[ApiDescriptionSettings(Order = 100, Description = "Language services")]
public partial class SysLangService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLang> _sysLangRep;

    public SysLangService(SqlSugarRepository<SysLang> sysLangRep)
    {
        _sysLangRep = sysLangRep;
    }

    /// <summary>
    /// Paginated query language 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Paginated query language")]
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    public async Task<SqlSugarPagedList<SysLangOutput>> Page(PageSysLangInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _sysLangRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.Name.Contains(input.Keyword) || u.Code.Contains(input.Keyword) || u.IsoCode.Contains(input.Keyword) || u.UrlCode.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.IsoCode), u => u.IsoCode.Contains(input.IsoCode.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UrlCode), u => u.UrlCode.Contains(input.UrlCode.Trim()))
            .Select<SysLangOutput>();
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get language details ℹ️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get language details")]
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    public async Task<SysLang> Detail([FromQuery] QueryByIdSysLangInput input)
    {
        return await _sysLangRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Add language ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("increaseLanguage")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddSysLangInput input)
    {
        var entity = input.Adapt<SysLang>();
        return await _sysLangRep.InsertAsync(entity) ? entity.Id : 0;
    }

    /// <summary>
    /// Update language ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Update language")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateSysLangInput input)
    {
        var entity = input.Adapt<SysLang>();
        await _sysLangRep.AsUpdateable(entity)
        .ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete language ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Delete language")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteSysLangInput input)
    {
        var entity = await _sysLangRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _sysLangRep.DeleteAsync(entity);   // Really delete
    }

    /// <summary>
    /// Get drop-down list data 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Get drop-down list data")]
    [ApiDescriptionSettings(Name = "DropdownData"), HttpPost]
    public async Task<dynamic> DropdownData()
    {
        var data = await _sysLangRep.Context.Queryable<SysLang>()
            .Where(m => m.Active == true)
            .Select(u => new
            {
                Code = u.Code,
                Value = u.UrlCode,
                Label = $"{u.Name}"
            }).ToListAsync();
        return data;
    }
}