namespace Vab.NetLibrary.Hosting.RestartableServices;

public interface ISleepDurationProvider
{
    TimeSpan GetSleepDelay(int retryNumber);
}
