using Exiled.Events.EventArgs.Map;
using LabApi.Events.Arguments.PlayerEvents;

namespace ExtendedItems;

public class EventHandler()
{
    public static void OnGrenadeExploding(ExplodingGrenadeEventArgs ev)
    {
        foreach (var player in ev.TargetsToAffect.ToArray()) Utils.Grenade_Damage(ev.Projectile, player);
    }

    public void OnLockerOpen(PlayerInteractedLockerEventArgs ev)
    {
        ev.Locker.Base._deniedBeep = 
    }
}