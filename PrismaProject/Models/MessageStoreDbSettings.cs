namespace PrismaProject.Models;

public class MessageStoreDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string MsgCollectionName { get; set; } = null!;
}
