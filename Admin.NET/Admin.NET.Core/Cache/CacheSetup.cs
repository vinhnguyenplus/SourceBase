// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Microsoft.Extensions.DependencyInjection.Extensions;
using NewLife.Caching.Services;

namespace Admin.NET.Core;

public static class CacheSetup
{
    /// <summary>
    /// Cache registration (new life Redis component)
    /// </summary>
    /// <param name="services"></param>
    public static void AddCache(this IServiceCollection services)
    {
        var cacheOptions = App.GetConfig<CacheOptions>("Cache", true);
        if (cacheOptions.CacheType == CacheTypeEnum.Redis.ToString())
        {
            var redis = new FullRedis(new RedisOptions
            {
                Configuration = cacheOptions.Redis.Configuration,
                Prefix = cacheOptions.Redis.Prefix
            })
            {
                // Automatically detect cluster nodes
                AutoDetect = App.GetConfig<bool>("Cache:Redis:AutoDetect", true)
            };
            // Maximum message size
            if (cacheOptions.Redis.MaxMessageSize > 0)
                redis.MaxMessageSize = cacheOptions.Redis.MaxMessageSize;

            // Inject the Redis cache provider
            services.AddSingleton<ICacheProvider>(u => new RedisCacheProvider(u) { Cache = redis });
        }

        // Memory cache has the bottom line. When Redis is not configured, memory cache is used and the logic code does not need to be modified.
        services.TryAddSingleton<ICacheProvider, CacheProvider>();
    }
}