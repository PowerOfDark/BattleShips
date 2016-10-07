using Common.Structures.Common;
using Common.Structures.Remote;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Lobby
{
    [ProtoContract]
    public class S2C_InitNewGame
    {
        [ProtoMember(1)]
        public PlayerDisplay Opponent { get; protected set; }
        [ProtoMember(2)]
        public PlayerSide Side { get; protected set; }
        [ProtoMember(3)]
        public int BoardSize { get; protected set; }
        [ProtoMember(4)]
        public IEnumerable<ShipInfo> Fleet { get; protected set; }
        [ProtoMember(5)]
        public bool ContinueTurnUntilMiss { get; protected set; }

        public S2C_InitNewGame(GameRuleSetBase gameRuleSet, PlayerDisplay opponent, PlayerSide side)
        {
            this.BoardSize = gameRuleSet.BoardSize;
            this.Fleet = gameRuleSet.GetFleet();
            this.ContinueTurnUntilMiss = gameRuleSet.ContinueTurnUntilMiss;
            this.Opponent = opponent;
            this.Side = side;
        }

        protected S2C_InitNewGame() { }
    }
}
