using System.Reflection;
using NitroxClient.MonoBehaviours.Overrides;
using NitroxModel_Subnautica.DataStructures;
using NitroxModel_Subnautica.DataStructures.GameLogic.Buildings.Rotation.Metadata;
using NitroxModel.Helper;

namespace NitroxPatcher.Patches.Dynamic
{
    public sealed partial class Base_GetAdjacentFace_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo METHOD = Reflect.Method(() => Base.GetAdjacentFace(default(Base.Face)));

        public static bool Prefix(ref Base.Face __result, Base.Face face)
        {
            if(MultiplayerBuilder.RotationMetadata.HasValue)
            {
                switch (MultiplayerBuilder.RotationMetadata.Value)
                {
                    case AnchoredFaceBuilderMetadata anchoredFaceRotationMetadata:
                        __result = new Base.Face(anchoredFaceRotationMetadata.Cell.ToUnity(), (Base.Direction)anchoredFaceRotationMetadata.Direction);
                        return false;
                    case BaseModuleBuilderMetadata baseModuleRotationMetadata:
                        __result = new Base.Face(baseModuleRotationMetadata.Cell.ToUnity(), (Base.Direction)baseModuleRotationMetadata.Direction);
                        return false;
                }
            }

            return true;
        }
    }
}
