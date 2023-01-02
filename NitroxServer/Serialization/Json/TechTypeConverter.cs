﻿using System;
using Newtonsoft.Json;
using NitroxModel.DataStructures.GameLogic;

namespace NitroxServer.Serialization.Json;

public class TechTypeConverter : JsonConverter<NitroxTechType>
{
    public override void WriteJson(JsonWriter writer, NitroxTechType value, JsonSerializer serializer)
    {
        if (value == null || string.IsNullOrEmpty(value.Name))
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value.Name);
        }
    }
    public override NitroxTechType ReadJson(JsonReader reader, Type objectType, NitroxTechType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.TokenType == JsonToken.Null ? null : new NitroxTechType((string)reader.Value);
    }
}
