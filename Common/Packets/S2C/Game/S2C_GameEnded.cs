using Common.Structures.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Game
{
    [ProtoContract]
    public class S2C_GameEnded
    {
        [ProtoMember(1)]
        public BoardOwner Winner { get; protected set; }
        [ProtoMember(2)]
        public GameResult GameResult { get; protected set; }

        public S2C_GameEnded(BoardOwner winner, GameResult gameResult)
        {
            this.Winner = winner;
            this.GameResult = gameResult;
        }

        protected S2C_GameEnded() { }
    }
}
