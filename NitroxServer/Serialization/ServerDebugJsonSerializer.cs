using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NitroxServer.Serialization.Json;

namespace NitroxServer.Serialization;

public class ServerDebugJsonSerializer : ServerJsonSerializer
{
    public ServerDebugJsonSerializer()
    {
        Serializer = new JsonSerializer();

        Serializer.Error += delegate (object _, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Log.Error(e.ErrorContext.Error, "Json serialization error: ");
        };

        Serializer.TypeNameHandling = TypeNameHandling.Auto;
        Serializer.Formatting = Formatting.Indented;
        Serializer.ContractResolver = new AttributeContractResolver();
        Serializer.Converters.Add(new VersionConverter());
        Serializer.Converters.Add(new KeyValuePairConverter());
        Serializer.Converters.Add(new StringEnumConverter());
        Serializer.Converters.Add(new NitroxIdConverter());
        Serializer.Converters.Add(new TechTypeConverter());
    }
    
    public override string FileEnding => "_debug.json";
}
