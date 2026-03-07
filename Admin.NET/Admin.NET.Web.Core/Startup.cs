// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;
using Admin.NET.Core.ElasticSearch;
using Admin.NET.Core.Service;
using AspNetCoreRateLimit;
using Furion;
using Furion.Logging;
using Furion.SpecificationDocument;
using Furion.VirtualFileServer;
using IPTools.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OnceMi.AspNetCore.OSS;
using Scalar.AspNetCore;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

#if NET10_0_OR_GREATER
using Admin.NET.Core.Update;
#endif

namespace Admin.NET.Web.Core;

[AppStartup(int.MaxValue)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configuration options
        services.AddProjectOptions();

        // Cache registration
        services.AddCache();
        // SqlSugar
        services.AddSqlSugar();
        // JWT
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true, jwtBearerConfigure: options =>
        {
            // Implement JWT authentication process control
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var httpContext = context.HttpContext;
                    // If the request URL contains the token parameter, set the Token value
                    if (httpContext.Request.Query.ContainsKey("token"))
                        context.Token = httpContext.Request.Query["token"];
                    return Task.CompletedTask;
                }
            };
        }).AddSignatureAuthentication(options =>  // Add Signature authentication
        {
            options.Events = SysOpenAccessService.GetSignatureAuthenticationEventImpl();
        });

        // Allow cross domain
        services.AddCorsAccessor();
        // remote request
        services.AddHttpRemote();
        // task queue
        services.AddTaskQueue();
        // Task scheduling
        services.AddSchedule(options =>
        {
            options.AddPersistence<DbJobPersistence>(); // Add job persister
            options.AddMonitor<JobMonitor>(); // Add job execution monitor
        });
        // Desensitization test
        services.AddSensitiveDetection();

        // Json serialization settings
        static void SetNewtonsoftJsonSetting(JsonSerializerSettings setting)
        {
            setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            //setting.Converters.AddDateTimeTypeConverters(localized: false); // Time localization
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // time formatting
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // Ignore circular references
            // setting.ContractResolver = new CamelCasePropertyNamesContractResolver(); // Resolve capitalization of dynamic object property names
            // setting.NullValueHandling = NullValueHandling.Ignore; // Ignore null values
            setting.Converters.AddLongTypeConverters(); // Convert long to string (prevent js precision overflow) when more than 17 bits are turned on
            // setting.MetadataPropertyHandling = MetadataPropertyHandling.Ignore; // Solve DateTimeOffset exception
            // setting.DateParseHandling = DateParseHandling.None; // Solve DateTimeOffset exception
            // setting.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }); // Solve DateTimeOffset exception
        }
        ;

        services.AddControllersWithViews()
            .AddAppLocalization()
            .AddNewtonsoftJson(options => SetNewtonsoftJsonSetting(options.SerializerSettings))
            //.AddXmlSerializerFormatters()
            //.AddXmlDataContractSerializerFormatters()
            .AddInjectWithUnifyResult<AdminResultProvider>()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); // Disable Unicode transcoding
                options.JsonSerializerOptions.Converters.AddDateTimeTypeConverters("yyyy-MM-dd HH:mm:ss"); // time formatting
            });

        // Three-party authorized login OAuth
        services.AddOAuth();

        // ElasticSearch
        services.AddElasticSearchClients();

        // Configure Nginx forwarding to obtain the client’s real IP
        // Note 1: If the load balancing does not forward requests through the Loopback address locally, be sure to add options.KnownNetworks.Clear() and options.KnownProxies.Clear()
        // Note 2: If the environment variable ASPNETCORE_FORWARDEDHEADERS_ENABLED is set to True, the following configuration code is not required
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Current limiting service
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // event bus
        services.AddEventBus(options =>
        {
            options.UseUtcTimestamp = false;
            // Disable event logging
            options.LogEnabled = false;
            // Event executor (retry on failure)
            options.AddExecutor<RetryEventHandlerExecutor>();
            // Event executor (handler that still handles unhandled exceptions after retries)
            options.UnobservedTaskExceptionHandler = (obj, args) =>
            {
                if (args.Exception?.Message != null)
                    Log.Error($"EeventBus has unhandled exception: {args.Exception?.Message} ", args.Exception);
            };
            // Event executor-monitor (entered every time processing)
            options.AddMonitor<EventHandlerMonitor>();

            #region RedisinformationQueue

            // Replace event source storage with Redis
            var cacheOptions = App.GetConfig<CacheOptions>("Cache", true);
            if (cacheOptions.CacheType == CacheTypeEnum.Redis.ToString())
            {
                options.ReplaceStorer(serviceProvider =>
                {
                    var cacheProvider = serviceProvider.GetRequiredService<NewLife.Caching.ICacheProvider>();
                    // Create a default memory channel event source object and customize the queue routing key, such as: adminnet_eventsource_queue
                    return new RedisEventSourceStorer(cacheProvider, "adminnet_eventsource_queue", 3000);
                });
            }

            #endregion RedisinformationQueue

            #region RabbitMQinformationQueue

            //// Create a default memory channel event source object, you can customize the queue routing key, such as: adminnet
            //var eventBusOpt = App.GetConfig<EventBusOptions>("EventBus", true);
            //var rbmqEventSourceStorer = new RabbitMQEventSourceStore(new ConnectionFactory
            //{
            //    UserName = eventBusOpt.RabbitMQ.UserName,
            //    Password = eventBusOpt.RabbitMQ.Password,
            //    HostName = eventBusOpt.RabbitMQ.HostName,
            //    Port = eventBusOpt.RabbitMQ.Port
            //}, "adminnet", 3000);

            ////Replace default event bus memory
            //options.ReplaceStorer(serviceProvider =>
            //{
            //    return rbmqEventSourceStorer;
            //});

            #endregion RabbitMQinformationQueue
        });

        // image processing
        services.AddImageSharp();

        // OSS object storage
        var ossOpt = App.GetConfig<OSSProviderOptions>("OSSProvider", true);
        services.AddOSSService(Enum.GetName(ossOpt.Provider), "OSSProvider");

        // File storage service
        services.AddTransient<SysFileProviderService>();
        services.AddSingleton<IOSSServiceManager, OSSServiceManager>(); // Change to singleton to keep cache
        services.AddTransient<MultiOSSFileProvider>();

        // template engine
        services.AddViewEngine();

        // instant messaging
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15); // The interval between pings from the server to the client
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // The interval between client and server pings
            options.MaximumReceiveMessageSize = 1024 * 1014 * 10; // Data packet size is 10M, the default maximum is 32K
        }).AddNewtonsoftJsonProtocol(options => SetNewtonsoftJsonSetting(options.PayloadSerializerSettings));

        // System log
        services.AddLoggingSetup();

        // Verification code
        services.AddCaptcha();

        // Console logo
        services.AddConsoleLogo();

        //// Swagger time formatting
        //services.AddSwaggerGen(c =>
        //{
        //    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
        //    {
        //        Type = "string",
        //        Format = "date-time",
        //        Example = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) // Example value
        //    });

        //    // Make sure the generated document contains the OpenAPI version field
        //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = "Admin.NET API",
        //        Description = "Admin.NET universal permissions development platform"
        //    });
        //    c.OperationFilter<TenantHeaderOperationFilter>();
        //});

        // Completely load the IP address database file into the memory to improve query speed (exchanging space for time, the memory will increase by 60-70M)
        IpToolSettings.LoadInternationalDbToMemory = true;
        // Set the default query query China and International
        //IpToolSettings.DefalutSearcherType = IpSearcherType.China;
        IpToolSettings.DefalutSearcherType = IpSearcherType.International;

        // Configure the compression level of gzip and br to be optimal
        //services.Configure<BrotliCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.Optimal;
        //});
        //services.Configure<GzipCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.Optimal;
        //});
        // Register compressed response
        services.AddResponseCompression((options) =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml"
             ]);
        });

        // Register the virtual file system service
        services.AddVirtualFileServer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // response compression
        app.UseResponseCompression();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Admin.NET", "Admin.NET");
            await next();
        });

        // image processing
        app.UseImageSharp();

        // Specific file type (file suffix) processing
        var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
        // contentTypeProvider.Mappings[".file suffix"] = "MIME type";
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypeProvider
        });
        // Secondary directory file path analysis
        if (!string.IsNullOrEmpty(App.Settings.VirtualPath))
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = App.Settings.VirtualPath,
                FileProvider = App.WebHostEnvironment.WebRootFileProvider
            });
        ////enable HTTPS
        //app.UseHttpsRedirection();

        // Enable OAuth
        app.UseOAuth();

        // Add status code interception middleware
        app.UseUnifyResultStatusCodes();

        // Enable multi-language, must be used before UseRouting
        app.UseAppLocalization();

        // Route registration
        app.UseRouting();

        // To enable cross-origin, must be registered between UseRouting and UseAuthentication
        app.UseCorsAccessor();

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Current limiting component (after cross-domain)
        app.UseIpRateLimiting();
        app.UseClientRateLimiting();
        app.UsePolicyRateLimit();

        // Task scheduling dashboard
        app.UseScheduleUI(options =>
        {
            options.RequestPath = "/schedule"; // Must start with / and not end with /
            options.DisableOnProduction = false; // Whether to turn off in production environment
            options.DisplayEmptyTriggerJobs = true; // Whether to display jobs for empty job triggers
            options.DisplayHead = false; // Whether to display the page header
            options.DefaultExpandAllJobs = false; // Whether to expand all jobs by default
            options.EnableDirectoryBrowsing = false; // Whether to enable directory browsing
            options.Title = "Settimetask board"; // Custom board title

            options.LoginConfig.OnLoging = async (username, password, httpContext) =>
            {
                var res = await httpContext.RequestServices.GetRequiredService<SysAuthService>().SwaggerSubmitUrl(new SpecificationAuth { UserName = username, Password = password });
                return res == 200;
            };
            options.LoginConfig.DefaultUsername = "";
            options.LoginConfig.DefaultPassword = "";
            options.LoginConfig.SessionKey = "schedule_session_key"; // Log in to the Session key stored on the client side
        });

        app.UseInject(string.Empty, options =>
        {
            foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
            {
                groupInfo.Description += "<br/><u><b><font color='FF0000'> 👮This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project! </font></b></u>";
            }
            options.ConfigureSwagger(m =>
            {
                m.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            });
        });

#if NET10_0_OR_GREATER
        app.UseAutoVersionUpdate();
#endif

        app.UseEndpoints(endpoints =>
        {
            // Configure Scalar third-party UI integration (consistent routing prefixes represent independence, different routing prefixes represent coexistence)
            if (App.GetConfig<bool>("AppSettings:InjectSpecificationDocument", true))
            {
                endpoints.MapScalarApiReference("sapi", options =>
                {
                    options.WithTitle("Admin.NET");

                    // Configure OpenAPI documentation
                    foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
                    {
                        options.AddDocument(groupInfo.Group, groupInfo.Title, groupInfo.RouteTemplate);
                    }
                });
            }
            // Register hub
            endpoints.MapHubs();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}