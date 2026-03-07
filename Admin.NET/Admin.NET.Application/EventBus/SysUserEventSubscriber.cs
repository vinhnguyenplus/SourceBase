// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion.EventBus;

namespace Admin.NET.Application;

/// <summary>
/// System user operation event subscription
/// </summary>
public class SysUserEventSubscriber : IEventSubscriber, ISingleton, IDisposable
{
    public SysUserEventSubscriber()
    {
    }

    /// <summary>
    /// Add system users
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.Add)]
    public Task AddUser(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Registered user
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.Register)]
    public Task RegisterUser(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update system user
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.Update)]
    public Task UpdateUser(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete system user
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.Delete)]
    public Task DeleteUser(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Set system user status
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.SetStatus)]
    public Task SetUserStatus(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Authorized user roles
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.UpdateRole)]
    public Task UpdateUserRole(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Unlock login
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysUserEventTypeEnum.UnlockLogin)]
    public Task UnlockUserLogin(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Release service scope
    /// </summary>
    public void Dispose()
    {
    }
}