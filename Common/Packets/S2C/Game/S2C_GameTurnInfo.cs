using Common.Structures.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Game
{
    [ProtoContract]
    public class S2C_GameTurnInfo
    {
        [ProtoMember(1)]
        public bool Reason_Hit { get; protected set; }
        [ProtoMember(2)]
        public BoardOwner Player { get; protected set; }


        public S2C_GameTurnInfo(BoardOwner player, bool reasonHit)
        {
            this.Player = player;
            this.Reason_Hit = reasonHit;
        }

        protected S2C_GameTurnInfo() { }
    }
}
