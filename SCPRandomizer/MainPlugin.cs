using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Text;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Coroutines;

using Team = Smod2.API.Team;

[PluginDetails(
    name = "SCPRandomizer",
    description = "",
    author = "FrostBytes#1337",
    id = "frost.scprandomizer",
    version = "1.0",
    SmodMajor = 3,
    SmodMinor = 3,
    SmodRevision = 0
    )]
public class MainPlugin : Plugin
{
    public bool roundStatus = false;
    public Random pr = new Random();
    AutoResetEvent thread = new AutoResetEvent(false);
    public double eventChance = 0.5;
    public Role[] scpArray = {Role.SCP_049, Role.SCP_049_2, Role.SCP_096, Role.SCP_106, Role.SCP_173, Role.SCP_939_53};

    public override void OnDisable()
    {
        throw new NotImplementedException();
    }

    public override void OnEnable()
    {
        Type t = this.GetType();
        PluginDetails attribute = t.GetCustomAttributes(typeof(PluginDetails), true).FirstOrDefault() as PluginDetails;
        if (attribute != null) {
            Info("Plugin " + attribute.name + " has initialized");
        }
    }

    public override void Register()
    {
        AddEventHandlers(new RoundHandler(this), Smod2.Events.Priority.Lowest);
    }

    public void RunNextEvent()
    {
        StartCoroutine(DoRandomEvent());
    }

    public IEnumerable<float> DoRandomEvent() 
    {
        while (true)
        {
            if (roundStatus)
            {
                yield return Coroutine.WaitForSeconds(5);
                Info("Running next event...");
                if (Round.Duration > 900)
                {
                    Info("The round is too long! End it!");
                    Server.Map.DetonateWarhead();
                    break;
                }
                EventType events = EventType.None;

                Random r = new Random();
                // ugly code repeating (plan to extend this which is why I'm using an enum)
                if (r.NextDouble() <= eventChance)
                    events = events | EventType.InventoryEvent;
                if (r.NextDouble() <= eventChance / 3)
                    events = events | EventType.SpawnEvent;
                if (r.NextDouble() <= eventChance / 6)
                    events = events | EventType.RoleChangeEvent;
                if (r.NextDouble() <= eventChance / 6)
                    events = events | EventType.ServerEvent;

                // its time
                if ((events & EventType.InventoryEvent) > 0) DoRandomInvEvent();
                if ((events & EventType.SpawnEvent) > 0) DoRandomSpawnEvent();
                if ((events & EventType.RoleChangeEvent) > 0) DoRandomRoleEvent();
                if ((events & EventType.ServerEvent) > 0) DoRandomServerEvent();
            }
            else { Info("Round ended. Not running the event."); }
        }
    }

    private void DoRandomServerEvent()
    {
        int eve = GetRandom(8);
        switch (eve)
        {
            case 0:
                Info("lock doors, and unlock after 15 seconds");
                LockDoors();
                StartCoroutine(UnlockDoors());
                break;
            case 1:
                Info("open all doors");
                foreach (Smod2.API.Door door in Server.Map.GetDoors())
                {
                    door.Open = false;
                }
                break;
            case 2:
                Info("destroy open important door");
                Smod2.API.Door[] idoors = Server.Map.GetDoors().Where(d =>
                {
                    return !string.IsNullOrEmpty(d.Name);
                }).ToArray();
                Smod2.API.Door doo = idoors[GetRandom(idoors.Length)];
                doo.Open = true;
                doo.Locked = true;
                doo.Destroyed = true;
                break;
            case 3:
                Info("warhead");
                Server.Map.StartWarhead();
                break;
            case 4:
                Info("lock and close doors, then unlock after 15 seconds");
                LockAndCloseDoors();
                StartCoroutine(UnlockDoors());
                break;
            case 5:
                Info("everyone will explode in 15 seconds");
                Server.Map.AnnounceCustomMessage("Everyone will explode in 15 seconds");
                break;
            case 6:
                Info("swap players");
                Player[] players = Server.GetPlayers().ToArray();
                for (int i = 0; i < players.Length; i = i + 2)
                {
                    if (i + 1 >= players.Length) break;
                    Vector first = players[i].GetPosition();
                    players[i].Teleport(players[i + 1].GetPosition());
                    players[i + 1].Teleport(first);
                }
                break;
            case 7:
                Info("flicker lights");
                FlickerLights();
                break;
            default:
                break;
        }
    }

