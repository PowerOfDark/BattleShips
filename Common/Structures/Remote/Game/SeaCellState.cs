using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{
    [ProtoContract]
    public enum SeaCellState
    {
        SEA = 0, FIRE_MISSED = 1, SHIP = 2, SHIP_HIT = 3, SHIP_SUNK = 4
    }
}
