using Microsoft.Extensions.Logging;

namespace Vab.NetLibrary.Hosting;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> _startingService =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(StartingService)),
            "Starting service {service}.");

    private static readonly Action<ILogger, string, int, Exception> _restartingService =
        LoggerMessage.Define<string, int>(LogLevel.Error, new EventId(2, nameof(RestartingService)),
            "Restarting service {hostedService} due to an exception. Retry #{retryNumber}.");

    private static readonly Action<ILogger, string, Exception> _configurationChanged =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(3, nameof(ConfigurationChanged)),
            "Restarting service {hostedService} due to a configuration change.");

    private static readonly Action<ILogger, string, Exception> _unhandledException =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(4, nameof(UnhandledException)),
            "An unhandled exception occured at service {hostedService}.");

    public static void StartingService(this ILogger logger, string hostedService) =>
        _startingService(logger, hostedService, null!);

    public static void RestartingService(this ILogger logger, string hostedService, int retryNumber,
        Exception exception) =>
        _restartingService(logger, hostedService, retryNumber, exception);

    public static void ConfigurationChanged(this ILogger logger, string hostedService) =>
        _configurationChanged(logger, hostedService, null!);

    public static void UnhandledException(this ILogger logger, string hostedService, Exception exception) =>
        _unhandledException(logger, hostedService, exception);
}