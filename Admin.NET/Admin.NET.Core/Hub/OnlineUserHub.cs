// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion.InstantMessaging;
using Microsoft.AspNetCore.SignalR;

namespace Admin.NET.Core;

/// <summary>
/// Online User Hub
/// </summary>
[MapHub("/hubs/onlineUser")]
public class OnlineUserHub : Hub<IOnlineUserHub>
{
    private const string GROUP_ONLINE = "GROUP_ONLINE_"; // Tenant group prefix

    private readonly SqlSugarRepository<SysOnlineUser> _sysOnlineUerRep;
    private readonly SysMessageService _sysMessageService;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _onlineUserHubContext;
    private readonly SysCacheService _sysCacheService;
    private readonly SysConfigService _sysConfigService;

    public OnlineUserHub(SqlSugarRepository<SysOnlineUser> sysOnlineUerRep,
        SysMessageService sysMessageService,
        IHubContext<OnlineUserHub, IOnlineUserHub> onlineUserHubContext,
        SysCacheService sysCacheService,
        SysConfigService sysConfigService)
    {
        _sysOnlineUerRep = sysOnlineUerRep;
        _sysMessageService = sysMessageService;
        _onlineUserHubContext = onlineUserHubContext;
        _sysCacheService = sysCacheService;
        _sysConfigService = sysConfigService;
    }

    /// <summary>
    /// connect
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = (httpContext.User.FindFirst(ClaimConst.UserId)?.Value).ToLong();
        var account = httpContext.User.FindFirst(ClaimConst.Account)?.Value;
        var realName = httpContext.User.FindFirst(ClaimConst.RealName)?.Value;
        var tenantId = (httpContext.User.FindFirst(ClaimConst.TenantId)?.Value).ToLong();

        if (userId < 0 || string.IsNullOrWhiteSpace(account)) return;
        var user = new SysOnlineUser
        {
            ConnectionId = Context.ConnectionId,
            UserId = userId,
            UserName = account,
            RealName = realName,
            Time = DateTime.Now,
            Ip = httpContext.GetRemoteIpAddressToIPv4(true),
            Browser = httpContext.GetClientBrowser(),
            Os = httpContext.GetClientOs(),
            TenantId = tenantId,
        };
        await _sysOnlineUerRep.InsertAsync(user);

        // Whether to enable single user login
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysSingleLogin))
        {
            _sysCacheService.HashAddOrUpdate(CacheConst.KeyUserOnline, "" + user.UserId, user);
        }
        else  // For non-single user login, the user connection ID is bound.
        {
            _sysCacheService.HashAdd(CacheConst.KeyUserOnline, user.UserId + Context.ConnectionId, user);
        }

        // Group by tenant ID
        var groupName = $"{GROUP_ONLINE}{user.TenantId}";
        await _onlineUserHubContext.Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var userList = await _sysOnlineUerRep.AsQueryable().Filter("", true)
            .Where(u => u.TenantId == user.TenantId).Take(10).ToListAsync();

        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysLoginOutReminder))
            await _onlineUserHubContext.Clients.Groups(groupName).OnlineUserList(new OnlineUserList
            {
                RealName = user.RealName,
                Online = true,
                UserList = userList
            });
    }

    /// <summary>
    /// disconnect
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (string.IsNullOrEmpty(Context.ConnectionId)) return;

        var httpContext = Context.GetHttpContext();

        var user = await _sysOnlineUerRep.AsQueryable().Filter("", true).FirstAsync(u => u.ConnectionId == Context.ConnectionId);
        if (user == null) return;

        await _sysOnlineUerRep.DeleteByIdAsync(user.Id);

        // Whether to enable single user login
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysSingleLogin))
        {
            _sysCacheService.HashDel<SysOnlineUser>(CacheConst.KeyUserOnline, "" + user.UserId);
            // _sysCacheService.Remove(CacheConst.KeyUserOnline + user.UserId);
        }
        else
        {
            _sysCacheService.HashDel<SysOnlineUser>(CacheConst.KeyUserOnline, user.UserId + Context.ConnectionId);
            // _sysCacheService.Remove(CacheConst.KeyUserOnline + user.UserId + Context.ConnectionId);
        }

        // Notify current group users of changes
        var userList = await _sysOnlineUerRep.AsQueryable().Filter("", true)
            .Where(u => u.TenantId == user.TenantId).Take(10).ToListAsync();

        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysLoginOutReminder))
            await _onlineUserHubContext.Clients.Groups($"{GROUP_ONLINE}{user.TenantId}").OnlineUserList(new OnlineUserList
            {
                RealName = user.RealName,
                Online = false,
                UserList = userList
            });
    }

    /// <summary>
    /// Forced offline
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task ForceOffline(OnlineUserHubInput input)
    {
        await _onlineUserHubContext.Clients.Client(input.ConnectionId).ForceOffline("Forced offline");
    }

    /// <summary>
    /// Send a message to someone
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessage(MessageInput message)
    {
        await _sysMessageService.SendUser(message);
    }

    /// <summary>
    /// Send message to everyone
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessageToAll(MessageInput message)
    {
        await _sysMessageService.SendAllUser(message);
    }

    /// <summary>
    /// Send a message to someone (other than me)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessageToOther(MessageInput message)
    {
        await _sysMessageService.SendOtherUser(message);
    }

    /// <summary>
    /// Send a message to someone
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientsSendMessageToUsers(MessageInput message)
    {
        await _sysMessageService.SendUsers(message);
    }
}