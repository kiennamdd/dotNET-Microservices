using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Discount.API.Domain.Common
{
    public abstract class EntityBase<Key>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Key Id { get; protected set; } = default!;
    }
}
