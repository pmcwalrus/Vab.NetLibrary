namespace Vab.NetLibrary.Hosting.RestartableServices;

internal sealed class DefaultSleepDurationProvider : ISleepDurationProvider
{
    public static DefaultSleepDurationProvider Instance { get; } = new();

    public TimeSpan GetSleepDelay(int retryNumber) => TimeSpan.FromSeconds(Math.Min(retryNumber, 10));
}
