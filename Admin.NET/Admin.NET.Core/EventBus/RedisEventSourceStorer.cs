// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using NewLife.Caching.Queues;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace Admin.NET.Core;

/// <summary>
/// Redis custom event source storage
/// </summary>
/// <remarks>
/// When deployed in a cluster, each message is generally consumed only once by one service node.
/// There are some special situations that need to be notified to every node in the server group (such as the need to force the loading of certain configurations, key services, etc.).
/// In this case, EventId must be defined starting with "broadcast:".
/// This system will treat events starting with "broadcast:" as "broadcast messages" to ensure that every service node in the cluster can consume this message.
/// </remarks>
public sealed class RedisEventSourceStorer : IEventSourceStorer, IDisposable
{
    /// <summary>
    /// consumer
    /// </summary>
    private readonly EventConsumer<ChannelEventSource> _eventConsumer;

    /// <summary>
    /// Memory channel event source memory
    /// </summary>
    private readonly Channel<IEventSource> _channel;

    private IProducerConsumer<ChannelEventSource> _queueSingle;

    private RedisStream<string> _queueBroadcast;

    private ILogger<RedisEventSourceStorer> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="cacheProvider">Redis connection object</param>
    /// <param name="routeKey">routing key</param>
    /// <param name="capacity">The maximum number of messages that the memory can process. If it exceeds this capacity, it will wait for writing.</param>
    public RedisEventSourceStorer(ICacheProvider cacheProvider, string routeKey, int capacity)
    {
        _logger = App.GetRequiredService<ILogger<RedisEventSourceStorer>>();

        // Configure the channel and wait after the setting exceeds the default capacity.
        var boundedChannelOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // Create a limited capacity channel
        _channel = Channel.CreateBounded<IEventSource>(boundedChannelOptions);

        //_redis = redis as FullRedis;

        // Create a broadcast message subscriber, that is, all server nodes can receive the message (used to publish restart, reload configuration, etc. messages)
        FullRedis redis = (FullRedis)cacheProvider.Cache;
        var clusterOpt = App.GetConfig<ClusterOptions>("Cluster", true);
        _queueBroadcast = redis.GetStream<string>(routeKey + ":broadcast");
        _queueBroadcast.Group = clusterOpt.ServerId;// Assigned to different groups based on server ID
        _queueBroadcast.Expire = TimeSpan.FromSeconds(10);// Message expires in 10 seconds()
        _queueBroadcast.ConsumeAsync(OnConsumeBroadcast);

        // Create a queue message subscriber, as long as one service node consumes the message
        _queueSingle = redis.GetQueue<ChannelEventSource>(routeKey + ":single");
        _eventConsumer = new EventConsumer<ChannelEventSource>(_queueSingle);

        // Subscription messages are written to Channel
        _eventConsumer.Received += async (send, cr) =>
        {
            // var oriColor = Console.ForegroundColor;
            try
            {
                ChannelEventSource ces = (ChannelEventSource)cr;
                await ConsumeChannelEventSourceAsync(ces, ces.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the message in Received!");
            }
        };
        _eventConsumer.Start();
    }

    private async Task OnConsumeBroadcast(string source, Message message, CancellationToken token)
    {
        ChannelEventSource ces = JsonConvert.DeserializeObject<ChannelEventSource>(source);
        await ConsumeChannelEventSourceAsync(ces, token);
    }

    private async Task ConsumeChannelEventSourceAsync(ChannelEventSource ces, CancellationToken cancel = default)
    {
        // Print test events
        if (ces.EventId != null && ces.EventId.IndexOf(":Test") > 0)
        {
            var oriColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"haveinformationTo handle{ces.EventId},{ces.Payload}");
            Console.ForegroundColor = oriColor;
        }
        await _channel.Writer.WriteAsync(ces, cancel);
    }

    /// <summary>
    /// Write event source to memory
    /// </summary>
    /// <param name="eventSource">event source object</param>
    /// <param name="cancellationToken">Cancel task token</param>
    /// <returns><see cref="ValueTask"/></returns>
    public async ValueTask WriteAsync(IEventSource eventSource, CancellationToken cancellationToken)
    {
        // empty check
        if (eventSource == default)
            throw new ArgumentNullException(nameof(eventSource));

        // Determine whether it is a ChannelEventSource or a custom EventSource.
        if (eventSource is ChannelEventSource source)
        {
            // Asynchronous publishing
            await Task.Factory.StartNew(() =>
            {
                if (source.EventId != null && source.EventId.StartsWith("broadcast:"))
                {
                    string str = JsonConvert.SerializeObject(source);
                    _queueBroadcast.Add(str);
                }
                else
                {
                    _queueSingle.Add(source);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);
        }
        else
        {
            // Handling dynamic subscription issues
            await _channel.Writer.WriteAsync(eventSource, cancellationToken);
        }
    }

    /// <summary>
    /// Read an event source from memory
    /// </summary>
    /// <param name="cancellationToken">Cancel task token</param>
    /// <returns>event source object</returns>
    public async ValueTask<IEventSource> ReadAsync(CancellationToken cancellationToken)
    {
        // Read an event source
        var eventSource = await _channel.Reader.ReadAsync(cancellationToken);
        return eventSource;
    }

    /// <summary>
    /// Release unmanaged resources
    /// </summary>
    public async void Dispose()
    {
        await _eventConsumer.Stop();
        GC.SuppressFinalize(this);
    }
}