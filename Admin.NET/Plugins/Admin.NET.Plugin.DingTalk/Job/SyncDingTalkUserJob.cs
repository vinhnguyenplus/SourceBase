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
/// Synchronize DingTalk user jobs
/// </summary>
[JobDetail("SyncDingTalkUserJob", Description = "Synchronize DingTalk users", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "SyncDingTalkUserTrigger", Description = "Synchronize DingTalk users")]
public class SyncDingTalkUserJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly ILogger _logger;

    public SyncDingTalkUserJob(IServiceScopeFactory scopeFactory, IDingTalkApi dingTalkApi, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _dingTalkApi = dingTalkApi;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var _sysUserRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysUser>>();
        var _dingTalkUserRepo = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<DingTalkUser>>();
        var _dingTalkOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<DingTalkOptions>>();

        // Get Token
        var tokenRes = await _dingTalkApi.GetDingTalkToken(_dingTalkOptions.Value.ClientId, _dingTalkOptions.Value.ClientSecret);
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var dingTalkUserList = new List<DingTalkEmpRosterFieldVo>();
        var offset = 0;
        while (offset >= 0)
        {
            // Get list of user IDs
            var userIdsRes = await _dingTalkApi.GetDingTalkCurrentEmployeesList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesListInput
            {
                StatusList = "2,3,5,-1",
                Size = 50,
                Offset = offset
            });
            if (!userIdsRes.Success)
            {
                _logger.LogError(userIdsRes.ErrMsg);
                break;
            }
            // Get roster based on userId
            var rosterRes = await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(
                tokenRes.AccessToken,
                new GetDingTalkCurrentEmployeesRosterListInput()
                {
                    UserIdList = string.Join(",", userIdsRes.Result.DataList),
                    FieldFilterList =
                        $"{DingTalkConst.NameField},{DingTalkConst.JobNumberField},{DingTalkConst.MobileField},{DingTalkConst.DeptId},{DingTalkConst.Dept},{DingTalkConst.Position}",
                    AgentId = _dingTalkOptions.Value.AgentId
                }
            );
            if (!rosterRes.Success)
            {
                _logger.LogError(rosterRes.ErrMsg);
                break;
            }
            dingTalkUserList.AddRange(rosterRes.Result);
            if (userIdsRes.Result.NextCursor == null)
            {
                break;
            }
            // Save paging cursor
            offset = (int)userIdsRes.Result.NextCursor;
        }

        // Determine whether to add or update
        var sysDingTalkUserIdList = await _dingTalkUserRepo.AsQueryable().Select(u => new
        {
            u.Id,
            u.DingTalkUserId
        }).ToListAsync();

        var uDingTalkUser = dingTalkUserList.Where(u => sysDingTalkUserIdList.Any(m => m.DingTalkUserId == u.UserId)); // User ID that needs to be updated
        var iDingTalkUser = dingTalkUserList.Where(u => !sysDingTalkUserIdList.Any(m => m.DingTalkUserId == u.UserId)); // Need to add new user ID

        // Add new DingTalk user
        var iUser = iDingTalkUser.Select(res => new DingTalkUser
        {
            DingTalkUserId = res.UserId,
            Name = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.NameField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Mobile = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.MobileField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            JobNumber = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.JobNumberField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            DeptId = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.DeptId).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Dept = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.Dept).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Position = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.Position).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
        }).ToList();
        if (iUser.Count > 0)
        {
            await _dingTalkUserRepo.CopyNew().AsInsertable(iUser).ExecuteCommandAsync();
        }

        // Update DingTalk users
        var uUser = uDingTalkUser.Select(res => new DingTalkUser
        {
            Id = sysDingTalkUserIdList.Where(u => u.DingTalkUserId == res.UserId).Select(u => u.Id).FirstOrDefault(),
            DingTalkUserId = res.UserId,
            Name = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.NameField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Mobile = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.MobileField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            JobNumber = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.JobNumberField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            DeptId = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.DeptId).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Dept = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.Dept).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Position = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.Position).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
        }).ToList();
        if (uUser.Count > 0)
        {
            await _dingTalkUserRepo.CopyNew().AsUpdateable(uUser).UpdateColumns(u => new
            {
                u.DingTalkUserId,
                u.Name,
                u.Mobile,
                u.JobNumber,
                u.DeptId,
                u.Dept,
                u.Position,
                u.UpdateTime,
                u.UpdateUserName,
                u.UpdateUserId,
            }).ExecuteCommandAsync();
        }

        // Update the system user ID in the DingTalk user table through the system user account (employee number)
        var sysUser = await _sysUserRep.AsQueryable()
            .Select(u => new
            {
                u.Id,
                u.Account
            }).ToListAsync();
        var sysDingTalkUser = await _dingTalkUserRepo.AsQueryable()
            .Where(u => sysUser.Any(m => m.Account == u.JobNumber))
            .Select(u => new
            {
                u.Id,
                u.JobNumber,
                u.Mobile,
                u.DeptId,
                u.Dept,
                u.Position
            }).ToListAsync();
        var uSysDingTalkUser = sysDingTalkUser.Select(u => new DingTalkUser
        {
            Id = u.Id,
            SysUserId = sysUser.Where(m => m.Account == u.JobNumber).Select(u => u.Id).FirstOrDefault(),
        }).ToList();

        await _dingTalkUserRepo.CopyNew().AsUpdateable(uSysDingTalkUser).UpdateColumns(u => new
        {
            u.SysUserId,
            u.UpdateTime,
            u.UpdateUserName,
            u.UpdateUserId,
        }).ExecuteCommandAsync();

        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("【" + DateTime.Now + "】Synchronized DingTalk users");
        Console.ForegroundColor = originColor;
    }
}