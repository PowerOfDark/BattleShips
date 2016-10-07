using Common.Structures.Common;
using Common.Structures.Local;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Game
{
    [ProtoContract]
    public class S2C_GameBoardUpdated
    {
        [ProtoMember(1)]
        public BoardOwner BoardOwner { get; set; }
        [ProtoMember(2)]
        public IEnumerable<SimpleSeaCell> UpdatedCells { get; protected set; }

        public S2C_GameBoardUpdated(BoardOwner boardOwner, IEnumerable<SimpleSeaCell> updatedCells)
        {
            this.BoardOwner = boardOwner;
            this.UpdatedCells = updatedCells;
        }

        protected S2C_GameBoardUpdated() { }
    }
}
