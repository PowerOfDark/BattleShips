using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.C2S.Lobby
{
    [ProtoContract]
    public class C2S_SendPlayerInvite
    {
        [ProtoMember(1)]
        public ShortGuid Destination { get; protected set; }

        public C2S_SendPlayerInvite(ShortGuid destination)
        {
            this.Destination = destination;
        }

        protected C2S_SendPlayerInvite() { }
    }
}
