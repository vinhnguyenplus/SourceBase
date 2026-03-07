// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core.Service;

/// <summary>
/// job execution monitor
/// </summary>
public class JobMonitor : IJobMonitor
{
    private readonly SysConfigService _sysConfigService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<JobMonitor> _logger;

    public JobMonitor(IServiceScopeFactory serviceScopeFactory, IEventPublisher eventPublisher, ILogger<JobMonitor> logger)
    {
        var serviceScope = serviceScopeFactory.CreateScope();
        _sysConfigService = serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public Task OnExecutingAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public async Task OnExecutedAsync(JobExecutedContext context, CancellationToken stoppingToken)
    {
        if (context.Exception == null) return;

        var exception = $"Scheduled task [{context.Trigger.Description}] error: {context.Exception}";
        // Record job exception information locally
        _logger.LogError(exception);

        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysErrorMail))
        {
            // Send job exception information to email
            await _eventPublisher.PublishAsync(CommonConst.SendErrorMail, exception, stoppingToken);
        }
    }
}