using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryEvents
{
    [RandomEvent(EventType.InventoryEvent)]
    public static void SetAllCups()
    {
        Player rplr = Utility.RandomPlayer;
        foreach (Smod2.API.Item item in rplr.GetInventory())
        {
            item.Remove();
        }
        for (int i = 0; i < 8; i++)
        {
            rplr.GiveItem(ItemType.CUP);
        }
    }

    [RandomEvent(EventType.InventoryEvent)]
    public static void SetCardsToJanitor()
    {
        Player rplr = Utility.RandomPlayer;
        foreach (Smod2.API.Item item in rplr.GetInventory())
        {
            int it = (int)item.ItemType;
            if (0 <= it && it <= 11)
            {
                item.Remove();
                rplr.GiveItem(ItemType.JANITOR_KEYCARD);
            }
        }
    }

    [RandomEvent(EventType.InventoryEvent)]
    public static void GiveO5Card()
        => Utility.RandomPlayer.GiveItem(ItemType.O5_LEVEL_KEYCARD);

    [RandomEvent(EventType.InventoryEvent)]
    public static void GiveFlashbang()
        => Utility.RandomPlayer.GiveItem(ItemType.FLASHBANG);

    [RandomEvent(EventType.InventoryEvent)]
    public static void GiveUSP()
        => Utility.RandomPlayer.GiveItem(ItemType.USP);

    [RandomEvent(EventType.InventoryEvent)]
    public static void SetHandToNull()
        => Utility.RandomPlayer.SetCurrentItem(ItemType.NULL);

    [RandomEvent(EventType.InventoryEvent)]
    public static void FillFlashbang()
    {
        Player rplr = Utility.RandomPlayer;
        for (int i = 0; i < 8; i++)
        {
            rplr.GiveItem(ItemType.FLASHBANG);
        }
    }

    [RandomEvent(EventType.InventoryEvent)]
    public static void RemoveRandomItem()
    {
        List<Smod2.API.Item> inv = Utility.RandomPlayer.GetInventory();

        int ritem = Utility.plugin.pr.Next(inv.Count);
        try { inv[ritem].Remove(); }
        catch (Exception) { Utility.plugin.Info("cant remove random item"); }
    }

    [RandomEvent(EventType.InventoryEvent)]
    public static void UpgradeInv()
    {
        Player rplr = Utility.RandomPlayer;
        foreach (Smod2.API.Item item in rplr.GetInventory())
        {
            if (item.ItemType == ItemType.NULL) break;
            int[] outputs = Scp914.singleton.recipes[(int)item.ItemType].outputs[(int)KnobSetting.FINE].outputs.ToArray();
            int finalitem = outputs[Utility.plugin.pr.Next(outputs.Length)];
            item.Remove();
            if (finalitem >= 0)
            {
                rplr.GiveItem((ItemType)finalitem);
            }
        }
    }

    [RandomEvent(EventType.InventoryEvent)]
    public static void DowngradeInv()
    {
        Player rplr = Utility.RandomPlayer;
        foreach (Smod2.API.Item item in rplr.GetInventory())
        {
            if (item.ItemType == ItemType.NULL) break;
            int[] outputs = Scp914.singleton.recipes[(int)item.ItemType].outputs[(int)KnobSetting.COARSE].outputs.ToArray();
            int finalitem = outputs[Utility.plugin.pr.Next(outputs.Length)];
            item.Remove();
            if (finalitem >= 0)
            {
                rplr.GiveItem((ItemType)finalitem);
            }
        }
    }
}

public class SpawnEvents
{
    [RandomEvent(EventType.SpawnEvent)]
    public static void SpawnFrag()
    {
        Player rplr = Utility.RandomPlayer;
        rplr.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, rplr.GetPosition(), false, 0);
    }

    [RandomEvent(EventType.SpawnEvent)]
    public static void SpawnFlash()
    {
        Player rplr = Utility.RandomPlayer;
        rplr.ThrowGrenade(ItemType.FLASHBANG, false, Vector.Zero, false, rplr.GetPosition(), false, 0);
    }

    [RandomEvent(EventType.SpawnEvent)]
    public static void USPDropClassD()
    {
        Player dclass = Utility.GetRandomPlayer(Role.CLASSD);
        if (dclass != null)
        {
            Utility.plugin.Server.Map.SpawnItem(ItemType.USP, dclass.GetPosition(), Vector.Zero);
            Utility.plugin.Server.Map.SpawnItem(ItemType.USP, dclass.GetPosition(), Vector.Zero);
        }
    }

    [RandomEvent(EventType.SpawnEvent)]
    public static void MicroHIDDrop()
        => Utility.plugin.Server.Map.SpawnItem(ItemType.MICROHID, Utility.RandomPlayer.GetPosition(), Vector.Zero);
}

