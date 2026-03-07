// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;

namespace Admin.NET.Core.Service;

/// <summary>
/// System update management service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 390)]
public class SysUpdateService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;
    private readonly CDConfigOptions _cdConfigOptions;

    public SysUpdateService(IOptions<CDConfigOptions> giteeOptions, SysCacheService sysCacheService)
    {
        _cdConfigOptions = giteeOptions.Value;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// Backup list
    /// </summary>
    /// <returns></returns>
    [DisplayName("Backup list")]
    [ApiDescriptionSettings(Name = "List"), HttpPost]
    public Task<List<BackupOutput>> List()
    {
        const string backendDir = "Admin.NET";
        var rootPath = Path.GetFullPath(Path.Combine(_cdConfigOptions.BackendOutput, ".."));
        return Task.FromResult(Directory.GetFiles(rootPath, backendDir + "*.zip", SearchOption.TopDirectoryOnly)
            .Select(filePath =>
            {
                var file = new FileInfo(filePath);
                return new BackupOutput
                {
                    CreateTime = file.CreationTime,
                    FilePath = filePath,
                    FileName = file.Name
                };
            })
            .OrderByDescending(u => u.CreateTime)
            .ToList());
    }

    /// <summary>
    /// reduction
    /// </summary>
    /// <returns></returns>
    [DisplayName("restore")]
    [ApiDescriptionSettings(Name = "Restore"), HttpPost]
    public async Task Restore(RestoreInput input)
    {
        // Check parameters
        CheckConfig();
        try
        {
            var file = (await List()).FirstOrDefault(u => u.FileName.EqualIgnoreCase(input.FileName));
            if (file == null)
            {
                PrintfLog("File does not exist...");
                return;
            }

            PrintfLog("Restoring...");
            using ZipArchive archive = new(File.OpenRead(file.FilePath), ZipArchiveMode.Read, leaveOpen: false);
            archive.ExtractToDirectory(_cdConfigOptions.BackendOutput, true);
            PrintfLog("Restore successful...");
        }
        catch (Exception ex)
        {
            PrintfLog("An exception occurred:" + ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Updating the system remotely
    /// </summary>
    /// <returns></returns>
    [DisplayName("System update")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update()
    {
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now}] Deploy project from remote repository");
        try
        {
            PrintfLog("----------------------------Deploy the project from the remote warehouse-start---------------------------");

            // Check parameters
            CheckConfig();

            // Check operation interval
            if (_cdConfigOptions.UpdateInterval > 0)
            {
                if (_sysCacheService.Get<bool>(CacheConst.KeySysUpdateInterval)) throw Oops.Oh("Do not operate frequently");
                _sysCacheService.Set(CacheConst.KeySysUpdateInterval, true, TimeSpan.FromMinutes(_cdConfigOptions.UpdateInterval));
            }

            PrintfLog($"Client host: {App.HttpContext.Request.Host}");
            PrintfLog($"Client IP: {App.HttpContext.GetRemoteIpAddressToIPv4(true)}");
            PrintfLog($"Repository address: https://gitee.com/{_cdConfigOptions.Owner}/{_cdConfigOptions.Repo}.git");
            PrintfLog($"Warehouse branch: {_cdConfigOptions.Branch}");

            // Get the decompressed root directory
            var rootPath = Path.GetFullPath(Path.Combine(_cdConfigOptions.BackendOutput, ".."));
            var tempDir = Path.Combine(rootPath, $"{_cdConfigOptions.Repo}-{_cdConfigOptions.Branch}");

            PrintfLog("Clean up old files...");
            FileHelper.TryDelete(tempDir);

            PrintfLog("Pulling remote code...");
            var stream = await GiteeHelper.DownloadRepoZip(_cdConfigOptions.Owner, _cdConfigOptions.Repo,
                _cdConfigOptions.AccessToken, _cdConfigOptions.Branch);

            PrintfLog("Unzip the file package...");
            using ZipArchive archive = new(stream, ZipArchiveMode.Read, leaveOpen: false);
            archive.ExtractToDirectory(rootPath);

            // Project directory
            var backendDir = "Admin.NET"; // Backend root directory
            var entryProjectName = "Admin.NET.Web.Entry"; // Start project directory
            var tempOutput = Path.Combine(rootPath, $"{_cdConfigOptions.Repo}_temp");

            PrintfLog("Compile project...");
            PrintfLog($"Release version: {_cdConfigOptions.Publish.Configuration}");
            PrintfLog($"Target framework: {_cdConfigOptions.Publish.TargetFramework}");
            PrintfLog($"Running environment: {_cdConfigOptions.Publish.RuntimeIdentifier}");
            var option = _cdConfigOptions.Publish;
            var adminNetDir = Path.Combine(tempDir, backendDir);
            var args = $"publish \"{entryProjectName}\" -c {option.Configuration} -f {option.TargetFramework} -r {option.RuntimeIdentifier} --output \"{tempOutput}\"";
            await RunCommandAsync("dotnet", args, adminNetDir);

            PrintfLog("Copy wwwroot Table of Contents...");
            var wwwrootDir = Path.Combine(adminNetDir, entryProjectName, "wwwroot");
            FileHelper.CopyDirectory(wwwrootDir, Path.Combine(tempOutput, "wwwroot"), true);

            // Delete excluded files
            foreach (var filePath in (_cdConfigOptions.ExcludeFiles ?? new()).SelectMany(file => Directory.GetFiles(tempOutput, file, SearchOption.TopDirectoryOnly)))
            {
                PrintfLog($"Exclude files: {filePath}");
                FileHelper.TryDelete(filePath);
            }

            PrintfLog("Backing up the original project files...");
            string backupPath = Path.Combine(rootPath, $"{_cdConfigOptions.Repo}_{DateTime.Now:yyyy_MM_dd}.zip");
            if (File.Exists(backupPath)) File.Delete(backupPath);
            ZipFile.CreateFromDirectory(_cdConfigOptions.BackendOutput, backupPath);

            // Move temporary files to official directory
            FileHelper.CopyDirectory(tempOutput, _cdConfigOptions.BackendOutput, true);

            PrintfLog("Clean files...");
            FileHelper.TryDelete(tempOutput);
            FileHelper.TryDelete(tempDir);

            if (_cdConfigOptions.BackupCount > 0)
            {
                var fileList = await List();
                if (fileList.Count > _cdConfigOptions.BackupCount)
                    PrintfLog("Clearing unnecessary backup files...");
                while (fileList.Count > _cdConfigOptions.BackupCount)
                {
                    var last = fileList.Last();
                    FileHelper.TryDelete(last.FilePath);
                    fileList.Remove(last);
                }
            }

            PrintfLog("Takes effect after restarting the project...");
        }
        catch (Exception ex)
        {
            PrintfLog("An exception occurred:" + ex.Message);
            throw;
        }
        finally
        {
            PrintfLog("----------------------------Deploy Project from Remote Repository - End----------------------------");
            Console.ForegroundColor = originColor;
        }
    }

    /// <summary>
    /// Warehouse WebHook interface
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("Warehouse WebHook interface")]
    [ApiDescriptionSettings(Name = "WebHook"), HttpPost]
    public async Task WebHook(Dictionary<string, object> input)
    {
        if (!_cdConfigOptions.Enabled) throw Oops.Oh("Continuous deployment feature not enabled");
        PrintfLog("---------------------------- WebHook request received - Start ----------------------------");

        try
        {
            // Get request header information
            var even = App.HttpContext.Request.Headers.FirstOrDefault(u => u.Key == "X-Gitee-Event").Value
                .FirstOrDefault();
            var ua = App.HttpContext.Request.Headers.FirstOrDefault(u => u.Key == "User-Agent").Value.FirstOrDefault();

            var timestamp = input.GetValueOrDefault("timestamp")?.ToString();
            var token = input.GetValueOrDefault("sign")?.ToString();
            PrintfLog("User-Agent：" + ua);
            PrintfLog("Gitee-Event：" + even);
            PrintfLog("Gitee-Token：" + token);
            PrintfLog("Gitee-Timestamp：" + timestamp);

            PrintfLog("Starting signature verification...");
            var secret = GetWebHookKey();
            var stringToSign = $"{timestamp}\n{secret}";
            using var mac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var signData = mac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            var encodedSignData = Convert.ToBase64String(signData);
            var calculatedSignature = WebUtility.UrlEncode(encodedSignData);

            if (calculatedSignature != token) throw Oops.Oh("IllegalSignature");
            PrintfLog("Signature verification successful...");

            var hookName = input.GetValueOrDefault("hook_name") as string;
            PrintfLog("Hook-Name：" + hookName);

            switch (hookName)
            {
                // Submit changes
                case "push_hooks":
                    {
                        var commitList = input.GetValueOrDefault("commits")?.Adapt<List<Dictionary<string, object>>>() ?? new();
                        foreach (var commit in commitList)
                        {
                            var author = commit.GetValueOrDefault("author")?.Adapt<Dictionary<string, object>>();
                            PrintfLog("Commit-Message：" + commit.GetValueOrDefault("message"));
                            PrintfLog("Commit-Time：" + commit.GetValueOrDefault("timestamp"));
                            PrintfLog("Commit-Author：" + author?.GetValueOrDefault("username"));
                            PrintfLog("Modified-List：" + author?.GetValueOrDefault("modified")?.Adapt<List<string>>().Join());
                            PrintfLog("----------------------------------------------------------");
                        }

                        break;
                    }
                // Merge Pull Request
                case "merge_request_hooks":
                    {
                        var pull = input.GetValueOrDefault("pull_request")?.Adapt<Dictionary<string, object>>();
                        var user = pull?.GetValueOrDefault("user")?.Adapt<Dictionary<string, object>>();
                        PrintfLog("Pull-Request-Title：" + pull?.GetValueOrDefault("message"));
                        PrintfLog("Pull-Request-Time：" + pull?.GetValueOrDefault("created_at"));
                        PrintfLog("Pull-Request-Author：" + user?.GetValueOrDefault("username"));
                        PrintfLog("Pull-Request-Body：" + pull?.GetValueOrDefault("body"));
                        break;
                    }
                // new issue
                case "issue_hooks":
                    {
                        var issue = input.GetValueOrDefault("issue")?.Adapt<Dictionary<string, object>>();
                        var user = issue?.GetValueOrDefault("user")?.Adapt<Dictionary<string, object>>();
                        var labelList = issue?.GetValueOrDefault("labels")?.Adapt<List<Dictionary<string, object>>>();
                        PrintfLog("Issue-UserName：" + user?.GetValueOrDefault("username"));
                        PrintfLog("Issue-Labels：" + labelList?.Select(u => u.GetValueOrDefault("name")).Join());
                        PrintfLog("Issue-Title：" + issue?.GetValueOrDefault("title"));
                        PrintfLog("Issue-Time：" + issue?.GetValueOrDefault("created_at"));
                        PrintfLog("Issue-Body：" + issue?.GetValueOrDefault("body"));
                        return;
                    }
                // Comment
                case "note_hooks":
                    {
                        var comment = input.GetValueOrDefault("comment")?.Adapt<Dictionary<string, object>>();
                        var user = input.GetValueOrDefault("user")?.Adapt<Dictionary<string, object>>();
                        PrintfLog("comment-UserName：" + user?.GetValueOrDefault("username"));
                        PrintfLog("comment-Time：" + comment?.GetValueOrDefault("created_at"));
                        PrintfLog("comment-Content：" + comment?.GetValueOrDefault("body"));
                        return;
                    }
                default:
                    return;
            }

            var updateInterval = _cdConfigOptions.UpdateInterval;
            try
            {
                _cdConfigOptions.UpdateInterval = 0;
                await Update();
            }
            finally
            {
                _cdConfigOptions.UpdateInterval = updateInterval;
            }
        }
        finally
        {
            PrintfLog("---------------------------- WebHook request received - end ----------------------------");
        }
    }

    /// <summary>
    /// Get WebHook interface key
    /// </summary>
    /// <returns></returns>
    [DisplayName("Obtain WebHook API Key")]
    [ApiDescriptionSettings(Name = "WebHookKey"), HttpGet]
    public string GetWebHookKey()
    {
        return CryptogramUtil.Encrypt(_cdConfigOptions.AccessToken);
    }

    /// <summary>
    /// Get log list
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get log list")]
    [ApiDescriptionSettings(Name = "Logs"), HttpGet]
    public List<string> LogList()
    {
        return _sysCacheService.Get<List<string>>(CacheConst.KeySysUpdateLog) ?? new();
    }

    /// <summary>
    /// Clear log
    /// </summary>
    /// <returns></returns>
    [DisplayName("Clear logs")]
    [ApiDescriptionSettings(Name = "Clear"), HttpGet]
    public void ClearLog()
    {
        _sysCacheService.Remove(CacheConst.KeySysUpdateLog);
    }

    /// <summary>
    /// Check parameters
    /// </summary>
    /// <returns></returns>
    private void CheckConfig()
    {
        PrintfLog("Checking CD configuration parameters...");

        if (_cdConfigOptions == null) throw Oops.Oh("CDConfig configuration cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.Owner)) throw Oops.Oh("The warehouse username cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.Repo)) throw Oops.Oh("Warehouse name cannot be empty");

        // if (string.IsNullOrWhiteSpace(_cdConfigOptions.Branch)) throw Oops.Oh("The branch name cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.AccessToken)) throw Oops.Oh("Authorization information cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.BackendOutput)) throw Oops.Oh("The deployment directory cannot be empty");

        if (_cdConfigOptions.Publish == null) throw Oops.Oh("Compilation configuration cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.Publish.Configuration)) throw Oops.Oh("The runtime environment compile configuration cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.Publish.TargetFramework)) throw Oops.Oh(".NET version compilation configuration cannot be empty");

        if (string.IsNullOrWhiteSpace(_cdConfigOptions.Publish.RuntimeIdentifier)) throw Oops.Oh("The runtime platform configuration cannot be empty");
    }

    /// <summary>
    /// Print log
    /// </summary>
    /// <param name="message"></param>
    private void PrintfLog(string message)
    {
        var logList = _sysCacheService.Get<List<string>>(CacheConst.KeySysUpdateLog) ?? new();

        var content = $"【{DateTime.Now}】 {message}";

        Console.WriteLine(content);

        logList.Add(content);

        _sysCacheService.Set(CacheConst.KeySysUpdateLog, logList);
    }

    /// <summary>
    /// execute command
    /// </summary>
    /// <param name="command">Order</param>
    /// <param name="arguments">parameter</param>
    /// <param name="workingDirectory">working directory</param>
    private async Task RunCommandAsync(string command, string arguments, string workingDirectory)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();

        while (!process.StandardOutput.EndOfStream)
        {
            string line = await process.StandardOutput.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;
            PrintfLog(line.Trim());
        }
        await process.WaitForExitAsync();
    }
}