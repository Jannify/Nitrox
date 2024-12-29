using System;
using System.Reflection;
using NitroxClient.GameLogic.Settings;
using NitroxClient.MonoBehaviours.Gui.Input;
using NitroxClient.MonoBehaviours.Gui.Input.KeyBindings;
using NitroxModel.Helper;

namespace NitroxPatcher.Patches.Persistent;

public partial class GameSettings_SerializeInputSettings_Patch : NitroxPatch, IPersistentPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method(() => GameSettings.SerializeInputSettings(default(GameSettings.ISerializer)));

    public static void Postfix()
    {
        KeyBindingManager keyBindingManager = new();

        foreach (GameInput.BindingSet bindingSet in Enum.GetValues(typeof(GameInput.BindingSet)))
        {
            foreach (KeyBinding keyBinding in keyBindingManager.KeyboardKeyBindings)
            {
                Log.Debug($"Getting keybinding: {keyBinding.Device}, {keyBinding.Label} ({keyBinding.Button}), {bindingSet}");
                string binding = GameInput.GetBinding(keyBinding.Device, keyBinding.Button, bindingSet);

                // We need to assign the correct binding for primary and secondary binding sets to the relevant area of the config.
                switch ((KeyBindingValues)keyBinding.Button)
                {
                    case KeyBindingValues.CHAT when bindingSet == GameInput.BindingSet.Primary:
                        NitroxPrefs.OpenChatKeybindingPrimary.Value = binding;
                        break;
                    case KeyBindingValues.FOCUS_DISCORD when bindingSet == GameInput.BindingSet.Primary:
                        NitroxPrefs.FocusDiscordKeybindingPrimary.Value = binding;
                        break;
                    case KeyBindingValues.CHAT when bindingSet == GameInput.BindingSet.Secondary:
                        NitroxPrefs.OpenChatKeybindingSecondary.Value = binding;
                        break;
                    case KeyBindingValues.FOCUS_DISCORD when bindingSet == GameInput.BindingSet.Secondary:
                        NitroxPrefs.FocusDiscordKeybindingSecondary.Value = binding;
                        break;
                }
            }
        }
    }
}
