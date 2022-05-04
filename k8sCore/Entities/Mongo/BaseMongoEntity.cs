using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace K8sCore.Entities.Mongo
{
    public class BaseMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}