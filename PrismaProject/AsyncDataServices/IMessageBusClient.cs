using PrismaProject.Dto;

namespace PrismaProject.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewMessage(MessagePublishDto msg);
}