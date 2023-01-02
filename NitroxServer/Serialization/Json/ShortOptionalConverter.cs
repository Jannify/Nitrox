using System;
using System.Reflection;
using Newtonsoft.Json;
using NitroxModel.DataStructures.Util;

namespace NitroxServer.Serialization.Json;

public class ShortOptionalConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        dynamic optional = value;

        if (optional.HasValue)
        {
            serializer.Serialize(writer, optional.Value, value.GetType().GetGenericArguments()[0]);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return objectType.GetMethod("OfNullable", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { null });
        }

        object value = serializer.Deserialize(reader, objectType.GetGenericArguments()[0]);
        return objectType.GetMethod("Of", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new[] { value });
    }
}
