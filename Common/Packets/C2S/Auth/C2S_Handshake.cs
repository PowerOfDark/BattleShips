using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Common.Packets.C2S.Auth
{
    [ProtoContract]
    public class C2S_Handshake
    {
        [ProtoMember(1)]
        public string Version { get; protected set; }
        [ProtoMember(2)]
        public string Nickname { get; protected set; }

        public C2S_Handshake(string Version, string Nickname)
        {
            this.Version = Version;
            this.Nickname = Nickname;
        }

        protected C2S_Handshake() { }

    }
}
