using Common.Structures.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_LobbyPlayerLeft
    {
        [ProtoMember(1)]
        public PlayerDisplay Player { get; protected set; }

        public S2C_LobbyPlayerLeft(PlayerDisplay Player)
        {
            this.Player = Player;
        }

        protected S2C_LobbyPlayerLeft() { }
    }
}
