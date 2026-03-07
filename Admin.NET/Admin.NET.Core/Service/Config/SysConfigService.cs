// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using NewLife.Reflection;

namespace Admin.NET.Core.Service;

/// <summary>
/// Platform parameter configuration service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 440)]
public class SysConfigService : IDynamicApiController, ITransient
{
    private static readonly SysTenantService SysTenantService = App.GetService<SysTenantService>();
    private readonly SqlSugarRepository<SysConfig> _sysConfigRep;
    private readonly SqlSugarRepository<SysTenant> _sysTenantRep;
    private readonly SysCacheService _sysCacheService;
    private readonly UserManager _userManager;

    public SysConfigService(
        SqlSugarRepository<SysTenant> sysTenantRep,
        SqlSugarRepository<SysConfig> sysConfigRep,
        SysCacheService sysCacheService,
        UserManager userManager)
    {
        _sysCacheService = sysCacheService;
        _sysConfigRep = sysConfigRep;
        _sysTenantRep = sysTenantRep;
        _userManager = userManager;
    }

    /// <summary>
    /// Get parameter configuration paginated list 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get parameter configuration paging list")]
    public async Task<SqlSugarPagedList<SysConfig>> Page(PageConfigInput input)
    {
        return await _sysConfigRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code?.Trim()), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupCode?.Trim()), u => u.GroupCode.Equals(input.GroupCode))
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get parameter configuration list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get parameter configuration list")]
    public async Task<List<SysConfig>> List(PageConfigInput input)
    {
        return await _sysConfigRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupCode?.Trim()), u => u.GroupCode.Equals(input.GroupCode))
            .ToListAsync();
    }

    /// <summary>
    /// Add parameter configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Increase parameter configuration")]
    public async Task AddConfig(AddConfigInput input)
    {
        if (input.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3010);

        var isExist = await _sysConfigRep.IsAnyAsync(u => u.Name == input.Name || u.Code == input.Code);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D9000);

        await _sysConfigRep.InsertAsync(input.Adapt<SysConfig>());
    }

    /// <summary>
    /// Update parameter configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Update parameter configuration")]
    public async Task UpdateConfig(UpdateConfigInput input)
    {
        if (input.SysFlag == YesNoEnum.Y && !_userManager.SuperAdmin) throw Oops.Oh(ErrorCodeEnum.D3010);

        var isExist = await _sysConfigRep.IsAnyAsync(u => (u.Name == input.Name || u.Code == input.Code) && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D9000);

        var config = input.Adapt<SysConfig>();
        await _sysConfigRep.AsUpdateable(config).IgnoreColumns(true).ExecuteCommandAsync();

        Remove(config);
    }

    /// <summary>
    /// Delete parameter configuration 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Delete parameter configuration")]
    public async Task DeleteConfig(DeleteConfigInput input)
    {
        var config = await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);

        // Disable deletion of system parameters
        if (config.SysFlag == YesNoEnum.Y)
        { throw Oops.Oh(ErrorCodeEnum.D9001); }
        else
        { await _sysConfigRep.DeleteAsync(config); }

        Remove(config);
    }

    /// <summary>
    /// Delete parameter configurations in batches 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchDelete"), HttpPost]
    [DisplayName("BatchDelete parameter configuration")]
    public async Task BatchDeleteConfig(List<long> ids)
    {
        foreach (var id in ids)
        {
            var config = await _sysConfigRep.GetFirstAsync(u => u.Id == id);

            // Disable deletion of system parameters
            if (config.SysFlag == YesNoEnum.Y) continue;

            await _sysConfigRep.DeleteAsync(config);

            Remove(config);
        }
    }

    /// <summary>
    /// Get parameter configuration details 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get parameter configuration details")]
    public async Task<SysConfig> GetDetail([FromQuery] ConfigInput input)
    {
        return await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Get parameter configuration value
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<T> GetConfigValue<T>(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return default;

        var value = _sysCacheService.Get<string>($"{CacheConst.KeyConfig}{code}");
        if (string.IsNullOrEmpty(value))
        {
            value = (await _sysConfigRep.AsQueryable().FirstAsync(u => u.Code == code))?.Value;
            _sysCacheService.Set($"{CacheConst.KeyConfig}{code}", value);
        }
        if (string.IsNullOrWhiteSpace(value)) return default;
        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// Update parameter configuration values
    /// </summary>
    /// <param name="code"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [NonAction]
    public async Task UpdateConfigValue(string code, string value)
    {
        var config = await _sysConfigRep.AsQueryable().FirstAsync(u => u.Code == code);
        if (config == null) return;

        config.Value = value;
        await _sysConfigRep.AsUpdateable(config).ExecuteCommandAsync();

        Remove(config);
    }

    /// <summary>
    /// Get group list 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get group list")]
    public async Task<List<string>> GetGroupList()
    {
        return await _sysConfigRep.AsQueryable()
            .GroupBy(u => u.GroupCode)
            .Select(u => u.GroupCode).ToListAsync();
    }

    /// <summary>
    /// Get Token expiration time
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetTokenExpire()
    {
        var tokenExpireStr = await GetConfigValue<string>(ConfigConst.SysTokenExpire);
        _ = int.TryParse(tokenExpireStr, out var tokenExpire);
        return tokenExpire == 0 ? 20 : tokenExpire;
    }

    /// <summary>
    /// Get RefreshToken expiration time
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetRefreshTokenExpire()
    {
        var refreshTokenExpireStr = await GetConfigValue<string>(ConfigConst.SysRefreshTokenExpire);
        _ = int.TryParse(refreshTokenExpireStr, out var refreshTokenExpire);
        return refreshTokenExpire == 0 ? 40 : refreshTokenExpire;
    }

    /// <summary>
    /// Batch update parameter configuration values
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchUpdate"), HttpPost]
    [DisplayName("Batch update parameter configuration values")]
    public async Task BatchUpdateConfig(List<BatchConfigInput> input)
    {
        foreach (var config in input)
        {
            var info = await _sysConfigRep.AsQueryable().FirstAsync(c => c.Code == config.Code);
            if (info == null || info.SysFlag == YesNoEnum.Y) continue;

            await _sysConfigRep.AsUpdateable().SetColumns(u => u.Value == config.Value).Where(u => u.Code == config.Code).ExecuteCommandAsync();
            Remove(info);
        }
    }

    /// <summary>
    /// Get system information 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [AllowAnonymous]
    [DisplayName("Get system information")]
    public async Task<dynamic> GetSysInfo()
    {
        var tenant = await SysTenantService.GetCurrentTenantSysInfo();
        var wayList = await _sysConfigRep.Context.Queryable<SysUserRegWay>()
            .Where(u => u.TenantId == tenant.Id)
            .Select(u => new { Label = u.Name, Value = u.Id })
            .ToListAsync();

        var captcha = await GetConfigValue<bool>(ConfigConst.SysCaptcha);
        var secondVer = await GetConfigValue<bool>(ConfigConst.SysSecondVer);
        var hideTenantForLogin = await GetConfigValue<bool>(ConfigConst.SysHideTenantLogin);
        return new
        {
            tenant.Logo,
            tenant.Title,
            tenant.ViceTitle,
            tenant.ViceDesc,
            tenant.Watermark,
            tenant.Copyright,
            tenant.Icp,
            tenant.IcpUrl,
            tenant.RegWayId,
            tenant.EnableReg,
            SecondVer = secondVer ? YesNoEnum.Y : YesNoEnum.N,
            Captcha = captcha ? YesNoEnum.Y : YesNoEnum.N,
            HideTenantForLogin = hideTenantForLogin,
            WayList = wayList
        };
    }

    /// <summary>
    /// Save system information 🔖
    /// </summary>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("saveSystem information")]
    public async Task SaveSysInfo(InfoSaveInput input)
    {
        var tenant = await _sysTenantRep.GetFirstAsync(u => u.Id == _userManager.TenantId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        if (!string.IsNullOrEmpty(input.LogoBase64)) SysTenantService.SetLogoUrl(tenant, input.LogoBase64, input.LogoFileName);
        // await UpdateConfigValue(ConfigConst.SysCaptcha, (input.Captcha == YesNoEnum.Y).ToString());
        // await UpdateConfigValue(ConfigConst.SysSecondVer, (input.SecondVer == YesNoEnum.Y).ToString());

        tenant.Copy(input);
        tenant.RegWayId = input.EnableReg == YesNoEnum.Y ? input.RegWayId : null;
        await _sysTenantRep.Context.Updateable(tenant).ExecuteCommandAsync();
    }

    private void Remove(SysConfig config)
    {
        _sysCacheService.Remove($"{CacheConst.KeyConfig}Value:{config.Code}");
        _sysCacheService.Remove($"{CacheConst.KeyConfig}Remark:{config.Code}");
        _sysCacheService.Remove($"{CacheConst.KeyConfig}{config.GroupCode}:GroupWithCache");
        _sysCacheService.Remove($"{CacheConst.KeyConfig}{config.Code}");
    }
}