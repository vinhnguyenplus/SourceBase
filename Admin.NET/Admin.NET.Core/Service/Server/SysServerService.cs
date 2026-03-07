// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

#if NET10_0_OR_GREATER

using XiHan.Framework.Utils.Core;
using XiHan.Framework.Utils.Reflections;
using ReflectionHelper = XiHan.Framework.Utils.Reflections.ReflectionHelper;

#endif // NET10_0_OR_GREATER

namespace Admin.NET.Core.Service;

/// <summary>
/// System server monitoring service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 290, Description = "Server monitoring")]
public class SysServerService : IDynamicApiController, ITransient
{
    public SysServerService()
    {
    }

#if NET10_0_OR_GREATER

    /// <summary>
    /// Get server hardware information
    /// </summary>
    /// <returns></returns>
    [DisplayName("Obtain server hardware information")]
    public SystemInfo HardwareInfo()
    {
        var hardwareInfo = SystemInfoManager.GetSystemInfo();
        return hardwareInfo;
    }

    /// <summary>
    /// Get server runtime information
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get server runtime information")]
    public XiHan.Framework.Utils.Runtime.RuntimeInfo RuntimeInfo()
    {
        var systemRuntimeInfo = new XiHan.Framework.Utils.Runtime.RuntimeInfo();
        return systemRuntimeInfo;
    }

    /// <summary>
    /// Get the framework main assembly
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the main assemblies of the framework")]
    public List<NuGetPackage> NuGetPackagesInfo()
    {
        var nuGetPackages = ReflectionHelper.GetNuGetPackages("Admin.NET");
        return nuGetPackages;
    }

#endif // NET10_0_OR_GREATER

