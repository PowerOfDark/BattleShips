using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.C2S.Game
{
    [ProtoContract]
    public class C2S_FireAt
    {
        [ProtoMember(1)]
        public int X { get; protected set; }
        [ProtoMember(2)]
        public int Y { get; protected set; }

        public C2S_FireAt(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        protected C2S_FireAt() { }
    }
}
