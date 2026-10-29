using ClyvoElo.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace ClyvoElo.Infrastructure.Mongo;

public static class MongoDbMapeamentos
{
    private static readonly object Trava = new();
    private static bool _registrado;

    public static void Registrar()
    {
        lock (Trava)
        {
            if (_registrado)
            {
                return;
            }

            var convencoes = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("ClyvoEloConvencoes", convencoes, tipo => tipo.Namespace?.StartsWith("ClyvoElo") == true);

            BsonClassMap.TryRegisterClassMap<MensagemAcompanhamento>(mapa =>
            {
                mapa.AutoMap();
                mapa.MapIdMember(m => m.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                mapa.MapMember(m => m.DataEnvio).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
                mapa.MapMember(m => m.DataLeitura).SetSerializer(new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Utc)));
            });

            _registrado = true;
        }
    }
}
