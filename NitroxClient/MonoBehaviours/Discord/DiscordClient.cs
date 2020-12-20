using System.Linq;
using NitroxClient.Helpers.DiscordGameSDK;
using NitroxClient.MonoBehaviours.Gui.MainMenu;
using NitroxModel.Discovery.InstallationFinders;
using NitroxModel.Helper;
using NitroxModel.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NitroxClient.MonoBehaviours.DiscordRP
{
    public class DiscordClient : MonoBehaviour
    {
        private const long CLIENT_ID = 405122994348752896;

        public Helpers.DiscordGameSDK.Discord Discord { get; private set; }
        private ActivityManager activityManager;
        private NetworkManager networkManager;
        private Activity activity;
        private bool showingWindow;
        private bool isP2PActive;

        private static DiscordClient main;
        public static DiscordClient Main
        {
            get
            {
                if (!main)
                {
                    main = new GameObject("DiscordController").AddComponent<DiscordClient>();
                }
                return main;
            }
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            Discord = new Helpers.DiscordGameSDK.Discord(CLIENT_ID, 0);
            activityManager = Discord.GetActivityManager();
            networkManager = Discord.GetNetworkManager();
            activityManager.RegisterSteam(SteamGameRegistryFinder.SUBNAUTICA_APP_ID);
            activityManager.OnActivityJoinRequest += ActivityJoinRequest;
            activityManager.OnActivityJoin += ActivityJoin;
            Discord.SetLogHook(Helpers.DiscordGameSDK.LogLevel.Debug, (level, message) => Log.Write((NitroxModel.Logger.LogLevel)level, "[Discord] " + message));

            activity = new Activity();
        }

        private void OnDisable()
        {
            Log.Info("[Discord] Shutdown");
            main = null;
            Discord.Dispose();
        }

        private void Update()
        {
            Discord.RunCallbacks();
            if (isP2PActive)
            {
                networkManager.Flush();
            }
        }

        private void ActivityJoin(string secret)
        {
            Log.Info("[Discord] Joining Server");
            if (SceneManager.GetActiveScene().name == "StartScreen" && MainMenuMultiplayerPanel.Main != null)
            {
                string[] splitSecret = secret.Split(':');
                string ip = string.Join(":", splitSecret.Take(splitSecret.Length - 1));
                string port = splitSecret.Last();
                MainMenuMultiplayerPanel.Main.OpenJoinServerMenu(ip, port);
            }
            else
            {
                Log.InGame("Please press on the \"Multiplayer\" in the MainMenu if you want to join a session.");
                Log.Warn("[Discord] Can't join a server outside of the main-menu.");
            }
        }

        private void ActivityJoinRequest(ref User user)
        {
            if (!showingWindow)
            {
                Log.Info($"[Discord] JoinRequest: Name:{user.Username}#{user.Discriminator} UserID:{user.Id}");
                DiscordJoinRequestGui acceptRequest = gameObject.AddComponent<DiscordJoinRequestGui>();
                acceptRequest.User = user;
                showingWindow = true;
            }
            else
            {
                Log.Debug("[Discord] Request window is already active.");
            }
        }

        public void InitializeRPInGame(string username, int playerCount, int maxConnections, string ipAddressPort)
        {
            activity.State = "In game";
            activity.Details = "Playing as " + username;
            activity.Timestamps.Start = 0;
            activity.Party.Id = "PartyID:" + CheckIP(ipAddressPort);
            activity.Party.Size.CurrentSize = playerCount;
            activity.Party.Size.MaxSize = maxConnections;
            activity.Secrets.Join = CheckIP(ipAddressPort);
            UpdateActivity();
        }

        public void InitializeRPMenu()
        {
            activity.State = "In menu";
            activity.Assets.LargeImage = "icon";
            UpdateActivity();
        }

        public void UpdatePartySize(int size)
        {
            activity.Party.Size.CurrentSize = size;
            UpdateActivity();
        }

        private void UpdateActivity()
        {
            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result != Result.Ok)
                {
                    Log.Error("[Discord] Updating Activity failed");
                }
            });
        }

        public void RespondJoinRequest(long userID, ActivityJoinRequestReply reply)
        {
            Log.Info($"[Discord] Responded with {reply} to JoinRequest: {userID}");
            showingWindow = false;
            activityManager.SendRequestReply(userID, reply, _ => { });
        }

        public void EnableP2P()
        {
            isP2PActive = true;
        }

        public void DisableP2P()
        {
            isP2PActive = false;
        }

        private static string CheckIP(string ipPort)
        {
            string ip = ipPort.Split(':')[0];
            string port = ipPort.Split(':')[1];

            if (ip == "127.0.0.1")
            {
                return WebHelper.GetPublicIP() + ":" + port;
            }
            return ipPort;
        }
    }
}
