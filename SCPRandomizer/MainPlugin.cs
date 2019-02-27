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
    private readonly string ducktales = "https://www.youtube.com/watch?v=J3L30PH1hzU";
    public bool roundStatus = false;
    public Random pr = new Random();

    public List<RandomRoutine> randomRoutines = new List<RandomRoutine>();

    public override void OnDisable()
    {
        throw new NotImplementedException();
    }

    public override void OnEnable()
    {
        Utility.plugin = this;

        RegisterModule(typeof(ServerEvents));
        RegisterModule(typeof(InventoryEvents));
        RegisterModule(typeof(RoleChangeEvents));
        RegisterModule(typeof(SpawnEvents));

        Info("Plugin " + this.Details.name +  " has initialized");
        Info(ducktales + " cool video");
    }

    public override void Register()
    {
        AddEventHandlers(new RoundHandler(this), Smod2.Events.Priority.Lowest);
    }

    public void RegisterModule(Type type)
    {
        foreach (MethodInfo method in type.GetMethods())
        {
            RandomEventAttribute eventAttribute = method.GetCustomAttribute<RandomEventAttribute>();
            if (eventAttribute != null)
            {
                RandomRoutine rr = new RandomRoutine((Action) Delegate.CreateDelegate(typeof(Action), method), 
                    eventAttribute.EventType, eventAttribute.Weight, eventAttribute.Stalls);
                randomRoutines.Add(rr);
            }
        }
    }

    public void StartEventCycle()
    {
        // finally start the dorandomevent coroutine to start the cycle
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

                RandomRoutine[] routines = Utility.Shuffle(randomRoutines.Where(routine => 
                {
                    bool safe = true;
                    // if stalls, do not keep
                    if (routine.stalls && Round.Duration > 600) safe = false;
                    return safe;
                }).ToArray());
                
                foreach (RandomRoutine routine in routines)
                {
                    float rng = (float) pr.NextDouble();
                    if (rng <= routine.weight)
                    {
                        routine.delegete.Invoke();
                    }
                }
            }
            else { Info("Round ended. Not running the event."); }
        }
    }
}
