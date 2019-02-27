using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smod2;
using Smod2.EventHandlers;
using Smod2.Events;

class RoundHandler : IEventHandlerRoundEnd, IEventHandlerRoundStart
{
    MainPlugin plugin;

    public RoundHandler(MainPlugin pl)
    {
        plugin = pl;
    }

    public void OnRoundEnd(RoundEndEvent ev)
    {
        plugin.roundStatus = false;
    }

    public void OnRoundStart(RoundStartEvent ev)
    {
        plugin.roundStatus = true;
        plugin.StartEventCycle();
    }
}
