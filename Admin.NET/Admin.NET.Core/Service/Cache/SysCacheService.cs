// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// System cache service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 400, Description = "System cache")]
public class SysCacheService : IDynamicApiController, ISingleton
{
    private static ICacheProvider _cacheProvider;
    private readonly CacheOptions _cacheOptions;

    public SysCacheService(ICacheProvider cacheProvider, IOptions<CacheOptions> cacheOptions)
    {
        _cacheProvider = cacheProvider;
        _cacheOptions = cacheOptions.Value;
    }

    /// <summary>
    /// Apply for distributed lock 🔖
    /// </summary>
    /// <param name="key">key to lock</param>
    /// <param name="msTimeout">The waiting time to apply for a lock, in milliseconds</param>
    /// <param name="msExpire">The lock expiration time. If there is no active release after this time, it will be released automatically. It must be an integer number of seconds, and the unit is milliseconds.</param>
    /// <param name="throwOnFailure">Whether an exception is thrown when failure occurs. If no exception is thrown, you can know that the lock application failed by returning null.</param>
    /// <returns></returns>
    [DisplayName("ApplyPleasepointsDistributed lock")]
    public IDisposable? BeginCacheLock(string key, int msTimeout = 500, int msExpire = 10000, bool throwOnFailure = true)
    {
        try
        {
            return _cacheProvider.Cache.AcquireLock(key, msTimeout, msExpire, throwOnFailure);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get cache key name set 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Get the collection of cache key names")]
    public List<string> GetKeyList()
    {
        return _cacheProvider.Cache == Cache.Default
            ? [.. _cacheProvider.Cache.Keys.Where(u => u.StartsWith(_cacheOptions.Prefix)).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u)]
            : [.. ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}*", 0, int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u)];
    }

    /// <summary>
    /// Increase cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [NonAction]
    public bool Set(string key, object value)
    {
        return !string.IsNullOrWhiteSpace(key) && _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value);
    }

