using System.Reflection;
using NitroxClient.MonoBehaviours.Overrides;
using NitroxModel_Subnautica.DataStructures;
using NitroxModel_Subnautica.DataStructures.GameLogic.Buildings.Rotation.Metadata;
using NitroxModel.Helper;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic
{
    public partial class Base_PickFace_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo METHOD = Reflect.Method((Base t) => t.PickFace(default(Transform), out Reflect.Ref<Base.Face>.Field));

        public static bool Prefix(Base __instance, ref bool __result, ref Base.Face face)
        {
            if(MultiplayerBuilder.RotationMetadata.HasValue)
            {
                switch (MultiplayerBuilder.RotationMetadata.Value)
                {
                    case AnchoredFaceBuilderMetadata anchoredFaceRotationMetadata:
                        face = new Base.Face(anchoredFaceRotationMetadata.Cell.ToUnity(), (Base.Direction)anchoredFaceRotationMetadata.Direction);
                        __result = true;
                        return false;
                    case BaseModuleBuilderMetadata baseModuleRotationMetadata:
                        face = new Base.Face(baseModuleRotationMetadata.Cell.ToUnity(), (Base.Direction)baseModuleRotationMetadata.Direction);
                        __result = true;
                        return false;
                }
            }
            return true;
        }
    }
}
