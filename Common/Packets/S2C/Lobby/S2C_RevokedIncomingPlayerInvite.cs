using Common.Structures.Common;
using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_RevokedIncomingPlayerInvite
    {
        [ProtoMember(1)]
        public PlayerDisplay Source { get; protected set; }

        public S2C_RevokedIncomingPlayerInvite(PlayerDisplay source)
        {
            this.Source = source;
        }

        protected S2C_RevokedIncomingPlayerInvite() { }
    }
}
