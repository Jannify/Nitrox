using System;
using NitroxClient.Communication.Abstract;
using NitroxClient.Communication.NetworkingLayer.PlatformP2P.Abstract;
using NitroxClient.Helpers.DiscordGameSDK;
using NitroxClient.MonoBehaviours.DiscordRP;
using NitroxModel.Core;
using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using NitroxModel.Packets;

namespace NitroxClient.Communication.NetworkingLayer.PlatformP2P.Discord
{
    public class DiscordP2PClient : IPlatformHandler, IClient
    {
        public bool IsConnected { get; private set; }
        private IClient host;

        private NetworkManager networkManager;
        private LobbyManager lobbyManager;
        private UserManager userManager;

        private User currentUser;
        private ulong otherUserPeerId;
        private long lobbyId;

        public bool IsInitialized()
        {
            return networkManager != null;
        }

        public bool Setup()
        {
            Helpers.DiscordGameSDK.Discord discord = DiscordClient.Main.Discord;
            networkManager = discord.GetNetworkManager();
            lobbyManager = discord.GetLobbyManager();
            userManager = discord.GetUserManager();
            currentUser = userManager.GetCurrentUser();
            return true;
        }

        public void Start(IConnectionInfo connectionInfo)
        {
            DiscordP2P discordP2P = connectionInfo as DiscordP2P;
            if (discordP2P == null)
            {
                throw new NotSupportedException("Tried passing incorrect connectionInfo to clientConnection");
            }

            DiscordClient.Main.EnableP2P();
            lobbyManager.OnMemberUpdate += OnMemberUpdateHandler;
            networkManager.OnRouteUpdate += OnRouteUpdateHandler;
            networkManager.OnMessage += ReceivedPacket;
            networkManager.OpenChannel(otherUserPeerId, 0, false);
            networkManager.OpenChannel(otherUserPeerId, 1, true);

            IMultiplayerSession multiplayerSession = NitroxServiceLocator.LocateService<IMultiplayerSession>();
            Optional<string> password = multiplayerSession.AuthenticationContext.ServerPassword;
            lobbyManager.ConnectLobby(discordP2P.LobbyID, password.HasValue ? password.Value : "123", ConnectLobbyHandler);
        }

        public void Stop()
        {
            DiscordClient.Main.DisableP2P();

            lobbyManager.OnMemberUpdate -= OnMemberUpdateHandler;
            networkManager.OnRouteUpdate -= OnRouteUpdateHandler;
            networkManager.OnMessage -= ReceivedPacket;
            networkManager.CloseChannel(otherUserPeerId, 0);
            networkManager.CloseChannel(otherUserPeerId, 1);

            lobbyManager.DisconnectLobby(lobbyId, result =>
            {
                if (result == Result.Ok)
                {
                    Log.Debug("[Discord] Left lobby!");
                }
                else
                {
                    Log.Error($"[Discord] Error while leaving the lobby with Id:{lobbyId}.");
                }
            });
        }


        public void Send(Packet packet)
        {
            networkManager.SendMessage(otherUserPeerId, (byte)packet.DeliveryMethod, packet.Serialize());
            networkManager.Flush();
        }

        public void ReceivedPacket(ulong peerId, byte channel, byte[] data)
        {
            Packet packet = Packet.Deserialize(data);
        }

        private void OnRouteUpdateHandler(string route)
        {
            LobbyMemberTransaction txn = lobbyManager.GetMemberUpdateTransaction(lobbyId, currentUser.Id);
            txn.SetMetadata("route", route);
            lobbyManager.UpdateMember(lobbyId, currentUser.Id, txn, (result =>
            {
                if (result != Result.Ok)
                {
                    Log.Error("[Discord] Updating Network Route failed");
                }
            }));
        }

        private void ConnectLobbyHandler(Result x, ref Lobby lobby)
        {
            lobbyId = lobby.Id;

            // Add our own peer id to our lobby member metadata
            // So other users can get it to connect to us
            string localPeerId = Convert.ToString(networkManager.GetPeerId());
            LobbyMemberTransaction txn = lobbyManager.GetMemberUpdateTransaction(lobby.Id, currentUser.Id);
            txn.SetMetadata("peer_id", localPeerId);
            lobbyManager.UpdateMember(lobby.Id, currentUser.Id, txn, (result) =>
            {
                if (result != Result.Ok)
                {
                    Log.Error("[Discord] ConnectLobbyHandler failed");
                }
            });

            // Get the first member in the lobby, assuming someone is already there
            long memberId = lobbyManager.GetMemberUserId(lobby.Id, 0);

            // Get their peer id and route from their metadata, added previously
            string rawPeerId = lobbyManager.GetMemberMetadataValue(lobbyId, currentUser.Id, "peer_id");
            // Metadata is stored as a string, so we need to make it an integer for OpenChannel
            otherUserPeerId = Convert.ToUInt64(rawPeerId);
            string otherRoute = lobbyManager.GetMemberMetadataValue(lobby.Id, memberId, "route");

            // Connect to them
            networkManager.OpenPeer(otherUserPeerId, otherRoute);
        }

        private void OnMemberUpdateHandler(long lobbyId, long userId)
        {
            string rawPeerId = lobbyManager.GetMemberMetadataValue(lobbyId, userId, "peer_id");
            // Metadata is stored as a string, so we need to make it an integer for OpenChannel
            ulong peerId = Convert.ToUInt64(rawPeerId);
            string newRoute = lobbyManager.GetMemberMetadataValue(lobbyId, userId, "route");
            networkManager.UpdatePeer(peerId, newRoute);
        }
    }
}
