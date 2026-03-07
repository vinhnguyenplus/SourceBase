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
[JobDetail(
    "SyncWokerflowLogJob",
    Description = "Synchronize DingTalk approval status",
    GroupName = "default",
    Concurrent = false
)]
[Daily(TriggerId = "SyncWokerflowLogTrigger", Description = "Synchronize DingTalk approval status")]
public class SyncWokerflowLogJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly ILogger _logger;
    private readonly SqlSugarRepository<DingTalkDept> _dingTalkDeptRep;
    private readonly SqlSugarRepository<DingTalkWokerflowLog> _dingTalkWokerflowLogRep;

    public SyncWokerflowLogJob(
        IServiceScopeFactory scopeFactory,
        IDingTalkApi dingTalkApi,
        SqlSugarRepository<DingTalkDept> dingTalkDeptRep,
        SqlSugarRepository<DingTalkWokerflowLog> dingTalkWokerflowLogRep,
        ILoggerFactory loggerFactory
    )
    {
        _scopeFactory = scopeFactory;
        _dingTalkApi = dingTalkApi;
        _dingTalkDeptRep = dingTalkDeptRep;
        _dingTalkWokerflowLogRep = dingTalkWokerflowLogRep;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var _dingTalkOptions = serviceScope.ServiceProvider.GetRequiredService<
            IOptions<DingTalkOptions>
        >();

        // Get Token
        var tokenRes = await _dingTalkApi.GetDingTalkToken(
            _dingTalkOptions.Value.ClientId,
            _dingTalkOptions.Value.ClientSecret
        );
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var dingTalkDeptList = new List<DingTalkDept>();
        // Get the list of outstanding approvals
        List<DingTalkWokerflowLog> flow_list = await _dingTalkWokerflowLogRep.GetListAsync(t =>
            t.Status == "RUNNING"
        );
        List<DingTalkWokerflowLog> update_list = new List<DingTalkWokerflowLog>();
        if (flow_list?.Count > 0)
        {
            foreach (var item in flow_list)
            {
                var flow = await _dingTalkApi.GetProcessInstances(
                    tokenRes.AccessToken,
                    item.instanceId
                );
                if (flow.Result.Status != item.Status)
                {
                    item.Status = flow.Result.Status;
                    item.UpdateTime = DateTime.Now;
                    item.WorkflowId = flow.Result.BusinessId;
                    item.taskId = flow
                        .Result.Tasks.FirstOrDefault(t => t.Status == "RUNNING")
                        ?.TaskId;
                    update_list.Add(item);
                }
            }

            if (update_list.Count > 0)
            {
                await _dingTalkWokerflowLogRep.UpdateRangeAsync(update_list);
            }
            var originColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("【" + DateTime.Now + "】Synchronize DingTalk approval record status");
            Console.ForegroundColor = originColor;
        }
    }
}