// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

#if NET10_0_OR_GREATER

using Microsoft.AspNetCore.Builder;
using XiHan.Framework.Utils.Logging;
using XiHan.Framework.Utils.Reflections;

namespace Admin.NET.Core.Update;

/// <summary>
/// Automatic version update middleware extension
/// </summary>
/// <remarks>
/// How to use
/// 1. Call app.UseAutoVersionUpdate() in the Configure method in Startup.cs of Admin.NET.Web.Core.
/// 2. Create a folder named UpdateScripts in the root directory of the entry project Admin.NET.Web.Entry and place the script file with the .sql suffix in it
/// 3. The script file naming format is version number, such as 1.0.0.sql, 1.0.1.sql, etc. The version number should comply with the semantic version specification.
/// 4. The properties of the script: Copy to output directory, set to: Always copy.
/// 5. Set the WorkerId of Configuration/App.json of Admin.NET.Application of the master node to 1.
/// 6. Set the Version of the entry project Admin.NET.Web.Entry.csproj.
/// ==================================================
/// When updating to a new version
/// 1. A new script file needs to be added to the UpdateScripts folder, and the script file name should be the new version number.
/// 2. Set the Version of the entry project Admin.NET.Web.Entry.csproj
/// </remarks>
[SuppressSniffer]
public static class AutoVersionUpdate
{
    /// <summary>
    /// Use automatic version update middleware
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseAutoVersionUpdate(this IApplicationBuilder app)
    {
        LogHelper.Info("AutoVersionUpdate middleware is running");

        var snowIdOpt = App.GetConfig<SnowIdOptions>("SnowId", true);
        if (snowIdOpt.WorkerId != 1)
        {
            LogHelper.Handle("Non-primary node, do not execute the script");
            return app;
        }

        var currentVersion = GetEntryAssemblyCurrentVersion();
        LogHelper.Handle($"Current version: {currentVersion}");

        var historyVersionInfo = GetEntryAssemblyHistoryVersionInfo();
        var historyVersion = historyVersionInfo.Version;
        var historyDate = historyVersionInfo.Date;
        var historyIsRunScript = historyVersionInfo.IsRunScript;

        LogHelper.Handle($"Historical version: {historyVersion}, update time: {historyDate}, has {historyIsRunScript} been executed");

        // The historical version is empty, the version number is the same, and the script is not executed.
        if (historyVersion == string.Empty)
        {
            LogHelper.Handle("The historical version is empty, the default is the latest version, and the script is not executed.");

            // Save current version information
            SetEntryAssemblyCurrentVersion(currentVersion, true);

            return app;
        }
        else if (currentVersion.CompareTo(historyVersion) <= 0 && historyIsRunScript)
        {
            LogHelper.Handle("Currentversion numberWith historyversion numberSame，And has been executedscript，No longer executing");

            // Save current version information
            SetEntryAssemblyCurrentVersion(currentVersion, false);

            return app;
        }
        else
        {
            LogHelper.Handle("The current version number is different from the historical version number, or the version number is the same but the script has not been executed. Start executing the script.");

            var scriptSqlVersions = GetScriptSqlVersions();

            // If the current version of the script does not exist, only the current version information will be saved and the script will not be executed.
            if (scriptSqlVersions.All(s => s.Version.CompareTo(currentVersion) < 0))
            {
                LogHelper.Handle("There is no current version of the script. Only the current version information is saved and the script is not executed.");

                // Save current version information
                SetEntryAssemblyCurrentVersion(currentVersion, false);

                return app;
            }

            // Execute script
            foreach (var sqlFileInfo in scriptSqlVersions)
            {
                var sqlVersion = sqlFileInfo.Version;

                // Only execute scripts that are larger than the historical version, or the current version has not been executed
                if (sqlVersion.CompareTo(historyVersion) < 0)
                {
                    LogHelper.Handle($"Version {sqlVersion} is lower than the historical version, skip");
                    continue;
                }
                if (sqlVersion == historyVersion && historyIsRunScript)
                {
                    LogHelper.Handle($"Version {sqlVersion} is equal to the historical version, and the script has been executed, skip");
                    continue;
                }

                // Execute script
                var sql = File.ReadAllText(sqlFileInfo.FilePath);
                if (sql != null)
                {
                    LogHelper.Handle($"Execute version {sqlVersion} script");

                    HandleSqlScript(app, sql, sqlVersion);
                }
            }
        }

        LogHelper.Success("AutoVersionUpdate middleware ended");

        return app;
    }

