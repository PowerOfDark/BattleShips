using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Common
{
    [ProtoContract]
    public enum BoardOwner
    {
        ME, ENEMY
    }
}
