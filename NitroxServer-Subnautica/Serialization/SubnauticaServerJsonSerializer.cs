using NitroxServer_Subnautica.Serialization.Json;
using NitroxServer.Serialization;

namespace NitroxServer_Subnautica.Serialization
{
    public class SubnauticaServerJsonSerializer : ServerJsonSerializer
    {
        public SubnauticaServerJsonSerializer()
        {
            Serializer.Converters.Add(new SubnauticaTechTypeConverter());
        }
    }
}
