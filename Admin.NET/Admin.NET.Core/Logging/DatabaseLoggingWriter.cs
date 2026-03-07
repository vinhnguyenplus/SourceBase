// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Database log writer
/// </summary>
public class DatabaseLoggingWriter : IDatabaseLoggingWriter, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly ISqlSugarClient _db;
    private readonly SysConfigService _sysConfigService; // Parameter configuration service
    private readonly ILogger<DatabaseLoggingWriter> _logger; // Log component

    public DatabaseLoggingWriter(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
        //_db = _serviceScope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        _sysConfigService = _serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
        _logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<DatabaseLoggingWriter>>();

        // Switch log independent database
        _db = SqlSugarSetup.ITenant.IsAnyConnection(SqlSugarConst.LogConfigId)
            ? SqlSugarSetup.ITenant.GetConnectionScope(SqlSugarConst.LogConfigId)
            : SqlSugarSetup.ITenant.GetConnectionScope(SqlSugarConst.MainConfigId);
    }

    public async Task WriteAsync(LogMessage logMsg, bool flush)
    {
        var jsonStr = logMsg.Context?.Get("loggingMonitor")?.ToString();
        if (string.IsNullOrWhiteSpace(jsonStr))
        {
            await _db.Insertable(new SysLogOp
            {
                DisplayTitle = "Custom Operation Log",
                LogDateTime = logMsg.LogDateTime,
                EventId = logMsg.EventId.Id,
                ThreadId = logMsg.ThreadId,
                TraceId = logMsg.TraceId,
                Exception = logMsg.Exception == null ? null : JSON.Serialize(logMsg.Exception),
                Message = logMsg.Message,
                LogLevel = logMsg.LogLevel,
                Status = "200",
            }).ExecuteCommandAsync();
            return;
        }

        var loggingMonitor = JSON.Deserialize<dynamic>(jsonStr);
        // Record data verification log
        if (loggingMonitor.validation != null && !await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysValidationLog)) return;

        // Get the current operator
        string account = "", realName = "", userId = "", tenantId = "";
        if (loggingMonitor.authorizationClaims != null)
        {
            var map = (loggingMonitor.authorizationClaims as IEnumerable<dynamic>)
                !.ToDictionary(u => u.type.ToString(), u => u.value.ToString());
            account = map.GetValueOrDefault(ClaimConst.Account);
            realName = map.GetValueOrDefault(ClaimConst.RealName);
            tenantId = map.GetValueOrDefault(ClaimConst.TenantId);
            userId = map.GetValueOrDefault(ClaimConst.UserId);
        }

        // Prioritize obtaining the IP address carried by the X-Forwarded-For header information (such as nginx proxy configuration forwarding)
        var remoteIPv4 = ((JArray)loggingMonitor.requestHeaders).OfType<JObject>()
            .FirstOrDefault(header => (string)header["key"] == "X-Forwarded-For")?["value"]?.ToString();

        if (string.IsNullOrEmpty(remoteIPv4))
            remoteIPv4 = loggingMonitor.remoteIPv4;

        remoteIPv4 = remoteIPv4?.Split(',')?.FirstOrDefault()?.Trim();

        (string ipLocation, double? longitude, double? latitude) = CommonUtil.GetIpAddress(remoteIPv4);

        var browser = "";
        var os = "";
        if (loggingMonitor.userAgent != null)
        {
            var client = Parser.GetDefault().Parse(loggingMonitor.userAgent.ToString());
            browser = $"{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}";
            os = $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}";
        }

        // Catch exceptions, otherwise the program will crash due to unhandled exception
        try
        {
            // Record exception log-send email
            if (logMsg.Exception != null || loggingMonitor.exception != null)
            {
                await _db.Insertable(new SysLogEx
                {
                    ControllerName = loggingMonitor.controllerName,
                    ActionName = loggingMonitor.actionTypeName,
                    DisplayTitle = loggingMonitor.displayTitle,
                    Status = loggingMonitor.returnInformation?.httpStatusCode,
                    RemoteIp = remoteIPv4,
                    Location = ipLocation,
                    Longitude = (decimal?)longitude,
                    Latitude = (decimal?)latitude,
                    Browser = browser, // loggingMonitor.userAgent,
                    Os = os, // loggingMonitor.osDescription + " " + loggingMonitor.osArchitecture,
                    Elapsed = loggingMonitor.timeOperationElapsedMilliseconds,
                    LogDateTime = logMsg.LogDateTime,
                    Account = account,
                    RealName = realName,
                    HttpMethod = loggingMonitor.httpMethod,
                    RequestUrl = loggingMonitor.requestUrl,
                    RequestParam = (loggingMonitor.parameters == null || loggingMonitor.parameters.Count == 0) ? null : JSON.Serialize(loggingMonitor.parameters[0].value),
                    ReturnResult = loggingMonitor.returnInformation == null ? null : JSON.Serialize(loggingMonitor.returnInformation),
                    EventId = logMsg.EventId.Id,
                    ThreadId = logMsg.ThreadId,
                    TraceId = logMsg.TraceId,
                    Exception = JSON.Serialize(loggingMonitor.exception),
                    Message = logMsg.Message,
                    CreateUserId = string.IsNullOrWhiteSpace(userId) ? 0 : long.Parse(userId),
                    TenantId = string.IsNullOrWhiteSpace(tenantId) ? 0 : long.Parse(tenantId),
                    LogLevel = logMsg.LogLevel
                }).ExecuteCommandAsync();

                // Send exception log to email
                if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysErrorMail))
                {
                    await App.GetRequiredService<IEventPublisher>().PublishAsync(CommonConst.SendErrorMail, logMsg.Exception ?? loggingMonitor.exception);
                }

                return;
            }

            // Record access log-login and logout
            if (loggingMonitor.actionName == "userInfo" || loggingMonitor.actionName == "logout")
            {
                await _db.Insertable(new SysLogVis
                {
                    ControllerName = loggingMonitor.controllerName,
                    ActionName = loggingMonitor.actionTypeName,
                    DisplayTitle = loggingMonitor.displayTitle,
                    Status = loggingMonitor.returnInformation?.httpStatusCode,
                    RemoteIp = remoteIPv4,
                    Location = ipLocation,
                    Longitude = (decimal?)longitude,
                    Latitude = (decimal?)latitude,
                    Browser = browser, // loggingMonitor.userAgent,
                    Os = os, // loggingMonitor.osDescription + " " + loggingMonitor.osArchitecture,
                    Elapsed = loggingMonitor.timeOperationElapsedMilliseconds,
                    LogDateTime = logMsg.LogDateTime,
                    Account = account,
                    RealName = realName,
                    CreateUserId = string.IsNullOrWhiteSpace(userId) ? 0 : long.Parse(userId),
                    TenantId = string.IsNullOrWhiteSpace(tenantId) ? 0 : long.Parse(tenantId),
                    LogLevel = logMsg.LogLevel
                }).ExecuteCommandAsync();
                return;
            }

            // Record operation log
            if (!await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysOpLog)) return;
            await _db.Insertable(new SysLogOp
            {
                ControllerName = loggingMonitor.controllerName,
                ActionName = loggingMonitor.actionTypeName,
                DisplayTitle = loggingMonitor.displayTitle,
                Status = loggingMonitor.returnInformation?.httpStatusCode,
                RemoteIp = remoteIPv4,
                Location = ipLocation,
                Longitude = (decimal?)longitude,
                Latitude = (decimal?)latitude,
                Browser = browser, // loggingMonitor.userAgent,
                Os = os, // loggingMonitor.osDescription + " " + loggingMonitor.osArchitecture,
                Elapsed = loggingMonitor.timeOperationElapsedMilliseconds,
                LogDateTime = logMsg.LogDateTime,
                Account = account,
                RealName = realName,
                HttpMethod = loggingMonitor.httpMethod,
                RequestUrl = loggingMonitor.requestUrl,
                RequestParam = (loggingMonitor.parameters == null || loggingMonitor.parameters.Count == 0) ? null : JSON.Serialize(loggingMonitor.parameters[0].value),
                ReturnResult = loggingMonitor.returnInformation == null ? null : JSON.Serialize(loggingMonitor.returnInformation),
                EventId = logMsg.EventId.Id,
                ThreadId = logMsg.ThreadId,
                TraceId = logMsg.TraceId,
                Exception = loggingMonitor.exception == null ? null : JSON.Serialize(loggingMonitor.exception),
                Message = logMsg.Message,
                CreateUserId = string.IsNullOrWhiteSpace(userId) ? 0 : long.Parse(userId),
                TenantId = string.IsNullOrWhiteSpace(tenantId) ? 0 : long.Parse(tenantId),
                LogLevel = logMsg.LogLevel
            }).ExecuteCommandAsync();

            await Task.Delay(50); // Delay writing to the database by 0.05 seconds, effectively reducing deadlock problems caused by high-frequency writing to the database
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation log storage");
        }
    }

    /// <summary>
    /// Release service scope
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}