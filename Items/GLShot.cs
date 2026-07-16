using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;

namespace ExtendedItems.Items;

[CustomItem(ItemType.GrenadeHE)]
public class GLShot : CustomGrenade
{
    public override uint Id { get; set; } = 10;
    public override string Name { get; set; } = "GLShot";
    public override string Description { get; set; } = "You shouldn't have this";
    public override float Weight { get; set; } = 100;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 0
    };

    public override bool ExplodeOnCollision { get; set; } = false;
    public override float FuseTime { get; set; } = 1000;
}