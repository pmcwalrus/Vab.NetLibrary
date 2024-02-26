using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Vab.NetLibrary.Hosting.RestartableServices;

public abstract class RestartableService : BackgroundService
{
    private readonly AsyncRetryPolicy _retryPolicy;

    protected ILogger Logger { get; }

    protected virtual string ServiceName => GetType().Name;

    protected RestartableService(ILogger logger, ISleepDurationProvider? sleepDurationProvider = null)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var delayProvider = sleepDurationProvider ?? DefaultSleepDurationProvider.Instance;

        _retryPolicy = Policy
            .Handle<Exception>(e => e is not OperationCanceledException)
            .WaitAndRetryForeverAsync(
                delayProvider.GetSleepDelay,
                (e, retryNumber, _) =>
                {
                    if (e is ChannelClosedException && e.InnerException != null)
                        e = e.InnerException;

                    Logger.RestartingService(ServiceName, retryNumber, e);
                });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        _retryPolicy.ExecuteAsync(ExecuteLoop, stoppingToken);

    protected abstract Task ExecuteAsync(CancellationTokenSource reloadTokenSource);

    internal virtual async Task ExecuteLoop(CancellationToken stoppingToken)
    {
        while (true)
        {
            using var reloadTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, default);

            try
            {
                Logger.StartingService(ServiceName);
                await ExecuteAsync(reloadTokenSource);
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                    throw;

                if (reloadTokenSource.IsCancellationRequested)
                    Logger.ConfigurationChanged(ServiceName);
            }
            catch (Exception)
            {
                reloadTokenSource.Cancel();
                throw;
            }
        }
    }
}