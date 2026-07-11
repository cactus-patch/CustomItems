using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;

namespace ExtendedItems;

public class EventHandler (Plugin plugin)
{
    protected void OnGrenadeExploding(ExplodingGrenadeEventArgs ev)
    {
        foreach (Player player in ev.TargetsToAffect.ToArray())
        {
            Utils.Grenade_Damage(ev.Projectile,  player);
        }
    }

    
}