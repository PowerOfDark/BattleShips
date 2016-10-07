using Common.Structures.Common;
using Common.Structures.Remote;
using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C
{
    [ProtoContract]
    public class S2C_IncomingPlayerInvite
    {
        [ProtoMember(1)]
        public PlayerDisplay Source { get; protected set; }

        /// <summary>
        /// S2C packet informing about an incoming game invitiation.
        /// </summary>
        /// <param name="from">The player <paramref name="from"/>, that sent this game invite. </param>
        public S2C_IncomingPlayerInvite(PlayerDisplay source)
        {
            this.Source = source;
        }


        protected S2C_IncomingPlayerInvite() { }
    }
}
