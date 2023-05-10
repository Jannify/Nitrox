using System;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BinaryPack.Attributes;

namespace NitroxModel.DataStructures.GameLogic.Entities;

[Serializable]
[DataContract]
public class PlanterEntity : Entity
{
    [IgnoreConstructor]
    protected PlanterEntity()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public PlanterEntity(NitroxId id, NitroxId parentId)
    {
        Id = id;
        ParentId = parentId;
    }

    /// <remarks>Used for deserialization</remarks>
    public PlanterEntity(NitroxId id, NitroxTechType techType, EntityMetadata metadata, NitroxId parentId, List<Entity> childEntities)
    {
        Id = id;
        TechType = techType;
        Metadata = metadata;
        ParentId = parentId;
        ChildEntities = childEntities;
    }

    public override string ToString()
    {
        return $"[PlanterEntity {base.ToString()}]";
    }
}
