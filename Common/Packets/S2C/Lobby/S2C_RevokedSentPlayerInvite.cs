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
    public class S2C_RevokedSentPlayerInvite
    {
        [ProtoMember(1)]
        public PlayerDisplay Destination { get; protected set; }

        public S2C_RevokedSentPlayerInvite(PlayerDisplay destination)
        {
            this.Destination = destination;
        }

        protected S2C_RevokedSentPlayerInvite() { }
    }
}
