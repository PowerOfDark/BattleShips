using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_ServerPlayersOnlineCount
    {
        [ProtoMember(1)]
        public int PlayersOnline { get; protected set; }

        public S2C_ServerPlayersOnlineCount(int PlayersOnline)
        {
            this.PlayersOnline = PlayersOnline;
        }

        protected S2C_ServerPlayersOnlineCount() { }
    }
}
