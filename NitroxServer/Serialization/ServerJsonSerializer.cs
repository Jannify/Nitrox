using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NitroxModel.Platforms.OS.Shared;
using NitroxServer.Serialization.Json;

namespace NitroxServer.Serialization;

public class ServerJsonSerializer : IServerSerializer
{
    protected JsonSerializer Serializer;

    public ServerJsonSerializer()
    {
        Serializer = new JsonSerializer();

        Serializer.Error += delegate (object _, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Log.Error(e.ErrorContext.Error, "Json serialization error: ");
        };

        Serializer.TypeNameHandling = TypeNameHandling.Auto;
        Serializer.ContractResolver = new AttributeContractResolver();
        Serializer.Converters.Add(new VersionConverter());
        Serializer.Converters.Add(new KeyValuePairConverter());
        Serializer.Converters.Add(new StringEnumConverter());
        Serializer.Converters.Add(new NitroxIdConverter());
        Serializer.Converters.Add(new ShortOptionalConverter());
    }

    public virtual string FileEnding => ".json";

    public void Serialize(Stream stream, object o)
    {
        stream.Position = 0;
        using JsonTextWriter writer = new(new StreamWriter(stream));
        Serializer.Serialize(writer, o);
    }

    public void Serialize(string filePath, object o)
    {
        string tmpPath = Path.ChangeExtension(filePath, ".tmp");
        using (StreamWriter stream = File.CreateText(tmpPath))
        {
            Serializer.Serialize(stream, o);
        }
        FileSystem.Instance.ReplaceFile(tmpPath, filePath);
    }

    public T Deserialize<T>(Stream stream)
    {
        stream.Position = 0;
        using JsonTextReader reader = new(new StreamReader(stream));
        return (T)Serializer.Deserialize(reader, typeof(T));
    }

    public T Deserialize<T>(string filePath)
    {
        using StreamReader reader = File.OpenText(filePath);
        return (T)Serializer.Deserialize(reader, typeof(T));
    }
}
