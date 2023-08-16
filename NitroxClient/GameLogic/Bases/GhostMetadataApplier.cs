using System.Collections;
using NitroxClient.Unity.Helper;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata.Bases;
using NitroxModel_Subnautica.DataStructures;
using UnityEngine;
using UWE;

namespace NitroxClient.GameLogic.Bases;

public static class GhostMetadataApplier
{
    public static void ApplyBasicMetadataTo(this GhostMetadata ghostMetadata, BaseGhost baseGhost)
    {
        baseGhost.targetOffset = ghostMetadata.TargetOffset.ToUnity();
    }

    public static IEnumerator ApplyMetadataToGhost(BaseGhost baseGhost, EntityMetadata entityMetadata, Base @base)
    {
        if (entityMetadata is not GhostMetadata ghostMetadata)
        {
            Log.Error($"Trying to apply metadata to a ghost that is not of type {nameof(GhostMetadata)} : [{entityMetadata.GetType()}]");
            yield break;
        }

        if (BuildUtils.IsUnderBaseDeconstructable(baseGhost, true))
        {
            if (entityMetadata is BaseDeconstructableGhostMetadata deconstructableMetadata)
            {
                yield return deconstructableMetadata.ApplyBaseDeconstructableMetadataTo(baseGhost, @base);
            }
            else
            {
                Log.Error($"[{nameof(GhostMetadataApplier)}] Metadata of type {entityMetadata.GetType()} can't be applied to BaseDeconstructable's ghost");
            }
            yield break;
        }

        switch (baseGhost)
        {
            case BaseAddWaterPark:
            case BaseAddPartitionDoorGhost:
            case BaseAddModuleGhost:
            case BaseAddFaceGhost:
                if (ghostMetadata is BaseAnchoredFaceGhostMetadata faceMetadata)
                {
                    faceMetadata.ApplyBaseAnchoredFaceMetadataTo(baseGhost);
                    yield break;
                }
                break;

            case BaseAddPartitionGhost:
                if (ghostMetadata is BaseAnchoredCellGhostMetadata cellMetadata)
                {
                    cellMetadata.ApplyBaseAnchoredCellMetadataTo(baseGhost);
                    yield break;
                }
                break;

            default:
                ghostMetadata.ApplyBasicMetadataTo(baseGhost);
                yield break;
        }
        Log.Error($"[{nameof(GhostMetadataApplier)}] Metadata of type {entityMetadata.GetType()} can't be applied to ghost of type {baseGhost.GetType()}");
    }

    public static void ApplyBaseAnchoredCellMetadataTo(this BaseAnchoredCellGhostMetadata ghostMetadata, BaseGhost baseGhost)
    {
        ApplyBasicMetadataTo(ghostMetadata, baseGhost);
        if (ghostMetadata.AnchoredCell.HasValue)
        {
            if (baseGhost is BaseAddPartitionGhost ghost)
            {
                ghost.anchoredCell = ghostMetadata.AnchoredCell.Value.ToUnity();
            }
        }
    }

    public static void ApplyBaseAnchoredFaceMetadataTo(this BaseAnchoredFaceGhostMetadata ghostMetadata, BaseGhost baseGhost)
    {
        ApplyBasicMetadataTo(ghostMetadata, baseGhost);
        if (ghostMetadata.AnchoredFace.HasValue)
        {
            switch (baseGhost)
            {
                case BaseAddWaterPark ghost:
                    ghost.anchoredFace = ghostMetadata.AnchoredFace.Value.ToUnity();
                    break;
                case BaseAddPartitionDoorGhost ghost:
                    ghost.anchoredFace = ghostMetadata.AnchoredFace.Value.ToUnity();
                    break;
                case BaseAddModuleGhost ghost:
                    ghost.anchoredFace = ghostMetadata.AnchoredFace.Value.ToUnity();
                    break;
                case BaseAddFaceGhost ghost:
                    ghost.anchoredFace = ghostMetadata.AnchoredFace.Value.ToUnity();
                    break;
            }
        }
    }

    public static IEnumerator ApplyBaseDeconstructableMetadataTo(this BaseDeconstructableGhostMetadata ghostMetadata, BaseGhost baseGhost, Base @base)
    {
        ghostMetadata.ApplyBasicMetadataTo(baseGhost);
        baseGhost.DisableGhostModelScripts();
        if (ghostMetadata.ModuleFace.HasValue)
        {
            if (!baseGhost.TryGetComponentInParent(out ConstructableBase constructableBase))
            {
                Log.Error($"Couldn't find an interior piece's parent ConstructableBase to apply a {nameof(BaseDeconstructableGhostMetadata)} to.");
                yield break;
            }

            constructableBase.moduleFace = ghostMetadata.ModuleFace.Value.ToUnity();

            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(ghostMetadata.ClassId);
            yield return request;
            if (!request.TryGetPrefab(out GameObject prefab))
            {
                // Without its module, the ghost will be useless, so we delete it (like in base game)
                Object.Destroy(constructableBase.gameObject);
                Log.Error($"Couldn't find a prefab for module of interior piece of ClassId: {ghostMetadata.ClassId}");
                yield break;
            }

            if (!baseGhost.targetBase)
            {
                baseGhost.targetBase = @base;
            }

            Base.Face face = ghostMetadata.ModuleFace.Value.ToUnity();
            face.cell += baseGhost.targetBase.GetAnchor();
            GameObject moduleObject = baseGhost.targetBase.SpawnModule(prefab, face);
            if (!moduleObject)
            {
                Object.Destroy(constructableBase.gameObject);
                Log.Error("Module couldn't be spawned for interior piece");
                yield break;
            }

            if (moduleObject.TryGetComponent(out IBaseModule baseModule))
            {
                baseModule.constructed = constructableBase.constructedAmount;
            }
        }
    }
}
