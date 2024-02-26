using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Vab.NetLibrary.Hosting.RestartableServices;

namespace Vab.NetLibrary.Hosting;

public abstract class SimpleQueueManagerBase<T> : RestartableService
{
    private readonly Channel<T> _channel;
    private readonly ILogger _logger;

    protected SimpleQueueManagerBase(ILogger logger, ISleepDurationProvider? sleepDurationProvider = null) 
        : base(logger, sleepDurationProvider)
    {
        _channel = Channel.CreateUnbounded<T>();
        _logger = logger;
    }

    protected ValueTask EnqueueItem(T queueItem)
        => _channel.Writer.WriteAsync(queueItem);

    protected override async Task ExecuteAsync(CancellationTokenSource reloadTokenSource)
    {
        try
        {
            await Worker(_channel.Reader.ReadAllAsync(reloadTokenSource.Token), reloadTokenSource.Token);
        }
        catch (Exception e)
        {
             _logger.UnhandledException(ServiceName, e);
            throw;
        }
    }

    protected abstract Task Worker(IAsyncEnumerable<T> stream, CancellationToken ct);
}