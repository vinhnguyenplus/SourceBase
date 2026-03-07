// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// event subscription
/// </summary>
public class AppEventSubscriber : IEventSubscriber, ISingleton, IDisposable
{
    private static readonly ISugarQueryable<SysTenant> SysTenantQueryable = App.GetService<ISqlSugarClient>().Queryable<SysTenant>();
    private readonly IServiceScope _serviceScope;

    public AppEventSubscriber(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
    }

    /// <summary>
    /// Add exception log
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(CommonConst.AddExLog)]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        // Switch log independent database
        var db = SqlSugarSetup.ITenant.IsAnyConnection(SqlSugarConst.LogConfigId)
            ? SqlSugarSetup.ITenant.GetConnectionScope(SqlSugarConst.LogConfigId)
            : SqlSugarSetup.ITenant.GetConnectionScope(SqlSugarConst.MainConfigId);
        await db.CopyNew().Insertable((SysLogEx)context.Source.Payload).ExecuteCommandAsync();
    }

    /// <summary>
    /// Send abnormal email
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(CommonConst.SendErrorMail)]
    public async Task SendOrderErrorMail(EventHandlerExecutingContext context)
    {
        long.TryParse(App.HttpContext?.User.FindFirst(ClaimConst.TenantId)?.Value, out var tenantId);
        var tenant = await SysTenantQueryable.FirstAsync(t => t.Id == tenantId);
        var title = $"{tenant?.Title} System exception";
        await _serviceScope.ServiceProvider.GetRequiredService<SysEmailService>().SendEmail(JSON.Serialize(context.Source.Payload), title);
    }

    /// <summary>
    /// Release service scope
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}