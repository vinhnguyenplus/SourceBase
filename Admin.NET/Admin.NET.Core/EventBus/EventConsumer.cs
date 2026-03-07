// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Redis message extension
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventConsumer<T> : IDisposable
{
    /// <summary>
    ///
    /// </summary>
    private Task _consumerTask;

    /// <summary>
    ///
    /// </summary>
    private CancellationTokenSource _consumerCts;

    /// <summary>
    /// consumer
    /// </summary>
    public IProducerConsumer<T> Consumer { get; }

    /// <summary>
    /// Message callback
    /// </summary>
    public event EventHandler<T> Received;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="consumer"></param>
    public EventConsumer(IProducerConsumer<T> consumer) => Consumer = consumer;

    /// <summary>
    /// start up
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Start()
    {
        if (Consumer is null)
        {
            throw new InvalidOperationException("Subscribe first using the Consumer.Subscribe() function");
        }
        if (_consumerTask != null)
        {
            return;
        }
        _consumerCts = new CancellationTokenSource();
        var ct = _consumerCts.Token;
        _consumerTask = Task.Factory.StartNew(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var cr = Consumer.TakeOne(10);
                    if (cr == null) continue;
                    Received?.Invoke(this, cr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message consumption exception: {ex.Message}");
                    await Task.Delay(1000); // Wait briefly and try again
                }
            }
        }, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// stop
    /// </summary>
    /// <returns></returns>
    public async Task Stop()
    {
        if (_consumerCts == null || _consumerTask == null) return;
        _consumerCts.Cancel();
        try
        {
            await _consumerTask;
        }
        finally
        {
            _consumerTask = null;
            _consumerCts = null;
        }
    }

    /// <summary>
    /// release
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// release
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_consumerTask != null)
            {
                Stop().Wait();
            }
        }
    }
}