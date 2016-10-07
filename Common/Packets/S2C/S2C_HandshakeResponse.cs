using Common.Structures.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Common.Packets.S2C
{
    [ProtoContract]
    public class S2C_HandshakeResponse
    {
        [ProtoMember(1)]
        public bool VersionOK { get; set; }
        [ProtoMember(2)]
        public bool NicknameOK { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public PlayerDisplay LocalPlayer { get; set; }

        public bool OK { get { return this.VersionOK && this.NicknameOK; } }

        public S2C_HandshakeResponse(bool VersionOK, bool NicknameOK, string Message, PlayerDisplay LocalPlayer)
        {
            this.VersionOK = VersionOK;
            this.NicknameOK = NicknameOK;
            this.Message = Message;
            this.LocalPlayer = LocalPlayer;
        }

        protected S2C_HandshakeResponse() { }

    }
}
