using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace K8sCore.Entities.Ef
{
    public class BaseEfEntity
    {
        public int Id { get; set; }
    }
}