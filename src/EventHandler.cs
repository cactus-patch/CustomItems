using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using Map = Exiled.Events.Handlers.Map;
using Player = Exiled.Events.Handlers.Player;

namespace ExtendedItems;

public class EventHandler
{
    public Plugin? Plugin;

    public void OnGrenadeExploding(ExplodingGrenadeEventArgs ev)
    {
        foreach (var player in ev.TargetsToAffect.ToArray()) Utils.Grenade_Damage(ev.Projectile, player);
    }

    public void OnFillingLockers(FillingLockerEventArgs ev)
    {
        if (ev.Pickup.Type == ItemType.GunFRMG0)
        {
            ev.IsAllowed = false;
        }
    }

    public void OnPlayerHealing(HealedEventArgs ev)
    {
        if (ev.HealedAmount > 50 && Utils.PlayerShot[ev.Player.Id])
            Timing.CallDelayed(5f, () => Utils.PlayerShot[ev.Player.Id] = false);
    }

    protected internal EventHandler(Plugin plugin)
    {
        Plugin = plugin;

        Log.Info("ExtendedItems EventHandler loaded");
        Map.ExplodingGrenade += OnGrenadeExploding;
        Map.FillingLocker += OnFillingLockers;

        Player.Healed += OnPlayerHealing;
    }

    ~EventHandler()
    {
        Log.Info("ExtendedItems EventHandler unloaded");

        Map.ExplodingGrenade -= OnGrenadeExploding;
        Map.FillingLocker -= OnFillingLockers;

        Player.Healed -= OnPlayerHealing;

        Plugin = null;
    }
}