using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BookkeepingNasheDetstvo.Shared.Serializers
{
    public class CustomMongoDecimalSerializer : SerializerBase<decimal>
    {
        private const decimal DECIMAL_PLACE = 10000m;

        public override decimal Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Math.Round(context.Reader.ReadInt64() / DECIMAL_PLACE, 4);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value)
        {
            context.Writer.WriteInt64(Convert.ToInt64(value * DECIMAL_PLACE));
        }
    }
}