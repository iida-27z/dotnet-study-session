namespace StudySession.Batch;

public class Worker(ILogger<Worker> logger, TimeProvider timeProvider) : BackgroundService
{
    private readonly TimeProvider _timeProvider = timeProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", _timeProvider.GetUtcNow());
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
