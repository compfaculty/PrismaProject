using Microsoft.Extensions.Logging;

namespace PrismaProject.Services;

public class Producer : IProducerService
{
    private readonly ILogger<Producer> _logger;
    public Producer(ILogger<Producer> logger)
    {
        _logger = logger;
    }
    public async Task SendMessageAsync(string message)
    {
        _logger.LogInformation("SendMessageAsync");
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}