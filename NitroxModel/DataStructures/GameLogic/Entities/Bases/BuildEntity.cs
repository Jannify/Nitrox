using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BinaryPack.Attributes;
using NitroxModel.DataStructures.GameLogic.Buildings.New;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using NitroxModel.DataStructures.Unity;

namespace NitroxModel.DataStructures.GameLogic.Entities.Bases;

[Serializable, DataContract]
public class BuildEntity : GlobalRootEntity
{
    // TODO: Move SavedBuild's fields in here
    [DataMember(Order = 1)]
    public SavedBuild SavedBuild { get; set; }

    [IgnoreConstructor]
    protected BuildEntity()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public BuildEntity(SavedBuild savedBuild)
    {
        SavedBuild = savedBuild;
        Id = savedBuild.NitroxId;
        Transform = new();
    }

    /// <remarks>Used for deserialization</remarks>
    public BuildEntity(SavedBuild savedBuild, NitroxTransform transform, int level, string classId, bool spawnedByServer, NitroxId id, NitroxTechType techType, EntityMetadata metadata, NitroxId parentId, List<Entity> childEntities)
    {
        SavedBuild = savedBuild;

        Id = id;
        TechType = techType;
        Metadata = metadata;
        ParentId = parentId;
        Transform = transform;
        ChildEntities = childEntities;
        Level = level;
        ClassId = classId;
        SpawnedByServer = spawnedByServer;
    }

    public override string ToString()
    {
        return $"[BuildEntity {SavedBuild}]";
    }
}