    /// <summary>
    /// Increase cache and set expiration time
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    [NonAction]
    public bool Set(string key, object value, TimeSpan expire)
    {
        return !string.IsNullOrWhiteSpace(key) && _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value, expire);
    }

    public async Task<TR> AdGetAsync<TR>(String cacheName, Func<Task<TR>> del, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1>(String cacheName, Func<T1, Task<TR>> del, T1 t1, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1, T2>(String cacheName, Func<T1, T2, Task<TR>> del, T1 t1, T2 t2, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1, t2], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1, T2, T3>(String cacheName, Func<T1, T2, T3, Task<TR>> del, T1 t1, T2 t2, T3 t3, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1, t2, t3], expiry);
    }

    private async Task<T> AdGetAsync<T>(string cacheName, Delegate del, Object[] obs, TimeSpan? expiry) where T : class
    {
        var key = Key(cacheName, obs);
        // Use distributed locks
        using (_cacheProvider.Cache.AcquireLock($@"lock:AdGetAsync:{cacheName}", 1000))
        {
            var value = Get<T>(key);
            if (value == null)
            {
                value = await ((dynamic)del).DynamicInvoke(obs);
                if (expiry == null)
                {
                    Set(key, value);
                }
                else
                {
                    Set(key, value, (TimeSpan)expiry);
                }
            }
            return value;
        }
    }

    public T Get<T>(String cacheName, object t1)
    {
        return Get<T>(cacheName, [t1]);
    }

    public T Get<T>(String cacheName, object t1, object t2)
    {
        return Get<T>(cacheName, [t1, t2]);
    }

    public T Get<T>(String cacheName, object t1, object t2, object t3)
    {
        return Get<T>(cacheName, [t1, t2, t3]);
    }

    private T Get<T>(String cacheName, Object[] obs)
    {
        var key = Key(cacheName, obs);
        return Get<T>(key);
    }

    private static string Key(string cacheName, object[] obs)
    {
        if (obs.OfType<TimeSpan>().Any()) throw new Exception("The cache parameter type cannot be: TimeSpan type");
        StringBuilder sb = new(cacheName);
        if (obs is { Length: > 0 })
        {
            sb.Append(':');
            foreach (var a in obs) sb.Append($"<{KeySingle(a)}>");
        }
        return sb.ToString();
    }

    private static string KeySingle(object t)
    {
        return t.GetType().IsClass && !t.GetType().IsPrimitive ? JsonConvert.SerializeObject(t) : t.ToString();
    }

    /// <summary>
    /// Get the remaining lifetime of the cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public TimeSpan GetExpire(string key)
    {
        return _cacheProvider.Cache.GetExpire(key);
    }

    /// <summary>
    /// Get cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public T Get<T>(string key)
    {
        return _cacheProvider.Cache.Get<T>($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// Delete cache 🔖
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("Clear cache")]
    public int Remove(string key)
    {
        return _cacheProvider.Cache.Remove($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// Clear all cache 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("Clear all cache")]
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    public void Clear()
    {
        _cacheProvider.Cache.Clear();

        Cache.Default.Clear();
    }

    /// <summary>
    /// Check if cache exists
    /// </summary>
    /// <param name="key">key</param>
    /// <returns></returns>
    [NonAction]
    public bool ExistKey(string key)
    {
        return _cacheProvider.Cache.ContainsKey($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// Delete cache based on key name prefix 🔖
    /// </summary>
    /// <param name="prefixKey">Key name prefix</param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteByPreKey"), HttpPost]
    [DisplayName("Delete cache based on key prefix")]
    public int RemoveByPrefixKey(string prefixKey)
    {
        var delKeys = _cacheProvider.Cache == Cache.Default
            ? _cacheProvider.Cache.Keys.Where(u => u.StartsWith($"{_cacheOptions.Prefix}{prefixKey}")).ToArray()
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", 0, int.MaxValue).ToArray();
        return _cacheProvider.Cache.Remove(delKeys);
    }

    /// <summary>
    /// Get the key name set based on the key name prefix 🔖
    /// </summary>
    /// <param name="prefixKey">Key name prefix</param>
    /// <returns></returns>
    [DisplayName("Get the key name set based on the key name prefix")]
    public List<string> GetKeysByPrefixKey(string prefixKey)
    {
        return _cacheProvider.Cache == Cache.Default
            ? _cacheProvider.Cache.Keys.Where(u => u.StartsWith($"{_cacheOptions.Prefix}{prefixKey}")).Select(u => u[_cacheOptions.Prefix.Length..]).ToList()
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", 0, int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).ToList();
    }

    /// <summary>
    /// Get cached value 🔖
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [DisplayName("Get cache value")]
    public object GetValue(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        if (Regex.IsMatch(key, @"%[0-9a-fA-F]{2}"))
            key = HttpUtility.UrlDecode(key);

        var fullKey = $"{_cacheOptions.Prefix}{key}";

        if (_cacheProvider.Cache == Cache.Default)
            return _cacheProvider.Cache.Get<object>(fullKey);

        if (_cacheProvider.Cache is FullRedis redisCache)
        {
            if (!redisCache.ContainsKey(fullKey))
                return null;
            try
            {
                var keyType = redisCache.TYPE(fullKey)?.ToLower();
                switch (keyType)
                {
                    case "string":
                        return redisCache.Get<string>(fullKey);

                    case "list":
                        var list = redisCache.GetList<string>(fullKey);
                        return list?.ToList();

                    case "hash":
                        var hash = redisCache.GetDictionary<string>(fullKey);
                        return hash?.ToDictionary(k => k.Key, v => v.Value);

                    case "set":
                        var set = redisCache.GetSet<string>(fullKey);
                        return set?.ToArray();

                    case "zset":
                        var sortedSet = redisCache.GetSortedSet<string>(fullKey);
                        return sortedSet?.Range(0, -1)?.ToList();

                    case "none":
                        return null;

                    default:
                        // Unknown type or special type
                        return new Dictionary<string, object>
                        {
                            { "key", key },
                            { "type", keyType ?? "unknown" },
                            { "message", "NoneUse the standard methodObtainthisTypeData" }
                        };
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "key", key },
                    { "error", ex.Message },
                    { "type", "exception" }
                };
            }
        }

        return _cacheProvider.Cache.Get<object>(fullKey);
    }

    /// <summary>
    /// Get or add cache (execute delegate to request data when the data does not exist)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <param name="expire">Expiration time in seconds</param>
    /// <returns></returns>
    [NonAction]
    public T GetOrAdd<T>(string key, Func<string, T> callback, int expire = -1)
    {
        if (string.IsNullOrWhiteSpace(key)) return default;
        return _cacheProvider.Cache.GetOrAdd($"{_cacheOptions.Prefix}{key}", callback, expire);
    }

    /// <summary>
    /// Hash matching
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public IDictionary<String, T> GetHashMap<T>(string key)
    {
        return _cacheProvider.Cache.GetDictionary<T>(key);
    }

    /// <summary>
    /// Add HASH in batches
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    [NonAction]
    public bool HashSet<T>(string key, Dictionary<string, T> dic)
    {
        var hash = GetHashMap<T>(key);
        foreach (var v in dic)
        {
            hash.Add(v);
        }
        return true;
    }

    /// <summary>
    /// Add a HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="hashKey"></param>
    /// <param name="value"></param>
    [NonAction]
    public void HashAdd<T>(string key, string hashKey, T value)
    {
        var hash = GetHashMap<T>(key);
        hash.Add(hashKey, value);
    }

    /// <summary>
    /// Add or update a HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="hashKey"></param>
    /// <param name="value"></param>
    [NonAction]
    public void HashAddOrUpdate<T>(string key, string hashKey, T value)
    {
        var hash = GetHashMap<T>(key);
        if (hash.ContainsKey(hashKey))
            hash[hashKey] = value;
        else
            hash.Add(hashKey, value);
    }

    /// <summary>
    /// Get multiple HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    [NonAction]
    public List<T> HashGet<T>(string key, params string[] fields)
    {
        var hash = GetHashMap<T>(key);
        return hash.Where(t => fields.Any(c => t.Key == c)).Select(t => t.Value).ToList();
    }

    /// <summary>
    /// Get a HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [NonAction]
    public T HashGetOne<T>(string key, string field)
    {
        var hash = GetHashMap<T>(key);
        return hash.TryGetValue(field, out T value) ? value : default;
    }

    /// <summary>
    /// Get all HASH based on KEY
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public IDictionary<string, T> HashGetAll<T>(string key)
    {
        var hash = GetHashMap<T>(key);
        return hash;
    }

    /// <summary>
    /// Delete HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    [NonAction]
    public int HashDel<T>(string key, params string[] fields)
    {
        var hash = GetHashMap<T>(key);
        fields.ToList().ForEach(t => hash.Remove(t));
        return fields.Length;
    }

    ///// <summary>
    /////Search for HASH
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="key"></param>
    ///// <param name="searchModel"></param>
    ///// <returns></returns>
    //[NonAction]
    //public List<KeyValuePair<string, T>> HashSearch<T>(string key, SearchModel searchModel)
    //{
    //    var hash = GetHashMap<T>(key);
    //    return hash.Search(searchModel).ToList();
    //}

    ///// <summary>
    /////Search for HASH
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="key"></param>
    ///// <param name="pattern"></param>
    ///// <param name="count"></param>
    ///// <returns></returns>
    //[NonAction]
    //public List<KeyValuePair<string, T>> HashSearch<T>(string key, string pattern, int count)
    //{
    //    var hash = GetHashMap<T>(key);
    //    return hash.Search(pattern, count).ToList();
    //}
}