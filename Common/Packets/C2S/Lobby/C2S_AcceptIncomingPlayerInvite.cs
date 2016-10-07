using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.C2S.Lobby
{
    [ProtoContract]
    public class C2S_AcceptIncomingPlayerInvite
    {
        [ProtoMember(1)]
        public ShortGuid Source { get; protected set; }

        public C2S_AcceptIncomingPlayerInvite(ShortGuid source)
        {
            this.Source = source;
        }

        protected C2S_AcceptIncomingPlayerInvite() { }
    }
}
