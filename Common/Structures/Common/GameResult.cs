using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Common
{
    [ProtoContract]
    public enum GameResult
    {
        ALL_SHIPS_SUNK, PLAYER_DISCONNECTED
    }
}
