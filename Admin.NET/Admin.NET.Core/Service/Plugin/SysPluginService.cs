// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System dynamic plug-in service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 245)]
public class SysPluginService : IDynamicApiController, ITransient
{
    private readonly IDynamicApiRuntimeChangeProvider _provider;
    private readonly SqlSugarRepository<SysPlugin> _sysPluginRep;
    private readonly UserManager _userManager;

    public SysPluginService(IDynamicApiRuntimeChangeProvider provider,
        SqlSugarRepository<SysPlugin> sysPluginRep,
        UserManager userManager)
    {
        _provider = provider;
        _userManager = userManager;
        _sysPluginRep = sysPluginRep;
    }

    /// <summary>
    /// Get dynamic plugin list 🧩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Get dynamic plugin list")]
    public async Task<SqlSugarPagedList<SysPlugin>> Page(PagePluginInput input)
    {
        return await _sysPluginRep.AsQueryable()
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Add dynamic plug-in 🧩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("Add dynamic plug-in")]
    public async Task AddPlugin(AddPluginInput input)
    {
        var isExist = await _sysPluginRep.IsAnyAsync(u => u.Name == input.Name || u.AssemblyName == input.AssemblyName);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1900);

        // Add dynamic assembly/interface
        input.AssemblyName = CompileAssembly(input.CsharpCode, input.AssemblyName);

        await _sysPluginRep.InsertAsync(input.Adapt<SysPlugin>());
    }

    /// <summary>
    /// Update dynamic plugin 🧩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("Updatedynamic plug-in")]
    public async Task UpdatePlugin(UpdatePluginInput input)
    {
        var isExist = await _sysPluginRep.IsAnyAsync(u => (u.Name == input.Name || u.AssemblyName == input.AssemblyName) && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1900);

        // Remove and then add dynamic assemblies/interfaces
        RemoveAssembly(input.AssemblyName);
        input.AssemblyName = CompileAssembly(input.CsharpCode);

        await _sysPluginRep.AsUpdateable(input.Adapt<SysPlugin>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete dynamic plugin 🧩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Remove dynamic plugin")]
    public async Task DeletePlugin(DeletePluginInput input)
    {
        var plugin = await _sysPluginRep.GetByIdAsync(input.Id);
        if (plugin == null) return;

        // Remove dynamic assembly/interface
        RemoveAssembly(plugin.AssemblyName);

        await _sysPluginRep.DeleteAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Add dynamic assembly/interface 🧩
    /// </summary>
    /// <param name="csharpCode"></param>
    /// <param name="assemblyName">Assembly name</param>
    /// <returns></returns>
    [DisplayName("Add dynamic assembly/interface")]
    public string CompileAssembly([FromBody] string csharpCode, [FromQuery] string assemblyName = default)
    {
        // Compile C# code and return dynamic assembly
        var dynamicAssembly = App.CompileCSharpClassCode(csharpCode, assemblyName);

        // Add an assembly into a dynamic WebAPI application part
        _provider.AddAssembliesWithNotifyChanges(dynamicAssembly);

        // Returns dynamic assembly name
        return dynamicAssembly.GetName().Name;
    }

    /// <summary>
    /// Remove dynamic assembly/interface 🧩
    /// </summary>
    [ApiDescriptionSettings(Name = "RemoveAssembly"), HttpPost]
    [DisplayName("Remove dynamic assembly/interface")]
    public void RemoveAssembly(string assemblyName)
    {
        _provider.RemoveAssembliesWithNotifyChanges(assemblyName);
    }
}