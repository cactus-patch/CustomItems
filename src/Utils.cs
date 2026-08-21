using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Lockers;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using MEC;
using UnityEngine;
using EP = Exiled.API.Features.Player;
using Item = Exiled.API.Features.Items.Item;
using LightSourceToy = LabApi.Features.Wrappers.LightSourceToy;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Random = System.Random;
using Room = Exiled.API.Features.Room;

namespace ExtendedItems;

public static class Utils
{
    private static Dictionary<LightSourceToy, ushort> Lights = [];
    /// <summary>
    ///     Calculates the global coords of a point inside a room based on the room type and the location
    /// </summary>
    /// <param name="roomType"></param>
    /// <param name="localPos"></param>
    /// <returns>Vector3</returns>
    public static Vector3 GetGlobalCords(RoomType roomType, Vector3 localPos)
    {
        var room = Room.Get(roomType);
        var roomPos = room.Position;
        var quarterTurns = Mathf.RoundToInt(room.Rotation.eulerAngles.y / 90f) % 4;

        return quarterTurns switch
        {
            0 => new Vector3(roomPos.x + localPos.x, roomPos.y + localPos.y, roomPos.z + localPos.z),
            1 => new Vector3(roomPos.x + localPos.z, roomPos.y + localPos.y, roomPos.z - localPos.x),
            2 => new Vector3(roomPos.x - localPos.x, roomPos.y + localPos.y, roomPos.z - localPos.z),
            3 => new Vector3(roomPos.x - localPos.z, roomPos.y + localPos.y, roomPos.z + localPos.x),
            _ => Vector3.zero
        };
    }

    public static void Exploding(EP player)
    {
        var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
        grenade.FuseTime = 0.1f;
        grenade.SpawnActive(player.Position + new Vector3(0, 1, 0), player);
    }
    
    public static void Grenade_Damage(EffectGrenadeProjectile grenade, EP player)
    {
        if (Vector3.Distance(grenade.Position, player.Position) <= 4)
        {
            if (Physics.Raycast(grenade.Position, player.Position, out _, 4, (int)LayerMasks.Grenade))
            {
                if (player is { IsScp: true, HumeShield: > 900 })
                    player.Hurt(player, 900, armorPenetration: 50);
                else if (player.IsHuman) player.Hurt(grenade.PreviousOwner, 150, armorPenetration: 50);
            }
        }
        else if (Physics.Raycast(grenade.Position, player.Position, out _, 20, (int)LayerMasks.Grenade))
        {
            player.Hurt(player, 25, armorPenetration: 50);
        }
    }

    public static void Explode(Pickup? grenade, EP player)
    {
        if (grenade is null)
        {
            player.ShowHint("Shits Fucked up :(\nmb gng");
        }
        else
        {
            var lVoloc = grenade.Rigidbody.linearVelocity;
            var pos = grenade.Position;
            grenade.Destroy();
            var explosiveGrenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
            explosiveGrenade.FuseTime = Vector3.Distance(pos, player.Position) > 7f ? 1f : 5f;

            explosiveGrenade.Projectile.Rigidbody.linearVelocity = lVoloc;
            explosiveGrenade.SpawnActive(pos, player);
        }
    }

    // For when the Coin hasn't been picked up for a set amount of time
    public static void CoinHintHandler(Pickup pickup, out CoroutineHandle handle)
    {
        var glowCoroutine = Timing.RunCoroutine(CoinCoroutine(pickup));
        handle = glowCoroutine;
    }

