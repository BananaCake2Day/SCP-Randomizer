using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.Coroutines;

public static class Utility
{
    static internal MainPlugin plugin;

    static public readonly Role[] scpArray = { Role.SCP_049, Role.SCP_049_2, Role.SCP_096, Role.SCP_106, Role.SCP_173, Role.SCP_939_53 };
    static public readonly Role[] safeRoles = { Role.CHAOS_INSURGENCY, Role.CLASSD, Role.FACILITY_GUARD, Role.NTF_CADET, Role.SCIENTIST, Role.NTF_LIEUTENANT };

    static public Player RandomPlayer {
        get {
            return GetRandomPlayer();
        }
    }

    static internal Player GetRandomPlayer()
    {
        Player[] plrs = plugin.Server.GetPlayers()
            .Where(pl => pl.TeamRole.Role != Role.SPECTATOR)
            .ToArray();
        if (plrs.Length <= 0) throw new Exception("No players in server. Stopping events");
        int chosen = plugin.pr.Next(plrs.Length);
        return plrs[chosen];
    }

    static internal Player GetRandomPlayer(Role role)
    {
        Player[] plrs = plugin.Server.GetPlayers()
            .Where(pl => pl.TeamRole.Role == role)
            .ToArray();
        if (plrs.Length <= 0) return null;
        int chosen = plugin.pr.Next(plrs.Length);
        return plrs[chosen];
    }

    static internal Player GetRandomPlayer(Smod2.API.Team role)
    {
        Player[] plrs = plugin.Server.GetPlayers()
            .Where(pl => pl.TeamRole.Team == role)
            .ToArray();
        if (plrs.Length <= 0) return null;
        int chosen = plugin.pr.Next(plrs.Length);
        return plrs[chosen];
    }

    static internal void ChangePlayerFromTo(Role from, Role to)
    {
        Player plr = GetRandomPlayer(from);
        if (plr == null) return;
        plr.ChangeRole(to, false, false, false, true);
    }

    static internal void ChangePlayerFromTo(Smod2.API.Team from, Role to)
    {
        Player plr = GetRandomPlayer(from);
        if (plr == null) return;
        plr.ChangeRole(to, false, false, false, true);
    }

    static internal void LockDoors()
    {
        foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
        {
            door.Locked = true;
        }
    }

    static internal void LockAndCloseDoors()
    {
        foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
        {
            door.Open = false;
            door.Locked = true;
        }
    }

    static internal void UnlockDoorsAfterSeconds(float time) =>
        plugin.StartCoroutine(UnlockDoors(time));

    static private IEnumerable<float> UnlockDoors(float time)
    {
        yield return Coroutine.WaitForSeconds(time);
        foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
        {
            door.Locked = false;
        }
    }

    static public T[] Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = plugin.pr.Next(n);
            n--;
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return array;
    }

    static internal void FlickerLights()
    {
        Room[] rooms = plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(room => room.ZoneType != ZoneType.ENTRANCE).ToArray();
        foreach (Room room in rooms)
        {
            room.FlickerLights();
        }
    }
}
