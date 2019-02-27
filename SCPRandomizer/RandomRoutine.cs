using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RandomRoutine
{
    public RandomRoutine(Action delegete, EventType eventType, float weight = .1f, bool stalls = false)
    {
        this.eventType = eventType;
        this.weight = weight;
        this.stalls = stalls;

        this.name = delegete.Method.Name;

        this.delegete = delegete;
    }

    public EventType eventType;
    public float weight;
    public bool stalls;

    public string name;

    public Action delegete;
}