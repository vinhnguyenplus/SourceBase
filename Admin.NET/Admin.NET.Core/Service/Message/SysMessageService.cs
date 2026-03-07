// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.AspNetCore.SignalR;

namespace Admin.NET.Core.Service;

/// <summary>
/// System message sending service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class SysMessageService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _chatHubContext;

    public SysMessageService(SysCacheService sysCacheService,
        IHubContext<OnlineUserHub, IOnlineUserHub> chatHubContext)
    {
        _sysCacheService = sysCacheService;
        _chatHubContext = chatHubContext;
    }

    /// <summary>
    /// Send message to everyone 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send a message to everyone")]
    public async Task SendAllUser(MessageInput input)
    {
        await _chatHubContext.Clients.All.ReceiveMessage(input);
    }

    /// <summary>
    /// Send a message to someone other than the sender 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send a message to someone other than the sender")]
    public async Task SendOtherUser(MessageInput input)
    {
        var hashKey = _sysCacheService.HashGetAll<SysOnlineUser>(CacheConst.KeyUserOnline);
        var exceptReceiveUsers = hashKey.Where(u => u.Value.UserId == input.ReceiveUserId).Select(u => u.Value).ToList();
        await _chatHubContext.Clients.AllExcept(exceptReceiveUsers.Select(t => t.ConnectionId)).ReceiveMessage(input);
    }

    /// <summary>
    /// Send a message to someone 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send a message to someone")]
    public async Task SendUser(MessageInput input)
    {
        var hashKey = _sysCacheService.HashGetAll<SysOnlineUser>(CacheConst.KeyUserOnline);
        var receiveUsers = hashKey.Where(u => u.Value.UserId == input.ReceiveUserId).Select(u => u.Value).ToList();
        await receiveUsers.ForEachAsync(u => _chatHubContext.Clients.Client(u.ConnectionId ?? "").ReceiveMessage(input));
    }

    /// <summary>
    /// Send a message to someone 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("Send a message to someone")]
    public async Task SendUsers(MessageInput input)
    {
        var hashKey = _sysCacheService.HashGetAll<SysOnlineUser>(CacheConst.KeyUserOnline);
        var receiveUsers = hashKey.Where(u => input.UserIds.Any(a => a == u.Value.UserId)).Select(u => u.Value).ToList();
        await receiveUsers.ForEachAsync(u => _chatHubContext.Clients.Client(u.ConnectionId ?? "").ReceiveMessage(input));
    }
}