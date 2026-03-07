// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace Admin.NET.Core;

/// <summary>
/// RabbitMQ custom event source storage
/// </summary>
public class RabbitMQEventSourceStore : IEventSourceStorer, IDisposable
{
    /// <summary>
    /// Memory channel event source memory
    /// </summary>
    private Channel<IEventSource> _channelEventSource;

    /// <summary>
    /// routing key
    /// </summary>
    private string _routeKey;

    /// <summary>
    /// connection object
    /// </summary>
    private IConnection _connection;

    /// <summary>
    /// channel object
    /// </summary>
    private IChannel _channel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">connection factory</param>
    /// <param name="routeKey">routing key</param>
    /// <param name="capacity">The maximum number of messages that the memory can process. If it exceeds this capacity, it will wait for writing.</param>
    public RabbitMQEventSourceStore(ConnectionFactory factory, string routeKey, int capacity)
    {
        InitEventSourceStore(factory, routeKey, capacity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Initialize event source memory
    /// </summary>
    /// <param name="factory">connection factory</param>
    /// <param name="routeKey">routing key</param>
    /// <param name="capacity">The maximum number of messages that the memory can process. If it exceeds this capacity, it will wait for writing.</param>
    private async Task InitEventSourceStore(ConnectionFactory factory, string routeKey, int capacity)
    {
        // Configure the channel (wait after exceeding the default capacity)
        var boundedChannelOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        // Create a limited capacity channel
        _channelEventSource = Channel.CreateBounded<IEventSource>(boundedChannelOptions);

        // Create connection
        _connection = await factory.CreateConnectionAsync();
        // Routing key name
        _routeKey = routeKey;

        // Create channel
        _channel = await _connection.CreateChannelAsync();

        // Declare routing queue
        await _channel.QueueDeclareAsync(routeKey, false, false, false, null);

        // Create message subscriber
        var consumer = new AsyncEventingBasicConsumer(_channel);

        // Subscribe to messages and write to memory Channel
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            // Read the original message
            var stringEventSource = Encoding.UTF8.GetString(ea.Body.ToArray());

            // Convert to IEventSource. If EventSource is customized, note that the properties are readable and writable.
            var eventSource = JSON.Deserialize<ChannelEventSource>(stringEventSource);

            // Write to memory pipeline storage
            await _channelEventSource.Writer.WriteAsync(eventSource);

            // Confirm that the message has been consumed
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        // Start the consumer and set it to answer messages manually
        await _channel.BasicConsumeAsync(routeKey, false, consumer);
    }

    /// <summary>
    /// Write event source to memory
    /// </summary>
    /// <param name="eventSource">event source object</param>
    /// <param name="cancellationToken">Cancel task token</param>
    /// <returns><see cref="ValueTask"/></returns>
    public async ValueTask WriteAsync(IEventSource eventSource, CancellationToken cancellationToken)
    {
        if (eventSource == default)
            throw new ArgumentNullException(nameof(eventSource));

        // Determine whether it is a ChannelEventSource or a custom EventSource
        if (eventSource is ChannelEventSource source)
        {
            // Serialization and publishing
            var data = Encoding.UTF8.GetBytes(JSON.Serialize(source));
            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;
            await _channel.BasicPublishAsync("", _routeKey, false, props, data);
        }
        else
        {
            // Handling dynamic subscriptions
            await _channelEventSource.Writer.WriteAsync(eventSource, cancellationToken);
        }
    }

    /// <summary>
    /// Read an event source from memory
    /// </summary>
    /// <param name="cancellationToken">Cancel task token</param>
    /// <returns>event source object</returns>
    public async ValueTask<IEventSource> ReadAsync(CancellationToken cancellationToken)
    {
        var eventSource = await _channelEventSource.Reader.ReadAsync(cancellationToken);
        return eventSource;
    }

    /// <summary>
    /// Release unmanaged resources
    /// </summary>
    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}