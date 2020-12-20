using NitroxClient.Communication.Abstract;
using NitroxClient.GameLogic;
using NitroxModel.Packets;

namespace NitroxClient.Communication.NetworkingLayer.PlatformP2P.Abstract
{
    public interface IPlatformHandler
    {
        public bool IsInitialized();

        public bool Setup();
    }
}
