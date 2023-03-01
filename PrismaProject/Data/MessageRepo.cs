using PrismaProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PrismaProject.Data;

public class MessageRepo : IMessageRepo
{
    private readonly IMongoCollection<Message> _msgCollection;

    public MessageRepo(
        IOptions<MessageStoreDbSettings> msgStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            msgStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            msgStoreDatabaseSettings.Value.DatabaseName);

        _msgCollection = mongoDatabase.GetCollection<Message>(
            msgStoreDatabaseSettings.Value.MsgCollectionName);
    }

    public async Task<List<Message>> GetAsync() =>
        await _msgCollection.Find(_ => true).ToListAsync();

    public async Task<Message?> GetAsync(string id) =>
        await _msgCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


    public async Task CreateAsync(Message msg)
    {
        Console.WriteLine("Create Async Mongo");
        await _msgCollection.InsertOneAsync(msg);
    }
        

    public Task UpdateAsync(string id, Message msg)
    {
        throw new NotImplementedException();
    }


    public async Task RemoveAsync(string id) =>
        await _msgCollection.DeleteOneAsync(x => x.Id == id);
}