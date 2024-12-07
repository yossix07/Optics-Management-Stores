using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;


namespace OMSAPI.General
{
    
    public class TimeOnlySerializer : IBsonSerializer<TimeOnly>
    {
        public Type ValueType => typeof(TimeOnly);

        public TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // Read the BSON representation of the time as a long.
            var ticks = context.Reader.ReadInt64();

            // Construct a new TimeOnly instance from the ticks.
            return new TimeOnly(ticks);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
        {
            // Write the ticks of the TimeOnly value to the BSON stream as a long.
            context.Writer.WriteInt64(value.Ticks);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                var timeOnly = (TimeOnly)value;
                Serialize(context, args, timeOnly);
            }
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Dispose()
        {
            // No resources need to be disposed.
            return;
        }
    }
}