public class RoleChangeEvents
{
    [RandomEvent(EventType.RoleChangeEvent)]
    public static void ClassDToScientist()
        => Utility.ChangePlayerFromTo(Role.CLASSD, Role.SCIENTIST);

    [RandomEvent(EventType.RoleChangeEvent)]
    public static void ScientistToClassD()
        => Utility.ChangePlayerFromTo(Role.SCIENTIST, Role.CLASSD);

    [RandomEvent(EventType.RoleChangeEvent, .04f)]
    public static void ClassDToRandomSCP()
        => Utility.ChangePlayerFromTo(Role.CLASSD, Utility.scpArray[Utility.plugin.pr.Next(Utility.scpArray.Length)]);

    [RandomEvent(EventType.RoleChangeEvent, .02f)]
    public static void UpgradeZombies()
    {
        foreach (Player player in Utility.plugin.Server.GetPlayers())
        {
            if (player.TeamRole.Role == Role.SCP_049_2) player.ChangeRole(Role.SCP_049, true, false, false);
        }
    }

    [RandomEvent(EventType.RoleChangeEvent, .04f)]
    public static void DowngradeSCP()
        => Utility.ChangePlayerFromTo(Smod2.API.Team.SCP, Role.CLASSD);

    [RandomEvent(EventType.RoleChangeEvent, .05f)]
    public static void DistributeSpectators()
    {
        foreach (Player player in Utility.plugin.Server.GetPlayers())
        {
            if (player.TeamRole.Role == Role.SPECTATOR)
            {
                Role randomRole = Utility.safeRoles[Utility.plugin.pr.Next(Utility.safeRoles.Length)];
                player.ChangeRole(randomRole);
            }
        }
    }

    [RandomEvent(EventType.RoleChangeEvent, .05f)]
    public static void LevelHundredBoss()
    {
        Player plr = Utility.GetRandomPlayer();
        plr.ChangeRole(Role.SCP_049_2);
        plr.SetHealth(1000);
    }
}

public class ServerEvents
{
    [RandomEvent(EventType.ServerEvent)]
    public static void LockUnlockDoors()
    {
        Utility.LockDoors();
        Utility.UnlockDoorsAfterSeconds(10);
    }

    [RandomEvent(EventType.ServerEvent)]
    public static void CloseAllDoors()
    {
        foreach (Smod2.API.Door door in Utility.plugin.Server.Map.GetDoors())
        {
            door.Open = false;
        }
    }

    [RandomEvent(EventType.ServerEvent)]
    public static void LockOpenImportantDoor()
    {
        Smod2.API.Door[] idoors = Utility.plugin.Server.Map.GetDoors().Where(d => string.IsNullOrEmpty(d.Name)).ToArray();
        Smod2.API.Door doo = idoors[Utility.plugin.pr.Next(idoors.Length)];
        doo.Open = true;
        doo.Locked = true;
        doo.Destroyed = true;
    }

    [RandomEvent(EventType.ServerEvent)]
    public static void StartWarhead()
    {
        AlphaWarheadController.host.StartDetonation();
        AlphaWarheadController.host.NetworktimeToDetonation = 90f;
    }

    [RandomEvent(EventType.ServerEvent)]
    public static void CloseLockUnlockDoors()
    {
        Utility.LockAndCloseDoors();
        Utility.UnlockDoorsAfterSeconds(10);
    }

    [RandomEvent(EventType.ServerEvent)]
    public static void EpicPrank()
        => Utility.plugin.Server.Map.AnnounceCustomMessage("ALLREMAINING PERSONNEL WILL BE EXECUTED IN 15 SECONDS");

    [RandomEvent(EventType.ServerEvent)]
    public static void SwapPlayers()
    {
        Player[] players = Utility.plugin.Server.GetPlayers().ToArray();
        for (int i = 0; i < players.Length; i = i + 2)
        {
            if (i + 1 >= players.Length) break;
            Vector first = players[i].GetPosition();
            players[i].Teleport(players[i + 1].GetPosition());
            players[i + 1].Teleport(first);
        }
    }

    public static void LightFlicker()
        => Utility.FlickerLights();
}