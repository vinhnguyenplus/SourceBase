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
[JobDetail("SyncDingTalkRoleJob", Description = "Synchronize DingTalk Roles", GroupName = "default", Concurrent = false)]
public class SyncDingTalkRoleJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly ILogger _logger;

    public SyncDingTalkRoleJob(IServiceScopeFactory scopeFactory, IDingTalkApi dingTalkApi, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _dingTalkApi = dingTalkApi;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var _dingTalkRoleRepo = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<DingTalkRoleUser>>();
        var _dingTalkOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<DingTalkOptions>>();

        // Get Token
        var tokenRes = await _dingTalkApi.GetDingTalkToken(_dingTalkOptions.Value.ClientId, _dingTalkOptions.Value.ClientSecret);
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var dingTalkRoleUserList = new List<DingTalkRoleUser>();
        // Get role list
        var roleIdsRes = await _dingTalkApi.GetDingTalkRoleList(tokenRes.AccessToken, new GetDingTalkCurrentRoleListInput
        { });
        if (roleIdsRes.Success)
        {
            _logger.LogError(roleIdsRes.ErrMsg);
            throw Oops.Oh(roleIdsRes.ErrMsg);
        }
        foreach (var item in roleIdsRes.Result.list)
        {
            foreach (var role_item in item.roles)
            {
                // Get the list of employees in a specified role based on the role id
                var role_user = await _dingTalkApi.GetDingTalkRoleSimplelist(
                    tokenRes.AccessToken,
                    new GetDingTalkCurrentRoleSimplelistInput()
                    {
                        role_id = role_item.id,
                    }
                );

                if (role_user.Success)
                {
                    _logger.LogError(role_user.ErrMsg);
                    break;
                }
                var tempList = role_user.Result.list.Select(u => new DingTalkRoleUser
                {
                    DingTalkUserId = u.userid,
                    groupId = item.groupId,
                    groupName = item.name,
                    roleId = role_item.id,
                    roleName = role_item.name
                }).ToList();
                if (tempList?.Count > 0)
                {
                    dingTalkRoleUserList.AddRange(tempList);
                }
            }
        }

        // Determine whether to add or update
        var sysDingTalkRoleList = await _dingTalkRoleRepo.AsQueryable().ToListAsync();
        // User ID that needs to be updated
        var uDingTalkRole = dingTalkRoleUserList.Where(u => sysDingTalkRoleList.Any(m => m.DingTalkUserId == u.DingTalkUserId && m.groupId == u.groupId));
        // Need to add new user ID
        var iDingTalkRole = dingTalkRoleUserList.Where(u => !sysDingTalkRoleList.Any(m => m.DingTalkUserId == u.DingTalkUserId && m.groupId == u.groupId));
        // Data that needs to be deleted
        var dDingTalkRole = sysDingTalkRoleList.Where(u => !dingTalkRoleUserList.Any(m => m.DingTalkUserId == u.DingTalkUserId && m.groupId == u.groupId)).ToList();
        // Added DingTalk character
        var iUser = iDingTalkRole.Select(res => new DingTalkRoleUser
        {
            DingTalkUserId = res.DingTalkUserId,
            groupId = res.groupId,
            groupName = res.groupName,
            roleId = res.roleId,
            roleName = res.roleName,
        }).ToList();
        if (iUser.Count > 0)
        {
            await _dingTalkRoleRepo.CopyNew().AsInsertable(iUser).ExecuteCommandAsync();
        }

        // Update DingTalk character
        var uUser = uDingTalkRole.Select(res => new DingTalkRoleUser
        {
            Id = sysDingTalkRoleList.Where(u => u.DingTalkUserId == res.DingTalkUserId).Select(u => u.Id).FirstOrDefault(),
            DingTalkUserId = res.DingTalkUserId,
            groupId = res.groupId,
            groupName = res.groupName,
            roleId = res.roleId,
            roleName = res.roleName
        }).ToList();
        //Add data that needs to be deleted
        Parallel.ForEach(dDingTalkRole, user => user.IsDelete = true);
        uUser.AddRange(dDingTalkRole);
        if (uUser.Count > 0)
        {
            await _dingTalkRoleRepo.CopyNew().AsUpdateable(uUser).UpdateColumns(u => new
            {
                u.DingTalkUserId,
                u.groupId,
                u.groupName,
                u.roleId,
                u.roleName,
                u.UpdateTime,
                u.UpdateUserName,
                u.UpdateUserId,
                u.IsDelete
            }).ExecuteCommandAsync();
        }

        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("【" + DateTime.Now + "】Synchronized DingTalk characters");
        Console.ForegroundColor = originColor;
    }
}