using NitroxClient.Communication.Abstract;

namespace NitroxClient.Communication.NetworkingLayer.PlatformP2P.Discord
{
    public class DiscordP2P : IConnectionInfo
    {
        public readonly long LobbyID;

        public DiscordP2P(long lobbyID)
        {
            LobbyID = lobbyID;
        }

        public override bool Equals(object obj)
        {
            return obj is DiscordP2P discordP2P &&
                   LobbyID.Equals(discordP2P.LobbyID);
        }

        public bool Equals(IConnectionInfo other)
        {
            return other is DiscordP2P discordP2P && LobbyID.Equals(discordP2P.LobbyID);
        }

        public override int GetHashCode()
        {
            return 898718151 + LobbyID.GetHashCode();
        }

        public static bool operator ==(DiscordP2P left, DiscordP2P right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiscordP2P left, DiscordP2P right)
        {
            return !(left == right);
        }
    }
}
