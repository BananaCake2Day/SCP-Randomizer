using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Flags]
enum EventType : byte
{
    None = 0,
    InventoryEvent = 1,
    SpawnEvent = 2,
    RoleChangeEvent = 4,
    PositionEvent = 8,
    ServerEvent = 16,
    SCPEvent = 32
}
