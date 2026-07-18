using Exiled.Events.EventArgs.Map;
using LabApi.Events.Arguments.PlayerEvents;

namespace ExtendedItems;

public class EventHandler()
{
    private Random random = new();
    private bool HasProperFolder =  Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,".."));
    public static void OnGrenadeExploding(ExplodingGrenadeEventArgs ev)
    {
        foreach (var player in ev.TargetsToAffect.ToArray()) Utils.Grenade_Damage(ev.Projectile, player);
    }

    public void OnLockerOpen(PlayerInteractedLockerEventArgs ev)
    {
        if (!Plugin.Instance.Config.EasterEggs)
        {
            return;
        }
        ev.Locker.Base._deniedBeep = 
    }
}