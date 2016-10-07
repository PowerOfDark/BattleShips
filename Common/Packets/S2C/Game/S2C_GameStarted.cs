using Common.Structures.Local;
using Common.Structures.Remote;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packets.S2C.Game
{
    [ProtoContract]
    public class S2C_GameStarted
    {
        [ProtoMember(1)]
        public SimpleBoard YourBoard { get; protected set; }

        public S2C_GameStarted(SimpleBoard board)
        {
            this.YourBoard = board;
        }

        protected S2C_GameStarted() { }
    }
}
