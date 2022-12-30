using NitroxModel.Packets;
using NitroxServer.Communication.Packets.Processors.Abstract;
using NitroxServer.GameLogic;
using NitroxServer.GameLogic.Bases;

namespace NitroxServer.Communication.Packets.Processors;

// TODO: Finish up this whole class
public abstract class BuildingProcessor<T> : AuthenticatedPacketProcessor<T> where T : Packet
{
    internal readonly BuildingManager buildingManager;
    internal readonly PlayerManager playerManager;

    public BuildingProcessor(BuildingManager buildingManager, PlayerManager playerManager)
    {
        this.buildingManager = buildingManager;
        this.playerManager = playerManager;
    }

    public override void Process(T packet, Player player)
    {
        playerManager.SendPacketToOtherPlayers(packet, player);
    }
}

public class PlaceGhostProcessor : BuildingProcessor<PlaceGhost>
{
    public PlaceGhostProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(PlaceGhost packet, Player player)
    {
        if (buildingManager.AddGhost(packet))
        {
            base.Process(packet, player);
        }
    }
}

public class PlaceModuleProcessor : BuildingProcessor<PlaceModule>
{
    public PlaceModuleProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(PlaceModule packet, Player player)
    {
        if (buildingManager.AddModule(packet))
        {
            base.Process(packet, player);
        }
    }
}

public class ModifyConstructedAmountProcessor : BuildingProcessor<ModifyConstructedAmount>
{
    public ModifyConstructedAmountProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(ModifyConstructedAmount packet, Player player)
    {
        base.Process(packet, player);
    }
}

public class PlaceBaseProcessor : BuildingProcessor<PlaceBase>
{
    public PlaceBaseProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(PlaceBase packet, Player player)
    {
        base.Process(packet, player);
    }
}

public class UpdateBaseProcessor : BuildingProcessor<UpdateBase>
{
    public UpdateBaseProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(UpdateBase packet, Player player)
    {
        base.Process(packet, player);
    }
}

public class BaseDeconstructedProcessor : BuildingProcessor<BaseDeconstructed>
{
    public BaseDeconstructedProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(BaseDeconstructed packet, Player player)
    {
        base.Process(packet, player);
    }
}

public class PieceDeconstructedProcessor : BuildingProcessor<PieceDeconstructed>
{
    public PieceDeconstructedProcessor(BuildingManager buildingManager, PlayerManager playerManager) : base(buildingManager, playerManager) { }

    public override void Process(PieceDeconstructed packet, Player player)
    {
        base.Process(packet, player);
    }
}
