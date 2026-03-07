// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Plugin.DingTalk;
using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Plugin.Job;

/// <summary>
/// To synchronize DingTalk role jobs, please set the automatic synchronization trigger according to your needs on the web page.
/// </summary>
[JobDetail("SyncDingTalkDeptJob", Description = "Synchronous DingTalk Department", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "SyncDingTalkDeptTrigger", Description = "Synchronous DingTalk Department")]
public class SyncDingTalkDeptJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly ILogger _logger;
    private readonly SqlSugarRepository<DingTalkDept> _dingTalkDeptRep;

    public SyncDingTalkDeptJob(
        IServiceScopeFactory scopeFactory,
        IDingTalkApi dingTalkApi,
        SqlSugarRepository<DingTalkDept> dingTalkDeptRep,
        ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _dingTalkApi = dingTalkApi;
        _dingTalkDeptRep = dingTalkDeptRep;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var _dingTalkOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<DingTalkOptions>>();

        // Get Token
        var tokenRes = await _dingTalkApi.GetDingTalkToken(_dingTalkOptions.Value.ClientId, _dingTalkOptions.Value.ClientSecret);
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var dingTalkDeptList = new List<DingTalkDept>();
        // Get department list
        var deptIdsRes = await _dingTalkApi.GetDingTalkDept(tokenRes.AccessToken, new GetDingTalkDeptInput
        { dept_id = 1 });
        if (deptIdsRes.ErrCode != 0)
        {
            _logger.LogError(deptIdsRes.ErrMsg);
            throw Oops.Oh(deptIdsRes.ErrMsg);
        }
        dingTalkDeptList.AddRange(deptIdsRes.Result.Select(d => new DingTalkDept
        {
            dept_id = d.dept_id,
            name = d.name,
            parent_id = d.parent_id
        }));
        foreach (var item in deptIdsRes.Result)
        {
            dingTalkDeptList.AddRange(await GetDingTalkDeptList(tokenRes.AccessToken, item.dept_id));
        }
        await _dingTalkDeptRep.InsertOrUpdateAsync(dingTalkDeptList);
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("【" + DateTime.Now + "】Synchronous DingTalk Department");
        Console.ForegroundColor = originColor;
    }

    private async Task<List<DingTalkDept>> GetDingTalkDeptList(string token, long dept_id)
    {
        List<DingTalkDept> listTemp = new List<DingTalkDept>();
        var deptIdsRes = await _dingTalkApi.GetDingTalkDept(token, new GetDingTalkDeptInput
        { dept_id = dept_id });
        if (deptIdsRes.ErrCode != 0)
        {
            return null;
        }
        listTemp.AddRange(deptIdsRes.Result.Select(x => new DingTalkDept
        {
            dept_id = x.dept_id,
            name = x.name,
            parent_id = x.parent_id
        }));
        foreach (var item in deptIdsRes.Result)
        {
            listTemp.AddRange(await GetDingTalkDeptList(token, item.dept_id));
        }
        return listTemp;
    }
}