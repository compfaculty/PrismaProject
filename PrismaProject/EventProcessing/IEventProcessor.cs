namespace PrismaProject.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}