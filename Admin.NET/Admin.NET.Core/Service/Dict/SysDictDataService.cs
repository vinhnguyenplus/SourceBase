// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System dictionary value service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 420, Description = "System dictionary value")]
public class SysDictDataService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysDictData> _sysDictDataRep;
    public readonly ISugarQueryable<SysDictData> VSysDictData;
    private readonly SysCacheService _sysCacheService;
    private readonly UserManager _userManager;
    private readonly SysLangTextCacheService _sysLangTextCacheService;

    public SysDictDataService(SqlSugarRepository<SysDictData> sysDictDataRep,
        SysCacheService sysCacheService,
        UserManager userManager,
        SysLangTextCacheService sysLangTextCacheService)
    {
        _userManager = userManager;
        _sysDictDataRep = sysDictDataRep;
        _sysCacheService = sysCacheService;
        _sysLangTextCacheService = sysLangTextCacheService;
        VSysDictData = _sysDictDataRep.Context.UnionAll(
            _sysDictDataRep.AsQueryable(),
            _sysDictDataRep.Change<SysDictDataTenant>().AsQueryable()
            //.WhereIF(_userManager.SuperAdmin, d => d.TenantId == _userManager.TenantId)
            .Select<SysDictData>());
    }

    /// <summary>
    /// Get a paginated list of dictionary values ​​🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get paginated list of dictionary values")]
    public async Task<SqlSugarPagedList<SysDictData>> Page(PageDictDataInput input)
    {
        var langCode = _userManager.LangCode;
        var baseQuery = VSysDictData
            .Where(u => u.DictTypeId == input.DictTypeId)
            .WhereIF(!string.IsNullOrEmpty(input.Code?.Trim()), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrEmpty(input.Label?.Trim()), u => u.Label.Contains(input.Label))
            .OrderBy(u => new { u.OrderNo, u.Code });
        var pageList = await baseQuery.ToPagedListAsync(input.Page, input.PageSize);
        var list = pageList.Items;
        var ids = list.Select(d => d.Id).Distinct().ToList();
        var translations = await _sysLangTextCacheService.GetTranslations(
                               "SysDictData",
                               "Label",
                               ids,
                               langCode);
        foreach (var item in list)
        {
            if (translations.TryGetValue(item.Id, out var translatedLabel) && !string.IsNullOrEmpty(translatedLabel))
            {
                item.Label = translatedLabel;
            }
        }
        pageList.Items = list;
        return pageList;
    }

    /// <summary>
    /// Get a list of dictionary values ​​🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get a list of dictionary values")]
    public async Task<List<SysDictData>> GetList([FromQuery] GetDataDictDataInput input)
    {
        var langCode = _userManager.LangCode;
        var list = await GetDictDataListByDictTypeId(input.DictTypeId);
        var ids = list.Select(d => d.Id).Distinct().ToList();
        var translations = await _sysLangTextCacheService.GetTranslations(
                               "SysDictData",
                               "Label",
                               ids,
                               langCode);
        foreach (var item in list)
        {
            if (translations.TryGetValue(item.Id, out var translatedLabel) && !string.IsNullOrEmpty(translatedLabel))
            {
                item.Label = translatedLabel;
            }
        }
        return await GetDictDataListByDictTypeId(input.DictTypeId);
    }

    /// <summary>
    /// Add dictionary value 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add dictionary value")]
    public async Task AddDictData(AddDictDataInput input)
    {
        var isExist = await VSysDictData.AnyAsync(u => u.Value == input.Value && u.DictTypeId == input.DictTypeId);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D3003);

        var dictType = await _sysDictDataRep.Change<SysDictType>().GetByIdAsync(input.DictTypeId);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3008);

        Remove(dictType);

        dynamic dictData = dictType.IsTenant == YesNoEnum.Y ? input.Adapt<SysDictDataTenant>() : input.Adapt<SysDictData>();
        await _sysDictDataRep.Context.Insertable(dictData).ExecuteCommandAsync();
    }

    /// <summary>
    /// Update dictionary value 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update dictionary value")]
    public async Task UpdateDictData(UpdateDictDataInput input)
    {
        var isExist = await VSysDictData.AnyAsync(u => u.Id == input.Id);
        if (!isExist) throw Oops.Oh(ErrorCodeEnum.D3004);

        isExist = await VSysDictData.AnyAsync(u => u.Value == input.Value && u.DictTypeId == input.DictTypeId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D3003);

        var dictType = await _sysDictDataRep.Change<SysDictType>().GetByIdAsync(input.DictTypeId);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3009);

        Remove(dictType);
        dynamic dictData = dictType.IsTenant == YesNoEnum.Y ? input.Adapt<SysDictDataTenant>() : input.Adapt<SysDictData>();
        await _sysDictDataRep.Context.Updateable(dictData).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete dictionary value 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete dictionary value")]
    public async Task DeleteDictData(DeleteDictDataInput input)
    {
        var dictData = await VSysDictData.FirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D3004);

        var dictType = await _sysDictDataRep.Change<SysDictType>().GetByIdAsync(dictData.DictTypeId);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3010);

        Remove(dictType);
        dynamic entity = dictType.IsTenant == YesNoEnum.Y ? input.Adapt<SysDictDataTenant>() : input.Adapt<SysDictData>();
        await _sysDictDataRep.Context.Deleteable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// Get dictionary value details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get dictionary value details")]
    public async Task<SysDictData> GetDetail([FromQuery] DictDataInput input)
    {
        return (await VSysDictData.FirstAsync(u => u.Id == input.Id))?.Adapt<SysDictData>();
    }

    /// <summary>
    /// Modify dictionary value status 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("Modify dictionary value status")]
    public async Task SetStatus(DictDataInput input)
    {
        var dictData = await VSysDictData.FirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D3004);

        var dictType = await _sysDictDataRep.Change<SysDictType>().GetByIdAsync(dictData.DictTypeId);
        if (dictType.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3009);

        Remove(dictType);

        dictData.Status = input.Status;
        dynamic entity = dictType.IsTenant == YesNoEnum.Y ? input.Adapt<SysDictDataTenant>() : input.Adapt<SysDictData>();
        await _sysDictDataRep.Context.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// Get a collection of dictionary values ​​based on dictionary type Id
    /// </summary>
    /// <param name="dictTypeId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysDictData>> GetDictDataListByDictTypeId(long dictTypeId)
    {
        return await GetDataListByIdOrCode(dictTypeId, null);
    }

    /// <summary>
    /// Get a collection of dictionary values ​​based on dictionary type encoding 🔖
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [DisplayName("Get the set of dictionary values based on the dictionary type code")]
    public async Task<List<SysDictData>> GetDataList(string code)
    {
        return await GetDataListByIdOrCode(null, code);
    }

    /// <summary>
    /// Get a collection of dictionary values ​​🔖
    /// </summary>
    /// <param name="typeId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysDictData>> GetDataListByIdOrCode(long? typeId, string code)
    {
        if (string.IsNullOrWhiteSpace(code) && typeId == null ||
            !string.IsNullOrWhiteSpace(code) && typeId != null)
            throw Oops.Oh(ErrorCodeEnum.D3011);

        var dictType = await _sysDictDataRep.Change<SysDictType>().AsQueryable()
            .Where(u => u.Status == StatusEnum.Enable)
            .WhereIF(!string.IsNullOrWhiteSpace(code), u => u.Code == code)
            .WhereIF(typeId != null, u => u.Id == typeId)
            .FirstAsync();
        if (dictType == null) return null;

        string dicKey = dictType.IsTenant == YesNoEnum.N ? $"{CacheConst.KeyDict}{dictType.Code}" : $"{CacheConst.KeyTenantDict}{_userManager}:{dictType?.Code}";
        var dictDataList = _sysCacheService.Get<List<SysDictData>>(dicKey);
        if (dictDataList == null)
        {
            //Platform dictionary and tenant dictionary are cached separately
            if (dictType.IsTenant == YesNoEnum.Y)
            {
                dictDataList = await _sysDictDataRep.Change<SysDictDataTenant>().AsQueryable()
                       .Where(u => u.DictTypeId == dictType.Id)
                       .Where(u => u.Status == StatusEnum.Enable)
                       .WhereIF(_userManager.SuperAdmin, d => d.TenantId == _userManager.TenantId).Select<SysDictData>()
                       .OrderBy(u => new { u.OrderNo, u.Value, u.Code })
                       .ToListAsync();
            }
            else
            {
                dictDataList = await _sysDictDataRep.AsQueryable()
                    .Where(u => u.DictTypeId == dictType.Id)
                    .Where(u => u.Status == StatusEnum.Enable)
                    .OrderBy(u => new { u.OrderNo, u.Value, u.Code })
                    .ToListAsync();
            }

            _sysCacheService.Set(dicKey, dictDataList);
        }
        return dictDataList;
    }

    /// <summary>
    /// Get a dictionary value set based on query conditions 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get a collection of dictionary values ​​based on query conditions")]
    public async Task<List<SysDictData>> GetDataList([FromQuery] QueryDictDataInput input)
    {
        var dataList = await GetDataList(input.Value);
        if (input.Status.HasValue) return dataList.Where(u => u.Status == (StatusEnum)input.Status.Value).ToList();
        return dataList;
    }

    /// <summary>
    /// Delete dictionary value based on dictionary type Id
    /// </summary>
    /// <param name="dictTypeId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task DeleteDictData(long dictTypeId)
    {
        var dictType = await _sysDictDataRep.Change<SysDictType>().AsQueryable().Where(u => u.Id == dictTypeId).FirstAsync();
        Remove(dictType);

        if (dictType?.IsTenant == YesNoEnum.Y)
            await _sysDictDataRep.Change<SysDictDataTenant>().DeleteAsync(u => u.DictTypeId == dictTypeId);
        else
            await _sysDictDataRep.DeleteAsync(u => u.DictTypeId == dictTypeId);
    }

    /// <summary>
    /// Display text Label through dictionary data Value query
    /// Suitable for subqueries that find text in lists based on dictionary data values ​​_sysDictDataService.MapDictValueToLabel(() =>obj.Type, "org_type",obj);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mappingFiled"></param>
    /// <param name="dictTypeCode"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public string MapDictValueToLabel<T>(Expression<Func<object>> mappingFiled, string dictTypeCode, T parameter)
    {
        return VSysDictData.InnerJoin<SysDictType>((d, dt) => d.DictTypeId.Equals(dt.Id) && dt.Code == dictTypeCode)
            .SetContext(d => d.Value, mappingFiled, parameter).FirstOrDefault()?.Label;
    }

    /// <summary>
    /// Display text Label query Value through dictionary data
    /// Suitable for list data import and subquery to find values ​​based on dictionary data text _sysDictDataService.MapDictLabelToValue(() => obj.Type, "org_type",obj);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mappingFiled"></param>
    /// <param name="dictTypeCode"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public string MapDictLabelToValue<T>(Expression<Func<object>> mappingFiled, string dictTypeCode, T parameter)
    {
        return VSysDictData.InnerJoin<SysDictType>((d, dt) => d.DictTypeId.Equals(dt.Id) && dt.Code == dictTypeCode)
            .SetContext(d => d.Label, mappingFiled, parameter).FirstOrDefault()?.Value;
    }

    /// <summary>
    /// Clean dictionary data cache
    /// </summary>
    /// <param name="dictType"></param>
    private void Remove(SysDictType dictType)
    {
        _sysCacheService.Remove($"{CacheConst.KeyDict}{dictType?.Code}");
        _sysCacheService.Remove($"{CacheConst.KeyTenantDict}{_userManager}:{dictType?.Code}");
    }
}