export const JobScriptCode = `#region using

using Furion;
using Furion.Logging;
using Furion.RemoteRequest.Extensions;
using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yitter.IdGenerator;

#endregion

namespace Admin.NET.Core;

/// <summary>
/// Dynamic job tasks
/// </summary>
[JobDetail("Your homework number")]
public class DynamicJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public DynamicJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        
        // Get user repository
        // var rep = serviceScope.ServiceProvider.GetService<SqlSugarRepository<SysUser>>();

        // Request URL
        // var result = await "http://www.baidu.com".GetAsStringAsync();
        // Console.WriteLine(result);

        // log
        // Log.Information("Log message");
    }
}`;
