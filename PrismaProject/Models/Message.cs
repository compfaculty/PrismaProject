using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace PrismaProject.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Msg")]
    public string Msg{ get; set; } = null!;
}