// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// SqlSugar second level cache
/// </summary>
public class SqlSugarCache : ICacheService
{
    /// <summary>
    /// System cache service
    /// </summary>
    private static readonly SysCacheService _cache = App.GetRequiredService<SysCacheService>();

    public void Add<V>(string key, V value)
    {
        _cache.Set($"{CacheConst.SqlSugar}{key}", value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        _cache.Set($"{CacheConst.SqlSugar}{key}", value, TimeSpan.FromSeconds(cacheDurationInSeconds));
    }

    public bool ContainsKey<V>(string key)
    {
        return _cache.ExistKey($"{CacheConst.SqlSugar}{key}");
    }

    public V Get<V>(string key)
    {
        return _cache.Get<V>($"{CacheConst.SqlSugar}{key}");
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        return _cache.GetKeysByPrefixKey(CacheConst.SqlSugar);
    }

    public V GetOrCreate<V>(string key, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        return _cache.GetOrAdd<V>($"{CacheConst.SqlSugar}{key}", (cacheKey) =>
        {
            return create();
        }, cacheDurationInSeconds);
    }

    public void Remove<V>(string key)
    {
        _cache.Remove(key); // When SqlSugar calls the Remove method, the key already contains the CacheConst.SqlSugar prefix.
    }
}