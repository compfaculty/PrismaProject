namespace PrismaProject.Services;

public interface IProducerService
{
    Task SendMessageAsync(string message);
}