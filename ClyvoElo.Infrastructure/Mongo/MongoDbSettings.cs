namespace ClyvoElo.Infrastructure.Mongo;

public class MongoDbSettings
{
    public const string Secao = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "clyvo_elo";
    public string ColecaoMensagens { get; set; } = "mensagens_acompanhamento";
}