    private void DoRandomRoleEvent()
    {
        int eve = GetRandom(7);
        switch (eve)
        {
            case 0:
                Info("classd to scientist");
                ChangePlayerFromTo(Role.CLASSD, Role.SCIENTIST);
                break;
            case 1:
                Info("scientist to classd");
                ChangePlayerFromTo(Role.SCIENTIST, Role.CLASSD);
                break;
            case 2:
                Info("classd to random scp");
                ChangePlayerFromTo(Role.CLASSD, scpArray[GetRandom(scpArray.Length)]);
                break;
            case 3:
                Info("upgrade zombois");
                foreach (Smod2.API.Player player in Server.GetPlayers())
                {
                    if (player.TeamRole.Role == Role.SCP_049_2) player.ChangeRole(Role.SCP_049, true, false, false);
                }
                break;
            case 4:
                Info("SCP to ClassD");
                ChangePlayerFromTo(Smod2.API.Team.SCP, Role.CLASSD);
                break;
            case 5:
                Info("distribute spectators");
                if (Round.Duration > 600) break;
                foreach (Player player in Server.GetPlayers())
                {
                    if (player.TeamRole.Role == Role.SPECTATOR)
                    {
                        Role randomRole = (Role)GetRandom(18);
                        randomRole = randomRole == Role.SPECTATOR ? Role.CLASSD : randomRole;
                        player.ChangeRole(randomRole);
                    }
                }
                break;
            case 6:
                Info("1000 hp zombie");
                Player plr = GetRandomPlayer();
                plr.ChangeRole(Role.SCP_049_2);
                plr.SetHealth(1000);
                break;
            default:
                break;
        }
    }

