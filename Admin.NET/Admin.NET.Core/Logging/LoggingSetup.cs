// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public static class LoggingSetup
{
    /// <summary>
    /// Log registration
    /// </summary>
    /// <param name="services"></param>
    public static void AddLoggingSetup(this IServiceCollection services)
    {
        // Log monitoring
        services.AddMonitorLogging(options =>
        {
            options.IgnorePropertyNames = new[] { "Byte" };
            options.IgnorePropertyTypes = new[] { typeof(byte[]) };
        });

        // console log
        var consoleLog = App.GetConfig<bool>("Logging:Monitor:ConsoleLog", true);
        services.AddConsoleFormatter(options =>
        {
            options.DateFormat = "yyyy-MM-dd HH:mm:ss(zzz) dddd";
            //options.WithTraceId = true; // Display thread Id
            //options.WithStackFrame = true; // Display assembly
            options.WriteFilter = (logMsg) =>
            {
                return consoleLog;
            };
        });

        // log write file
        if (App.GetConfig<bool>("Logging:File:Enabled", true))
        {
            var loggingMonitorSettings = App.GetConfig<LoggingMonitorSettings>("Logging:Monitor", true);
            Array.ForEach(new[] { LogLevel.Information, LogLevel.Warning, LogLevel.Error }, logLevel =>
            {
                services.AddFileLogging(options =>
                {
                    options.WithTraceId = true; // Show thread ID
                    options.WithStackFrame = true; // show assembly
                    options.FileNameRule = fileName => string.Format(fileName, DateTime.Now, logLevel.ToString()); // Create a file every day
                    options.WriteFilter = logMsg => logMsg.LogLevel == logLevel; // Log level
                    options.HandleWriteError = (writeError) => // Enable backup file when write fails
                    {
                        writeError.UseRollbackFileName(Path.GetFileNameWithoutExtension(writeError.CurrentFileName) + "-oops" + Path.GetExtension(writeError.CurrentFileName));
                    };
                    if (loggingMonitorSettings.JsonBehavior == JsonBehavior.OnlyJson)
                    {
                        options.MessageFormat = LoggerFormatter.Json;
                        // options.MessageFormat = LoggerFormatter.JsonIndented;
                        options.MessageFormat = (logMsg) =>
                        {
                            var jsonString = logMsg.Context.Get("loggingMonitor");
                            return jsonString?.ToString();
                        };
                    }
                });
            });
        }

        // Log writing to ElasticSearch
        if (App.GetConfig<bool>("ElasticSearch:Logging:Enabled", true))
        {
            services.AddDatabaseLogging<ElasticSearchLoggingWriter>(options =>
            {
                options.WithTraceId = true; // Show thread ID
                options.WithStackFrame = true; // show assembly
                options.IgnoreReferenceLoop = false; // Ignore loop detection
                options.MessageFormat = LoggerFormatter.Json;
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogName == CommonConst.SysLogCategoryName; // Only write LoggingMonitor logs
                };
            });
        }

        // Log written to database
        if (App.GetConfig<bool>("Logging:Database:Enabled", true))
        {
            services.AddDatabaseLogging<DatabaseLoggingWriter>(options =>
            {
                options.WithTraceId = true; // Show thread ID
                options.WithStackFrame = true; // show assembly
                options.IgnoreReferenceLoop = false; // Ignore loop detection
                options.MessageFormat = (logMsg) =>
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(logMsg.Message);
                    return stringBuilder.ToString();
                };
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogName == CommonConst.SysLogCategoryName; // Only write LoggingMonitor logs
                };
            });
        }
    }
}