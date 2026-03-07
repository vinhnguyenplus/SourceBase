// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Clean log job task
/// </summary>
[JobDetail("job_log", Description = "Clean operation log", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_log", Description = "Clean operation log")]
public class LogJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public LogJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        var db = serviceScope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();
        var sysConfigService = serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();

        var daysAgo = await sysConfigService.GetConfigValue<int>(ConfigConst.SysLogRetentionDays); // Log retention days
        await db.Deleteable<SysLogVis>().Where(u => u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // Delete access log
        await db.Deleteable<SysLogOp>().Where(u => u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // Delete operation log
        await db.Deleteable<SysLogDiff>().Where(u => u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // Delete differential log

        string msg = $"[{DateTime.Now}] System log clearing was successful, log data from {daysAgo} days ago was deleted!";
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ForegroundColor = originColor;

        // Custom log
        _logger.LogInformation(msg);
    }
}