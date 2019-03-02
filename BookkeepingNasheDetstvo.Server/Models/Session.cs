using MongoDB.Bson.Serialization.Attributes;

namespace BookkeepingNasheDetstvo.Server.Models
{
    public class Session
    {
        [BsonId]
        public string Token;

        public string OwnerId;
    }
}
