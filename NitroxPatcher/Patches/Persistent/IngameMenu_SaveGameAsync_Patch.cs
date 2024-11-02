using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using NitroxModel.Helper;

namespace NitroxPatcher.Patches.Persistent;

/// <summary>
/// Skips saving the multiplayer-world to the disk while keeping garbage collection and cleanup routines
/// </summary>
public sealed partial class IngameMenu_SaveGameAsync_Patch : NitroxPatch, IPersistentPatch
{
    private static readonly MethodInfo enumeratorMethod = Reflect.Method((IngameMenu t) => t.SaveGameAsync());
    private static readonly MethodInfo targetMethod = AccessTools.EnumeratorMoveNext(enumeratorMethod);

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
               .MatchStartForward(
                   new CodeMatch(OpCodes.Ldloc_1),
                   new CodeMatch(OpCodes.Call, Reflect.Method((IngameMenu x) => x.CaptureSaveScreenshot()))
               )
               .RemoveInstructions(2)
               .MatchEndForward(
                   new CodeMatch(OpCodes.Ldloc_1),
                   new CodeMatch(OpCodes.Call, Reflect.Method((IngameMenu x) => x.Close()))
               )
               .Advance(1)
               .Insert(
                   new CodeInstruction(OpCodes.Ldc_I4_0),
                   new CodeInstruction(OpCodes.Ret)
               )
               .InstructionEnumeration();
    }
}
