// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// System message template service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 305)]
public class SysTemplateService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysTemplate> _sysTemplateRep;
    private readonly UserManager _userManager;
    private readonly IViewEngine _viewEngine;

    public SysTemplateService(
        SqlSugarRepository<SysTemplate> sysTemplateRep,
        IViewEngine viewEngine,
        UserManager userManager)
    {
        _sysTemplateRep = sysTemplateRep;
        _userManager = userManager;
        _viewEngine = viewEngine;
    }

    /// <summary>
    /// Get template list 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings]
    [DisplayName("Get template list")]
    public async Task<SqlSugarPagedList<SysTemplate>> Page(PageTemplateInput input)
    {
        return await _sysTemplateRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupName), u => u.GroupName.Contains(input.GroupName))
            .WhereIF(_userManager.SuperAdmin && input.TenantId > 0, u => u.TenantId == input.TenantId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(input.Type.HasValue, u => u.Type == input.Type)
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// Get the template 📑
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [DisplayName("Get template")]
    [ApiDescriptionSettings]
    public async Task<SysTemplate> GetTemplate(string code)
    {
        return await _sysTemplateRep.GetFirstAsync(u => u.Name == code);
    }

    /// <summary>
    /// Preview template content 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Preview template content")]
    [ApiDescriptionSettings]
    public async Task<string> ProView(ProViewTemplateInput input)
    {
        var template = await _sysTemplateRep.GetFirstAsync(u => u.Id == input.Id);
        return await RenderAsync(template.Content, input.Data);
    }

    /// <summary>
    /// Add template 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Add template")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task AddTemplate(AddTemplateInput input)
    {
        var isExist = await _sysTemplateRep.IsAnyAsync(u => u.Name == input.Name);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.T1000);

        isExist = await _sysTemplateRep.IsAnyAsync(u => u.Code == input.Code);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.T1001);

        await _sysTemplateRep.InsertAsync(input.Adapt<SysTemplate>());
    }

    /// <summary>
    /// Update template 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Update template")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task UpdateTemplate(UpdateTemplateInput input)
    {
        var isExist = await _sysTemplateRep.IsAnyAsync(u => u.Name == input.Name && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.T1000);

        isExist = await _sysTemplateRep.IsAnyAsync(u => u.Code == input.Code && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.T1001);

        await _sysTemplateRep.AsUpdateable(input.Adapt<SysTemplate>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// Delete template 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Delete template")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task DeleteTemplate(BaseIdInput input)
    {
        await _sysTemplateRep.DeleteAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// Get group list 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings]
    [DisplayName("Get group list")]
    public async Task<List<string>> GetGroupList()
    {
        return await _sysTemplateRep.AsQueryable()
            .GroupBy(u => u.GroupName)
            .Select(u => u.GroupName).ToListAsync();
    }

    /// <summary>
    /// Render template content 📑
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Render template content")]
    [ApiDescriptionSettings, HttpPost]
    public async Task<string> Render(RenderTemplateInput input)
    {
        return await RenderAsync(input.Content, input.Data);
    }

    /// <summary>
    /// Render template content 📑
    /// </summary>
    /// <param name="content"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<string> RenderAsync(string content, object data)
    {
        return await _viewEngine.RunCompileFromCachedAsync(Regex.Replace(content, "@\\((.*?)\\)", "@(Model.$1)"), data, builderAction: builder =>
        {
            builder.AddAssemblyReferenceByName("System.Text.RegularExpressions");
            builder.AddAssemblyReferenceByName("System.Collections");
            builder.AddAssemblyReferenceByName("System.Linq");

            builder.AddUsing("System.Text.RegularExpressions");
            builder.AddUsing("System.Collections.Generic");
            builder.AddUsing("System.Linq");
        });
    }

    /// <summary>
    /// Render template content based on encoding
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<string> RenderByCode(string code, Dictionary<string, object> data)
    {
        var template = await _sysTemplateRep.GetFirstAsync(u => u.Code == code);
        return await RenderAsync(template.Content, data);
    }
}