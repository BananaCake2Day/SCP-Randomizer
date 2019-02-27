using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class RandomEventAttribute : Attribute
{
    public RandomEventAttribute(EventType type = EventType.None, float weight = .1f, bool stalls = false)
    {
        EventType = type;
        Weight = weight;
        Stalls = stalls;
    }

    public EventType EventType { get; }
    public float Weight { get; }
    public bool Stalls { get; set; }
}
