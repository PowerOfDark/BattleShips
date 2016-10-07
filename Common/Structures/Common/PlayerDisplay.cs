using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Common
{
    [ProtoContract]
    public class PlayerDisplay
    {
        [ProtoMember(1)]
        public ShortGuid PlayerID { get; protected set; }
        [ProtoMember(2)]
        public string Nickname { get; private set; }


        public PlayerDisplay(ShortGuid netId, string name)
        {
            this.PlayerID = netId;
            this.Nickname = name;
        }

        protected PlayerDisplay() { }
    }
}
