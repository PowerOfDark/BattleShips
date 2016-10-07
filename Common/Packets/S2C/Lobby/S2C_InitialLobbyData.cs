using Common.Structures.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_InitialLobbyData
    {
        [ProtoMember(1)]
        public PlayerDisplay[] AvailablePlayers { get; set; }
        [ProtoMember(2)]
        public int PlayersOnline { get; set; }

        public S2C_InitialLobbyData() { }
    }
}
