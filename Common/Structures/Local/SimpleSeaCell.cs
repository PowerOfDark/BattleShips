using Common.Structures.Remote;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Local
{
    [ProtoContract]
    public class SimpleSeaCell
    {
        [ProtoMember(1)]
        public int X { get; protected set; }
        [ProtoMember(2)]
        public int Y { get; protected set; }
        [ProtoMember(3)]
        public SeaCellState CellState { get; protected set; }

        public SimpleSeaCell(int x, int y, SeaCellState state)
        {
            this.X = x;
            this.Y = y;
            this.CellState = state;
        }

        protected SimpleSeaCell() { }
    }
}
