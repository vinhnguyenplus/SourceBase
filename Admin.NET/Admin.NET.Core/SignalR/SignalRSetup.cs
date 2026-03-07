// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Furion.Logging.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Admin.NET.Core;

public static class SignalRSetup
{
    /// <summary>
    /// Instant messaging SignalR registration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="SetNewtonsoftJsonSetting"></param>
    public static void AddSignalR(this IServiceCollection services, Action<JsonSerializerSettings> SetNewtonsoftJsonSetting)
    {
        var signalRBuilder = services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
            options.KeepAliveInterval = TimeSpan.FromMinutes(1);
            options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // Data packet size is 10M, the default maximum is 32K
        }).AddNewtonsoftJsonProtocol(options => SetNewtonsoftJsonSetting(options.PayloadSerializerSettings));

        // If Redis cache is not enabled, return directly
        var cacheOptions = App.GetConfig<CacheOptions>("Cache", true);
        if (cacheOptions.CacheType != CacheTypeEnum.Redis.ToString())
            return;

        // If cluster configuration is enabled, configure SignalR to support cluster mode.
        var clusterOpt = App.GetConfig<ClusterOptions>("Cluster", true);
        if (!clusterOpt.Enabled)
            return;

        var redisOptions = clusterOpt.SentinelConfig;
        ConnectionMultiplexer connection1;
        if (clusterOpt.IsSentinel) // Sentry mode
        {
            var redisConfig = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ServiceName = redisOptions.ServiceName,
                AllowAdmin = true,
                DefaultDatabase = redisOptions.DefaultDb,
                Password = redisOptions.Password
            };
            redisOptions.EndPoints.ForEach(u => redisConfig.EndPoints.Add(u));
            connection1 = ConnectionMultiplexer.Connect(redisConfig);
        }
        else
        {
            connection1 = ConnectionMultiplexer.Connect(clusterOpt.SignalR.RedisConfiguration);
        }
        // Key storage (data protection)
        services.AddDataProtection().PersistKeysToStackExchangeRedis(connection1, clusterOpt.DataProtecteKey);

        signalRBuilder.AddStackExchangeRedis(options =>
        {
            // The ChannelPrefix set here will not take effect. If two different projects have the same [assembly name + class name] and use the same redis service, please pay attention to modifying the class name of Hub/OnlineUserHub.
            // Please refer to the link below for the reason:
            // https://github.com/dotnet/aspnetcore/blob/f9121bc3e976ec40a959818451d126d5126ce868/src/SignalR/server/StackExchangeRedis/src/RedisHubLifetimeManager.cs#L74
            // https://github.com/dotnet/aspnetcore/blob/f9121bc3e976ec40a959818451d126d5126ce868/src/SignalR/server/StackExchangeRedis/src/Internal/RedisChannels.cs#L33
            options.Configuration.ChannelPrefix = new RedisChannel(clusterOpt.SignalR.ChannelPrefix, RedisChannel.PatternMode.Auto);
            options.ConnectionFactory = async writer =>
            {
                ConnectionMultiplexer connection;
                if (clusterOpt.IsSentinel)
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        ServiceName = redisOptions.ServiceName,
                        AllowAdmin = true,
                        DefaultDatabase = redisOptions.DefaultDb,
                        Password = redisOptions.Password
                    };
                    redisOptions.EndPoints.ForEach(u => config.EndPoints.Add(u));
                    connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                }
                else
                {
                    connection = await ConnectionMultiplexer.ConnectAsync(clusterOpt.SignalR.RedisConfiguration);
                }

                connection.ConnectionFailed += (_, e) =>
                {
                    "Failed to connect to Redis".LogError();
                };

                if (!connection.IsConnected)
                {
                    "NoneLegal connection Redis".LogError();
                }
                return connection;
            };
        });
    }
}