// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using NewLife.Caching.Queues;

namespace Admin.NET.Core;

/// <summary>
/// Redis message queue
/// </summary>
public static class RedisQueue
{
    private static ICacheProvider _cacheProvider = App.GetRequiredService<ICacheProvider>();

    /// <summary>Create a Redis message queue. Consume once by default. Use the STREAM structure when specifying a consumer group to support multiple consumer groups sharing messages.</summary>
    /// <remarks>
    /// When using queues, you can decide whether to use a simple queue or a complete queue based on whether to set up a consumer group. Simple queues (such as RedisQueue) can be used as command queues, with many topics but almost no messages. A complete queue (such as RedisStream) can be used as a message queue, with few topics but many messages, and supports multiple consumer groups.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic">theme</param>
    /// <param name="group">consumer group. Use a simple queue (such as RedisQueue) when no consumer group is specified, and use a complete queue (such as RedisStream) when a consumer group is specified.</param>
    /// <returns></returns>
    public static IProducerConsumer<T> GetQueue<T>(String topic, String group = null)
    {
        // The queue requires a single column
        var key = $"myStream:{topic}";
        if (_cacheProvider.InnerCache.TryGetValue<IProducerConsumer<T>>(key, out var queue)) return queue;

        queue = _cacheProvider.GetQueue<T>(topic, group);
        _cacheProvider.Cache.Set(key, queue);

        return queue;
    }

    /// <summary>
    /// Get trusted queue, need to confirm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static RedisReliableQueue<T> GetRedisReliableQueue<T>(string topic)
    {
        // The queue requires a single column
        var key = $"myQueue:{topic}";
        if (_cacheProvider.InnerCache.TryGetValue<RedisReliableQueue<T>>(key, out var queue)) return queue;

        queue = (_cacheProvider.Cache as FullRedis).GetReliableQueue<T>(topic);
        _cacheProvider.Cache.Set(key, queue);

        return queue;
    }

    /// <summary>
    /// Trusted queue rollback
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="retryInterval"></param>
    /// <returns></returns>
    public static int RollbackAllAck(string topic, int retryInterval = 60)
    {
        var queue = GetRedisReliableQueue<string>(topic);
        queue.RetryInterval = retryInterval;
        return queue.RollbackAllAck();
    }

    /// <summary>
    /// Send a list of data to a trusted queue
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddReliableQueueList<T>(string topic, List<T> value)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        var count = queue.Count;
        var result = queue.Add(value.ToArray());
        return result - count;
    }

    /// <summary>
    /// Send a piece of data to a trusted queue
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddReliableQueue<T>(string topic, T value)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        var count = queue.Count;
        var result = queue.Add(value);
        return result - count;
    }

    /// <summary>
    /// Get delay queue
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static RedisDelayQueue<T> GetDelayQueue<T>(string topic)
    {
        // The queue requires a single column
        var key = $"myDelay:{topic}";
        if (_cacheProvider.InnerCache.TryGetValue<RedisDelayQueue<T>>(key, out var queue)) return queue;

        queue = (_cacheProvider.Cache as FullRedis).GetDelayQueue<T>(topic);
        _cacheProvider.Cache.Set(key, queue);

        return queue;
    }

    /// <summary>
    /// Send a piece of data to the delay queue
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <param name="delay">delay time. Unit second</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddDelayQueue<T>(string topic, T value, int delay)
    {
        var queue = GetDelayQueue<T>(topic);
        return queue.Add(value, delay);
    }

    /// <summary>
    /// Send a list of data to the delay queue
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <param name="delay"></param>
    /// <typeparam name="T">delay time. Unit second</typeparam>
    /// <returns></returns>
    public static int AddDelayQueue<T>(string topic, List<T> value, int delay)
    {
        var queue = GetDelayQueue<T>(topic);
        queue.Delay = delay;
        return queue.Add(value.ToArray());
    }

    /// <summary>
    /// Get a piece of data from the trusted queue
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ReliableTakeOne<T>(string topic)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return queue.TakeOne(1);
    }

    /// <summary>
    /// Asynchronously obtain a piece of data from the trusted queue
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> ReliableTakeOneAsync<T>(string topic)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return await queue.TakeOneAsync(1);
    }

    /// <summary>
    /// Get multiple pieces of data from the trusted queue
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> ReliableTake<T>(string topic, int count)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return queue.Take(count).ToList();
    }

    /// <summary>
    /// Apply for distributed lock
    /// </summary>
    /// <param name="key">key to lock</param>
    /// <param name="msTimeout">The waiting time to apply for a lock, in milliseconds</param>
    /// <param name="msExpire">The lock expiration time. If there is no active release after this time, it will be released automatically. It must be an integer number of seconds, and the unit is milliseconds.</param>
    /// <param name="throwOnFailure">Whether an exception is thrown when failure occurs. If no exception is thrown, you can know that the lock application failed by returning null.</param>
    /// <returns></returns>
    public static IDisposable? BeginCacheLock(string key, int msTimeout = 500, int msExpire = 10000, bool throwOnFailure = true)
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
}