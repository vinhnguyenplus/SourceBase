// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System dictionary type service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 430, Description = "systemdictionaryType")]
public class SysDictTypeService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysDictType> _sysDictTypeRep;
    private readonly SysDictDataService _sysDictDataService;
    private readonly SysCacheService _sysCacheService;
    private readonly UserManager _userManager;
    private readonly SysLangTextCacheService _sysLangTextCacheService;

    public SysDictTypeService(SqlSugarRepository<SysDictType> sysDictTypeRep,
        SysDictDataService sysDictDataService,
        SysCacheService sysCacheService,
        UserManager userManager,
        SysLangTextCacheService sysLangTextCacheService)
    {
        _sysDictTypeRep = sysDictTypeRep;
        _sysDictDataService = sysDictDataService;
        _sysCacheService = sysCacheService;
        _userManager = userManager;
        _sysLangTextCacheService = sysLangTextCacheService;
    }

    /// <summary>
    /// Get dictionary type paginated list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get paginated list of dictionary types")]
    public async Task<SqlSugarPagedList<SysDictType>> Page(PageDictTypeInput input)
    {
        var langCode = _userManager.LangCode;
        var baseQuery = _sysDictTypeRep.AsQueryable()
            .WhereIF(!_userManager.SuperAdmin, u => u.IsTenant == YesNoEnum.Y)
            .WhereIF(!string.IsNullOrEmpty(input.Code?.Trim()), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrEmpty(input.Name?.Trim()), u => u.Name.Contains(input.Name));
        //.OrderBy(u => new { u.OrderNo, u.Code })
        var pageList = await baseQuery.ToPagedListAsync(input.Page, input.PageSize);
        var list = pageList.Items;
        var ids = list.Select(d => d.Id).Distinct().ToList();
        var translations = await _sysLangTextCacheService.GetTranslations(
                               "SysDictType",
                               "Name",
                               ids,
                               langCode);
        foreach (var item in list)
        {
            if (translations.TryGetValue(item.Id, out var translatedName) && !string.IsNullOrEmpty(translatedName))
            {
                item.Name = translatedName;
            }
        }
        pageList.Items = list;
        return pageList;
    }

    /// <summary>
    /// Get list of dictionary types 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the list of dictionary types")]
    public async Task<List<SysDictType>> GetList()
    {
        var langCode = _userManager.LangCode;
        var list = await _sysDictTypeRep.AsQueryable().OrderBy(u => new { u.OrderNo, u.Code }).ToListAsync();
        var ids = list.Select(d => d.Id).Distinct().ToList();
        var translations = await _sysLangTextCacheService.GetTranslations(
                               "SysDictType",
                               "Name",
                               ids,
                               langCode);
        foreach (var item in list)
        {
            if (translations.TryGetValue(item.Id, out var translatedName) && !string.IsNullOrEmpty(translatedName))
            {
                item.Name = translatedName;
            }
        }
        return list;
    }

    /// <summary>
    /// Get dictionary type-list of values ​​🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("ObtaindictionaryType-valueList")]
    public async Task<List<SysDictData>> GetDataList([FromQuery] GetDataDictTypeInput input)
    {
        var dictType = await _sysDictTypeRep.GetFirstAsync(u => u.Code == input.Code) ?? throw Oops.Oh(ErrorCodeEnum.D3000);
        var langCode = _userManager.LangCode;
        var list = await _sysDictDataService.GetDictDataListByDictTypeId(dictType.Id);
        var ids = list.Select(d => d.Id).Distinct().ToList();
        var translations = await _sysLangTextCacheService.GetTranslations(
                               "SysDictType",
                               "Name",
                               ids,
                               langCode);
        foreach (var item in list)
        {
            if (translations.TryGetValue(item.Id, out var translatedName) && !string.IsNullOrEmpty(translatedName))
            {
                item.Name = translatedName;
            }
        }
        return list;
    }

    /// <summary>
    /// Add dictionary type 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add dictionary type")]
    public async Task AddDictType(AddDictTypeInput input)
    {
        if (input.Code.ToLower().EndsWith("enum")) throw Oops.Oh(ErrorCodeEnum.D3006);
        if (input.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3008);

        var isExist = await _sysDictTypeRep.IsAnyAsync(u => u.Code == input.Code);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D3001);

        if (_userManager.SuperAdmin) input.IsTenant = YesNoEnum.N;  // The dictionary type added by the super administrator defaults to non-tenant level.

        await _sysDictTypeRep.InsertAsync(input.Adapt<SysDictType>());
    }

    /// <summary>
    /// Update dictionary type 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update dictionary type")]
    public async Task UpdateDictType(UpdateDictTypeInput input)
    {
        var dict = await _sysDictTypeRep.GetFirstAsync(x => x.Id == input.Id);
        if (dict.IsTenant != input.IsTenant) throw Oops.Oh(ErrorCodeEnum.D3012);
        if (dict == null) throw Oops.Oh(ErrorCodeEnum.D3000);

        if (dict.Code.ToLower().EndsWith("enum") && input.Code != dict.Code) throw Oops.Oh(ErrorCodeEnum.D3007);
        if (input.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3009);

        var isExist = await _sysDictTypeRep.IsAnyAsync(u => u.Code == input.Code && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D3001);

        _sysCacheService.Remove($"{CacheConst.KeyDict}{input.Code}");
        await _sysDictTypeRep.UpdateAsync(input.Adapt<SysDictType>());
    }

    /// <summary>
    /// Delete dictionary type 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete dictionary type")]
    public async Task DeleteDictType(DeleteDictTypeInput input)
    {
        var dictType = await _sysDictTypeRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D3000);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3010);

        // Delete dictionary value
        await _sysDictTypeRep.DeleteAsync(dictType);
        await _sysDictDataService.DeleteDictData(input.Id);
    }

    /// <summary>
    /// Get dictionary type details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get dictionary type details")]
    public async Task<SysDictType> GetDetail([FromQuery] DictTypeInput input)
    {
        return await _sysDictTypeRep.GetByIdAsync(input.Id);
    }

    /// <summary>
    /// Modify dictionary type status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Modify dictionary type status")]
    public async Task SetStatus(DictTypeInput input)
    {
        var dictType = await _sysDictTypeRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D3000);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3009);

        _sysCacheService.Remove($"{CacheConst.KeyDict}{dictType.Code}");

        dictType.Status = input.Status;
        await _sysDictTypeRep.AsUpdateable(dictType).UpdateColumns(u => new { u.Status }, true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Get all dictionary collection 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get all dictionary collections")]
    public async Task<dynamic> GetAllDictList()
    {
        var langCode = _userManager.LangCode;
        var ds = await _sysDictTypeRep.AsQueryable()
            .InnerJoin(_sysDictDataService.VSysDictData, (u, w) => u.Id == w.DictTypeId)
            .Select((u, w) => new DictDataOutput
            {
                DictDataId = w.Id, // for translation
                TypeCode = u.Code,
                Label = w.Label,
                Value = w.Value,
                Code = w.Code,
                TagType = w.TagType,
                StyleSetting = w.StyleSetting,
                ClassSetting = w.ClassSetting,
                ExtData = w.ExtData,
                Remark = w.Remark,
                OrderNo = w.OrderNo,
                Status = w.Status == StatusEnum.Enable && u.Status == StatusEnum.Enable ? StatusEnum.Enable : StatusEnum.Disable
            })
            .ToListAsync();
        var ids = ds.Select(x => x.DictDataId).Distinct().ToList();

        Dictionary<long, string> translations = new();
        if (ids.Any())
        {
            translations = await _sysLangTextCacheService.GetTranslations(
                "SysDictData",
                "Label",
                ids,
                langCode
            );
        }
        foreach (var item in ds)
        {
            if (translations.TryGetValue(item.DictDataId, out var translated) && !string.IsNullOrEmpty(translated))
            {
                item.Label = translated;
            }
        }

        var result = ds
            .OrderBy(u => u.OrderNo)
            .GroupBy(u => u.TypeCode)
            .ToDictionary(u => u.Key, u => u.ToList());

        return result;
    }
}