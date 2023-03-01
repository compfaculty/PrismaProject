using PrismaProject.Models;

namespace PrismaProject.Data;

public interface IMessageRepo
{
    public Task<List<Message>> GetAsync();
    public Task<Message?> GetAsync(string id);
    public Task CreateAsync(Message msg);
    public Task UpdateAsync(string id, Message msg);
    public Task RemoveAsync(string id);
}