using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BookkeepingNasheDetstvo.Server.Serializers
{
    public class CustomMongoDecimalSerializer : SerializerBase<decimal>
    {
        private const decimal DecimalPlace = 10000m;

        public override decimal Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Math.Round(context.Reader.ReadInt64() / DecimalPlace, 4);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value)
        {
            context.Writer.WriteInt64(Convert.ToInt64(value * DecimalPlace));
        }
    }
}