    #region Helper method

    /// <summary>
    /// Get the current version information of the entry assembly
    /// </summary>
    /// <returns></returns>
    private static string GetEntryAssemblyCurrentVersion()
    {
        var entryAssemblyVersion = ReflectionHelper.GetEntryAssemblyVersion();
        return entryAssemblyVersion.ToString(3);
    }

    /// <summary>
    /// Set the current version information of the entry assembly
    /// </summary>
    /// <param name="version"></param>
    /// <param name="isRunScript"></param>
    private static void SetEntryAssemblyCurrentVersion(string version, bool isRunScript)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "version.txt");
        var now = DateTime.Now;
        File.WriteAllText(path, $"{version}^{now:yyyy-MM-dd HH:mm:ss}^{isRunScript}");
    }

    /// <summary>
    /// Get the last running version information of the entry assembly
    /// </summary>
    /// <returns></returns>
    private static HistoryVersionInfo GetEntryAssemblyHistoryVersionInfo()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "version.txt");

        // Check if the file exists
        if (File.Exists(path))
        {
            // Read the contents of the file if it exists
            var info = File.ReadAllText(path);

            if (info.Contains('^'))
            {
                var parts = info.Split('^');
                var version = parts.Length > 0 ? parts[0].ToString() : string.Empty;
                var date = parts.Length > 1 ? parts[1] : string.Empty;
                var isRunScript = parts.Length > 2 ? parts[2].ToBoolean() : false;

                return new HistoryVersionInfo(version, date, isRunScript);
            }
        }

        // Returns the default value when the file does not exist or the content format is incorrect.
        return new HistoryVersionInfo(string.Empty, string.Empty, false);
    }

    /// <summary>
    /// Get the script SQL file version in the program directory
    /// </summary>
    /// <returns></returns>
    private static List<SqlFileInfo> GetScriptSqlVersions()
    {
        // Get all script files
        var path = Path.Combine(AppContext.BaseDirectory, "UpdateScripts");
        var scriptFiles = Directory.GetFiles(path, "*.sql").ToList();

        var sqlVersions = scriptFiles
            .Select(s => new SqlFileInfo(Path.GetFileNameWithoutExtension(s), s))
            .OrderBy(s => s.Version).ToList();
        return sqlVersions;
    }

    /// <summary>
    /// Save current version information
    /// </summary>
    /// <param name="app"></param>
    /// <param name="sql"></param>
    /// <param name="sqlVersion"></param>
    private static void HandleSqlScript(IApplicationBuilder app, string sql, string sqlVersion)
    {
        using var scope = App.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        var isSuccess = false;

        try
        {
            // Open transaction
            dbContext.Ado.BeginTran();
            dbContext.Ado.ExecuteCommand(sql);
            dbContext.Ado.CommitTran();
            isSuccess = true;
        }
        catch (Exception ex)
        {
            dbContext.Ado.RollbackTran();
            LogHelper.Error($"AutoVersionUpdate encountered an error executing the SQL script, version: {sqlVersion}, error: {ex.Message}");
        }
        finally
        {
            if (isSuccess)
            {
                // Save current version information
                SetEntryAssemblyCurrentVersion(sqlVersion, true);
            }
        }
    }

    #endregion Helper method
}

public record SqlFileInfo(string Version, string FilePath);
public record HistoryVersionInfo(string Version, string Date, bool IsRunScript);

#endif // NET10_0_OR_GREATER