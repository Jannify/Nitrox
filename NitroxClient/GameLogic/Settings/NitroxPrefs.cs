using System;
using UnityEngine;

namespace NitroxClient.GameLogic.Settings;

public static class NitroxPrefs
{
    // Add new fields here, you can use bool, float and int as type
    public static readonly NitroxPref<bool> HideIp = new("Nitrox.hideIp");
    public static readonly NitroxPref<bool> SilenceChat = new("Nitrox.silenceChat");
    public static readonly NitroxPref<bool> ChatUsed = new("Nitrox.chatUsed");
    public static readonly NitroxPref<bool> SafeBuilding = new("Nitrox.safeBuilding", true);
    public static readonly NitroxPref<bool> SafeBuildingLog = new("Nitrox.safeBuildingLog", true);

    /// <summary>
    /// In seconds. <see cref="MonoBehaviours.Vehicles.VehicleMovementReplicator"/>
    /// </summary>
    public static readonly NitroxPref<float> LatencyUpdatePeriod = new("Nitrox.latencyUpdatePeriod", 10);

    /// <summary>
    /// In milliseconds. <see cref="MonoBehaviours.Vehicles.VehicleMovementReplicator"/>
    /// </summary>
    public static readonly NitroxPref<float> SafetyLatencyMargin = new("Nitrox.safetyLatencyMargin", 0.05f);

    public static readonly NitroxPref<string> OpenChatKeybindingPrimary = new("Nitrox.keybinding.chat.primary", "Y");
    public static readonly NitroxPref<string> OpenChatKeybindingSecondary = new("Nitrox.keybinding.chat.secondary", "");
    public static readonly NitroxPref<string> FocusDiscordKeybindingPrimary = new("Nitrox.keybinding.discord.primary", "F");
    public static readonly NitroxPref<string> FocusDiscordKeybindingSecondary = new("Nitrox.keybinding.discord.secondary", "");
}

public abstract class NitroxPref;

public class NitroxPref<T> : NitroxPref where T : IConvertible
{
    public string Key { get; }
    public T DefaultValue { get; }

    public NitroxPref(string key, T defaultValue = default)
    {
        Key = key;
        DefaultValue = defaultValue;
    }

    public T Value
    {
        get
        {
            return DefaultValue switch
            {
                bool defaultBool => (T)Convert.ChangeType(PlayerPrefs.GetInt(Key, defaultBool ? 1 : 0), typeof(T)),
                float defaultFloat => (T)Convert.ChangeType(PlayerPrefs.GetFloat(Key, defaultFloat), typeof(T)),
                int defaultInteger => (T)Convert.ChangeType(PlayerPrefs.GetInt(Key, defaultInteger), typeof(T)),
                string defaultStr => (T)Convert.ChangeType(PlayerPrefs.GetString(Key, defaultStr), typeof(T)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        set
        {
            switch (value)
            {
                case bool boolValue:
                    PlayerPrefs.SetInt(Key, boolValue ? 1 : 0);
                    break;
                case float floatValue:
                    PlayerPrefs.SetFloat(Key, floatValue);
                    break;
                case int intValue:
                    PlayerPrefs.SetInt(Key, intValue);
                    break;
                case string stringValue:
                    PlayerPrefs.SetString(Key, stringValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
            PlayerPrefs.Save();
        }
    }
}
