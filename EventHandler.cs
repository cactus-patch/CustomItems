using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;

namespace ExtendedItems;

public class EventHandler
{
    public static void OnGrenadeExploding(ExplodingGrenadeEventArgs ev)
    {
        foreach (var player in ev.TargetsToAffect.ToArray()) Utils.Grenade_Damage(ev.Projectile, player);
    }

    public void OnFillingLockers(FillingLockerEventArgs ev)
    {
        if (ev.Pickup.Type == ItemType.GrenadeHE)
            ev.Chamber.RequiredPermissions = KeycardPermissions.ArmoryLevelThree |
                                             KeycardPermissions.ContainmentLevelTwo | KeycardPermissions.ExitGates;
    }
}