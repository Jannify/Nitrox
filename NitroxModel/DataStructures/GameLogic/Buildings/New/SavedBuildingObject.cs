using System.Collections.Generic;
using System.Runtime.Serialization;
using NitroxModel.DataStructures.GameLogic.Buildings.New.Metadata;
using NitroxModel.DataStructures.GameLogic.Entities.Bases;
using NitroxModel.DataStructures.Unity;

namespace NitroxModel.DataStructures.GameLogic.Buildings.New;

[DataContract]
public class SavedBase
{
    [DataMember(Order = 1)]
    public NitroxInt3 BaseShape;

    [DataMember(Order = 2)]
    public byte[] Faces;

    [DataMember(Order = 3)]
    public byte[] Cells;

    [DataMember(Order = 4)]
    public byte[] Links;

    [DataMember(Order = 5)]
    public NitroxInt3 CellOffset;

    [DataMember(Order = 6)]
    public byte[] Masks;

    [DataMember(Order = 7)]
    public byte[] IsGlass;

    [DataMember(Order = 8)]
    public NitroxInt3 Anchor;

    [DataMember(Order = 9)]
    public int PrecompressionSize;
}

[DataContract]
public class SavedBuild
{
    [DataMember(Order = 1)]
    public NitroxId NitroxId;

    [DataMember(Order = 2)]
    public NitroxVector3 Position;

    [DataMember(Order = 3)]
    public NitroxQuaternion Rotation;

    [DataMember(Order = 4)]
    public NitroxVector3 LocalScale;

    [DataMember(Order = 5)]
    public SavedBase Base;

    [DataMember(Order = 6)]
    public List<SavedInteriorPiece> InteriorPieces = new();

    [DataMember(Order = 7)]
    public List<SavedModule> Modules = new();

    [DataMember(Order = 8)]
    public List<SavedGhost> Ghosts = new();

    [DataMember(Order = 9)]
    public List<SavedMoonpool> Moonpools = new();
}

[DataContract]
public class SavedGlobalRoot
{
    [DataMember(Order = 1)]
    public List<BuildEntity> Builds = new();
    
    [DataMember(Order = 2)]
    public List<ModuleEntity> Modules = new();
    
    [DataMember(Order = 3)]
    public List<GhostEntity> Ghosts = new();
}

[DataContract]
public class SavedInteriorPiece
{
    [DataMember(Order = 1)]
    public NitroxId NitroxId;

    [DataMember(Order = 2)]
    public string ClassId;

    [DataMember(Order = 3)]
    public NitroxBaseFace BaseFace;

    [DataMember(Order = 4)]
    public float Constructed;
}

[DataContract]
public class SavedModule
{
    [DataMember(Order = 1)]
    public string ClassId;

    [DataMember(Order = 2)]
    public NitroxId NitroxId;

    [DataMember(Order = 3)]
    public NitroxVector3 Position;

    [DataMember(Order = 4)]
    public NitroxQuaternion Rotation;

    [DataMember(Order = 5)]
    public NitroxVector3 LocalScale;

    [DataMember(Order = 6)]
    public float ConstructedAmount;

    [DataMember(Order = 7)]
    public bool IsInside;
}

[DataContract]
public class SavedGhost : SavedModule
{
    [DataMember(Order = 1)]
    public NitroxBaseFace BaseFace;

    [DataMember(Order = 2)]
    public SavedBase Base;

    [DataMember(Order = 3)]
    public GhostMetadata Metadata;

    [DataMember(Order = 4)]
    public NitroxTechType TechType;
}

[DataContract]
public class SavedMoonpool
{
    [DataMember(Order = 1)]
    public NitroxId NitroxId;

    [DataMember(Order = 2)]
    public NitroxId ParentId;

    [DataMember(Order = 3)]
    public NitroxInt3 Cell;
}
