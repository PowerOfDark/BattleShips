using Common.Structures.Common;
using Common.Structures.Remote;
using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_SentPlayerInvite
    {
        [ProtoMember(1)]
        public PlayerDisplay Destination { get; protected set; }

        /// <summary>
        /// S2C packet informing about a sent invitation.
        /// </summary>
        /// <param name="destination">The player <paramref name="destination"/>, to which the invite was sent. </param>
        public S2C_SentPlayerInvite(PlayerDisplay destination)
        {
            this.Destination = destination;
        }


        protected S2C_SentPlayerInvite() { }
    }
}