    internal static void NormalizeLockerSpawns(Dictionary<Vector3, float> spawns, out Vector3 toSpawn)
    {
        var totalWeight = spawns.Values.Sum();
        var random = new Random();

        var roll = random.Next(0, (int)totalWeight);
        Vector3? selectedLockerType = Locker.Random(lockerType: LockerType.Scp500Pedestal)?.Position;

        foreach (KeyValuePair<Vector3, float> spawn in spawns)
        {
            if (roll < spawn.Value)
            {
                selectedLockerType = spawn.Key;
                break;
            }

            roll = (int)Mathf.Round(roll - spawn.Value);
        }

        Locker[] matchingLockers = Locker.List
            .Where(locker => locker.Position == selectedLockerType)
            .ToArray();

        if (matchingLockers.Length == 0)
        {
            toSpawn = Vector3.zero;
            return;
        }

        toSpawn = matchingLockers[random.Next(0, matchingLockers.Length)].Position;
    }

    internal static Color ParseConfigColor()
    {
        if (Plugin.Instance is null) return Color.white;
        var color = Plugin.Instance.Config.SCP1289Color;
        var fallback = Color.white;
        if (color[0] == '#')
            try
            {
                var temp = Enumerable.Range(1, color.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(color.Substring(x, 2), 16))
                    .ToArray();
                fallback = new Color(temp[0], temp[1], temp[2]);
            }
            catch (Exception)
            {
                return fallback;
            }
        else
            switch (color.ToLower())
            {
                case "red":
                    return Color.red;
                case "yellow":
                    return Color.yellow;
                case "green":
                    return Color.green;
                case "blue":
                    return Color.blue;
                case "cyan":
                    return Color.cyan;
                case "magenta":
                    return Color.magenta;
                case "black":
                    return Color.black;
                case "gray":
                    return Color.gray;
            }

        return fallback;
    }

    internal static Dictionary<Vector3, float> GetSpawnLocations(SpawnProperties? spawnProperties)
    {
        if (spawnProperties is null)
        {
            Log.Warn("SpawnProperties is null.");
            return new Dictionary<Vector3, float>();
        }

        Dictionary<Vector3, float> spawnLocations = spawnProperties.LockerSpawnPoints
            .TakeWhile(lockerSpawnPoints => lockerSpawnPoints.Chance != 0f).ToDictionary(
                lockerSpawnPoints => lockerSpawnPoints.Position, lockerSpawnPoints => lockerSpawnPoints.Chance);
        foreach (var dynamicSpawnLocation in spawnProperties.DynamicSpawnPoints)
            spawnLocations.Add(dynamicSpawnLocation.Position + new Vector3(0, .5f, 0), dynamicSpawnLocation.Chance);

        foreach (var roomSpawnLocation in spawnProperties.RoomSpawnPoints)
            spawnLocations.Add(Room.Get(roomSpawnLocation.Room).Position + new Vector3(0, .5f, 0),
                roomSpawnLocation.Chance);
        foreach (var staticSpawnLocation in spawnProperties.StaticSpawnPoints)
            spawnLocations.Add(staticSpawnLocation.Position + new Vector3(0, .5f, 0), staticSpawnLocation.Chance);

        return spawnLocations;
    }

    private static IEnumerator<float> CoinCoroutine(Pickup pickup)
    {
        if (Plugin.Instance != null)
        {
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.SCP1289Timer);
            var intensity = 0;
            var light = LightSourceToy.Create(new Vector3(0, 0, 0), pickup.Rotation, pickup.Transform, false);
            Lights.Add(light, pickup.Serial);

            light.Color = ParseConfigColor();
            light.Range = Plugin.Instance.Config.SCP1289LightRange;
            light.Type = LightType.Tube;
            light.ShadowStrength = .5f;
            light.Intensity = intensity;
            light.Parent = pickup.Transform;

            light.Spawn();

            while (intensity != 100)
            {
                light.Intensity = intensity;
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.TimeToFullGlow / 100);
                intensity++;
            }
        }
    }

    internal static void RemoveLight(Pickup pickup)
    {
        if (Lights.ContainsKey(Lights.FirstOrDefault(light => light.Value == pickup.Serial).Key))
        {
            Lights.Remove(Lights.FirstOrDefault(light => light.Value == pickup.Serial).Key);
            Lights.FirstOrDefault(light => light.Value == pickup.Serial).Key.Destroy();
        }
    }
}