    /// <summary>
    /// Get server configuration information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get server configuration information")]
    public dynamic GetServerBase()
    {
        return new
        {
            HostName = Environment.MachineName, // hostname
            SystemOs = ComputerUtil.GetOSInfo(),// RuntimeInformation.OSDescription, // operating system
            OsArchitecture = Environment.OSVersion.Platform.ToString() + " " + RuntimeInformation.OSArchitecture.ToString(), // System architecture
            ProcessorCount = Environment.ProcessorCount + " nuclear", // Number of CPU cores
            SysRunTime = ComputerUtil.GetRunTime(), // System running time
            RemoteIp = ComputerUtil.GetIpFromOnline(), // External network address
            LocalIp = App.HttpContext?.Connection?.LocalIpAddress!.MapToIPv4().ToString(), // local address
            FrameworkDescription = RuntimeInformation.FrameworkDescription + " / " + App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0].DbType.ToString(), // NET framework + database type
            Environment = App.HostEnvironment.IsDevelopment() ? "Development" : "Production",
            Wwwroot = App.WebHostEnvironment.WebRootPath, // Website root directory
            Stage = App.HostEnvironment.IsStaging() ? "Stage environment" : "Non-Stage Environment", // Whether Stage environment
        };
    }

    /// <summary>
    /// Get server usage information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get server usage information")]
    public dynamic GetServerUsed()
    {
        var programStartTime = Process.GetCurrentProcess().StartTime;
        var totalMilliseconds = (DateTime.Now - programStartTime).TotalMilliseconds.ToString();
        var ts = totalMilliseconds.Contains('.') ? totalMilliseconds.Split('.')[0] : totalMilliseconds;
        var programRunTime = DateTimeUtil.FormatTime(ts.ParseToLong());

        var memoryMetrics = ComputerUtil.GetComputerInfo();
        return new
        {
            memoryMetrics.FreeRam, // free memory
            memoryMetrics.UsedRam, // Used memory
            memoryMetrics.TotalRam, // total memory
            memoryMetrics.RamRate, // memory usage
            memoryMetrics.CpuRates, // CPU UsageMulti-CPU Not Completed
            memoryMetrics.CpuRate, // CPU 1 usage
            StartTime = programStartTime.ToString("yyyy-MM-dd HH:mm:ss"), // Service start time
            RunTime = programRunTime, // Service running time
        };
    }

    /// <summary>
    /// Get server disk information 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get server disk information")]
    public dynamic GetServerDisk()
    {
        return ComputerUtil.GetDiskInfos();
    }

    /// <summary>
    /// Get the framework main assembly 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the main assemblies of the framework")]
    public dynamic GetAssemblyList()
    {
        var furionAssembly = typeof(App).Assembly.GetName();
        var sqlSugarAssembly = typeof(ISqlSugarClient).Assembly.GetName();
        var yitIdAssembly = typeof(YitIdHelper).Assembly.GetName();
        var redisAssembly = typeof(Redis).Assembly.GetName();
        var jsonAssembly = typeof(NewtonsoftJsonMvcCoreBuilderExtensions).Assembly.GetName();
        var excelAssembly = typeof(IExcelImporter).Assembly.GetName();
        var pdfAssembly = typeof(Magicodes.ExporterAndImporter.Pdf.IPdfExporter).Assembly.GetName();
        var wordAssembly = typeof(Magicodes.ExporterAndImporter.Word.IWordExporter).Assembly.GetName();
        var captchaAssembly = typeof(Lazy.Captcha.Core.ICaptcha).Assembly.GetName();
        var wechatApiAssembly = typeof(WechatApiClient).Assembly.GetName();
        var wechatTenpayAssembly = typeof(WechatTenpayClient).Assembly.GetName();
        var ossAssembly = typeof(OnceMi.AspNetCore.OSS.IOSSServiceFactory).Assembly.GetName();
        var parserAssembly = typeof(Parser).Assembly.GetName();
        var elasticsearchClientAssembly = typeof(Elastic.Clients.Elasticsearch.ElasticsearchClient).Assembly.GetName();
        var limitAssembly = typeof(AspNetCoreRateLimit.IpRateLimitMiddleware).Assembly.GetName();
        var htmlParserAssembly = typeof(AngleSharp.Html.Parser.HtmlParser).Assembly.GetName();
        var fluentEmailAssembly = typeof(MailKit.Net.Smtp.SmtpClient).Assembly.GetName();
        var qRCodeGeneratorAssembly = typeof(QRCoder.QRCodeGenerator).Assembly.GetName();
        var alibabaSendSmsRequestAssembly = typeof(AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest).Assembly.GetName();
        var tencentSendSmsRequestAssembly = typeof(TencentCloud.Sms.V20190711.Models.SendSmsRequest).Assembly.GetName();
        var rabbitMQAssembly = typeof(RabbitMQEventSourceStore).Assembly.GetName();
        var ldapConnectionAssembly = typeof(Novell.Directory.Ldap.LdapConnection).Assembly.GetName();
        var ipToolAssembly = typeof(IPTools.Core.IpTool).Assembly.GetName();
        var weixinAuthenticationOptionsAssembly = typeof(AspNet.Security.OAuth.Weixin.WeixinAuthenticationOptions).Assembly.GetName();
        var giteeAuthenticationOptionsAssembly = typeof(AspNet.Security.OAuth.Gitee.GiteeAuthenticationOptions).Assembly.GetName();
        var hashidsAssembly = typeof(HashidsNet.Hashids).Assembly.GetName();
        var sftpClientAssembly = typeof(Renci.SshNet.SftpClient).Assembly.GetName();
        var hardwareInfoAssembly = typeof(Hardware.Info.HardwareInfo).Assembly.GetName();

        return new[]
        {
            new { furionAssembly.Name, furionAssembly.Version },
            new { sqlSugarAssembly.Name, sqlSugarAssembly.Version },
            new { yitIdAssembly.Name, yitIdAssembly.Version },
            new { redisAssembly.Name, redisAssembly.Version },
            new { jsonAssembly.Name, jsonAssembly.Version },
            new { excelAssembly.Name, excelAssembly.Version },
            new { pdfAssembly.Name, pdfAssembly.Version },
            new { wordAssembly.Name, wordAssembly.Version },
            new { captchaAssembly.Name, captchaAssembly.Version },
            new { wechatApiAssembly.Name, wechatApiAssembly.Version },
            new { wechatTenpayAssembly.Name, wechatTenpayAssembly.Version },
            new { ossAssembly.Name, ossAssembly.Version },
            new { parserAssembly.Name, parserAssembly.Version },
            new { elasticsearchClientAssembly.Name, elasticsearchClientAssembly.Version },
            new { limitAssembly.Name, limitAssembly.Version },
            new { htmlParserAssembly.Name, htmlParserAssembly.Version },
            new { fluentEmailAssembly.Name, fluentEmailAssembly.Version },
            new { qRCodeGeneratorAssembly.Name, qRCodeGeneratorAssembly.Version },
            new { alibabaSendSmsRequestAssembly.Name, alibabaSendSmsRequestAssembly.Version },
            new { tencentSendSmsRequestAssembly.Name, tencentSendSmsRequestAssembly.Version },
            new { rabbitMQAssembly.Name, rabbitMQAssembly.Version },
            new { ldapConnectionAssembly.Name, ldapConnectionAssembly.Version },
            new { ipToolAssembly.Name, ipToolAssembly.Version },
            new { weixinAuthenticationOptionsAssembly.Name, weixinAuthenticationOptionsAssembly.Version },
            new { giteeAuthenticationOptionsAssembly.Name, giteeAuthenticationOptionsAssembly.Version },
            new { hashidsAssembly.Name, hashidsAssembly.Version },
            new { sftpClientAssembly.Name, sftpClientAssembly.Version },
            new { hardwareInfoAssembly.Name, hardwareInfoAssembly.Version },
        };
    }
}