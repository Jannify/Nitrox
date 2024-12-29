using System.Collections.Generic;
using System.Linq;
using NitroxClient.GameLogic.Settings;
using NitroxClient.MonoBehaviours.Gui.Input.KeyBindings;
using NitroxClient.MonoBehaviours.Gui.Input.KeyBindings.Actions;

namespace NitroxClient.MonoBehaviours.Gui.Input;

public class KeyBindingManager
{
    // New bindings should not be set to a value equivalent to a pre-existing GameInput.Button enum or another custom binding
    public List<KeyBinding> KeyboardKeyBindings { get; } =
    [
        new((int)KeyBindingValues.CHAT, "Chat", GameInput.Device.Keyboard, new ChatKeyBindingAction(), NitroxPrefs.OpenChatKeybindingPrimary.Value, NitroxPrefs.OpenChatKeybindingSecondary.Value),
        new((int)KeyBindingValues.FOCUS_DISCORD, "Focus Discord (Alt +)", GameInput.Device.Keyboard, new DiscordFocusBindingAction(), NitroxPrefs.FocusDiscordKeybindingPrimary.Value, NitroxPrefs.FocusDiscordKeybindingSecondary.Value)
    ];


    // Returns highest custom key binding value. If no custom key bindings, returns 0.
    public int GetHighestKeyBindingValue()
    {
        return KeyboardKeyBindings.Select(keyBinding => (int)keyBinding.Button).DefaultIfEmpty(0).Max();
    }
}

/// <summary>
///     Refers the keybinding values used for button creation in the options menu, to a more clear form
/// </summary>
public enum KeyBindingValues
{
    CHAT = 1145,
    FOCUS_DISCORD = 1146
}
