using System;
using Newtonsoft.Json;
using NitroxModel.DataStructures.GameLogic;

namespace NitroxServer_Subnautica.Serialization.Json;

public class SubnauticaTechTypeConverter : JsonConverter<NitroxTechType>
{
    public override void WriteJson(JsonWriter writer, NitroxTechType value, JsonSerializer serializer)
    {
        if (value == null || string.IsNullOrEmpty(value.Name))
        {
            writer.WriteNull();
        }
        else if (Enum.TryParse(value.Name, false, out TechType result))
        {
            writer.WriteValue((int)result);
        }
        else
        {
            writer.WriteValue(value.Name);
        }
    }

    public override NitroxTechType ReadJson(JsonReader reader, Type objectType, NitroxTechType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.Integer)
        {
            TechType techType = (TechType)Enum.ToObject(typeof(TechType), reader.Value);
            return new NitroxTechType(techType.ToString());
        }

        string techTypeName = (string)reader.Value;
        return new NitroxTechType(techTypeName);
    }
}