    private void DoRandomSpawnEvent()
    {
        Player rplr = GetRandomPlayer();
        int eve = GetRandom(4);
        switch (eve)
        {
            case 0:
                Info("spawn frag");
                rplr.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, rplr.GetPosition(), false, 0);
                break;
            case 1:
                Info("spawn flash");
                rplr.ThrowGrenade(ItemType.FLASHBANG, false, Vector.Zero, false, rplr.GetPosition(), false, 0);
                break;
            case 2:
                Info("give two usps");
                Player dclass = GetRandomPlayerWithMask(Role.CLASSD);
                if (dclass != null)
                {
                    Server.Map.SpawnItem(ItemType.USP, dclass.GetPosition(), Vector.Zero);
                    Server.Map.SpawnItem(ItemType.USP, dclass.GetPosition(), Vector.Zero);
                }
                break;
            case 3:
                Info("microhid drop");
                Server.Map.SpawnItem(ItemType.MICROHID, rplr.GetPosition(), Vector.Zero);
                break;
            default:
                break;
        }
    }

    private void DoRandomInvEvent()
    {
        Scp914 machine = Scp914.singleton;
        // get player
        Player rplr = GetRandomPlayer();
        // for convience
        List<Smod2.API.Item> inv = rplr.GetInventory();

        // get event
        int eve = GetRandom(10);
        switch (eve)
        {
            case 0:
                Info("all cups");
                foreach (Smod2.API.Item item in inv)
                {
                    item.Remove();
                }
                for (int i = 0; i < 8; i++)
                {
                    rplr.GiveItem(ItemType.CUP);
                }
                break;
            case 1:
                Info("all keycards => janitor");
                foreach (Smod2.API.Item item in inv)
                {
                    int it = (int)item.ItemType;
                    if (0 <= it && it <= 11)
                    {
                        item.Remove();
                        rplr.GiveItem(ItemType.JANITOR_KEYCARD);
                    }
                }
                break;
            case 2:
                Info("give o5");
                rplr.GiveItem(ItemType.O5_LEVEL_KEYCARD);
                break;
            case 3:
                Info("flashbang");
                rplr.GiveItem(ItemType.FLASHBANG);
                break;
            case 4:
                Info("usp");
                rplr.GiveItem(ItemType.USP);
                break;
            case 5:
                Info("flashbang fill");
                for (int i = 0; i < 8; i++)
                {
                    rplr.GiveItem(ItemType.FLASHBANG);
                }
                break;
            case 6:
                Info("hand to null");
                rplr.SetCurrentItem(ItemType.NULL);
                break;
            case 7:
                Info("remove random item");
                int ritem = pr.Next(inv.Count); // GetRandom(inv.Count);
                try { inv[ritem].Remove(); }
                catch (Exception) { Info("cant remove random item"); }
                break;
            case 8:
                Info("upgrade");
                foreach (Smod2.API.Item item in inv)
                {
                    if (item.ItemType == ItemType.NULL) break;
                    int[] outputs = machine.recipes[(int)item.ItemType].outputs[(int)KnobSetting.FINE].outputs.ToArray();
                    int finalitem = outputs[GetRandom(outputs.Length)];
                    item.Remove();
                    if (finalitem >= 0)
                    {
                        rplr.GiveItem((ItemType) finalitem);
                    }
                }
                break;
            case 9:
                Info("downgrade");
                foreach (Smod2.API.Item item in inv)
                {
                    if (item.ItemType == ItemType.NULL) break;
                    int[] outputs = machine.recipes[(int)item.ItemType].outputs[(int)KnobSetting.COARSE].outputs.ToArray();
                    int finalitem = outputs[GetRandom(outputs.Length)];
                    item.Remove();
                    if (finalitem >= 0)
                    {
                        rplr.GiveItem((ItemType)finalitem);
                    }
                }
                break;
            default:
                break;
        }
    }

    private Player GetRandomPlayer()
    {
        List<Player> plrs = Server.GetPlayers();
        for (int i = plrs.Count - 1; i > 0; i--)
        {
            if (plrs[i].TeamRole.Role == Role.SPECTATOR)
            {
                plrs.RemoveAt(i);
            }
        }
        if (plrs.Count <= 0) throw new Exception("No players in server. Stopping events");
        int chosen = GetRandom(plrs.Count);
        return plrs[chosen];
    }

    private Player GetRandomPlayerWithMask(Role role)
    {
        List<Player> plrs = Server.GetPlayers();
        for (int i = plrs.Count - 1; i > 0; i--)
        {
            Role rol = plrs[i].TeamRole.Role;
            if (rol != role) {
                plrs.RemoveAt(i);
            }
        }
        if (plrs.Count <= 0) return null;
        int chosen = GetRandom(plrs.Count);
        return plrs[chosen];
    }

    private Player GetRandomPlayerWithMask(Smod2.API.Team role)
    {
        List<Player> plrs = Server.GetPlayers();
        for (int i = plrs.Count - 1; i > 0; i--)
        {
            Smod2.API.Team rol = plrs[i].TeamRole.Team;
            if (rol != role)
            {
                plrs.RemoveAt(i);
            }
        }
        if (plrs.Count <= 0) return null;
        int chosen = GetRandom(plrs.Count);
        return plrs[chosen];
    }

    private void ChangePlayerFromTo(Role from, Role to)
    {
        Player plr = GetRandomPlayerWithMask(from);
        if (plr == null) return;
        plr.ChangeRole(to, false, false, false, true);
    }

    private void ChangePlayerFromTo(Smod2.API.Team from, Role to)
    {
        Player plr = GetRandomPlayerWithMask(from);
        if (plr == null) return;
        plr.ChangeRole(to, false, false, false, true);
    }

    private int GetRandom(int max) // max > n
    {
        return (int) Math.Floor(pr.NextDouble() * max);
    }

    private void LockDoors()
    {
        foreach (Smod2.API.Door door in Server.Map.GetDoors())
        {
            door.Locked = true;
        }
    }

    private void LockAndCloseDoors()
    {
        foreach (Smod2.API.Door door in Server.Map.GetDoors())
        {
            door.Open = false;
            door.Locked = true;
        }
    }

    private IEnumerable<float> UnlockDoors()
    {
        yield return Coroutine.WaitForSeconds(5);
        foreach (Smod2.API.Door door in Server.Map.GetDoors())
        {
            door.Locked = false;
        }
    }

    private void FlickerLights()
    {
        Room[] rooms = Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(room =>
        {
            return room.ZoneType != ZoneType.ENTRANCE;
        }).ToArray();
        foreach (Room room in rooms)
        {
            room.FlickerLights();
        }
    }
}
