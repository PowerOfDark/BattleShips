using NetworkCommsDotNet.Tools;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Surrogates
{
    [ProtoContract]
    public class ShortGuidSurrogate
    {
        [ProtoMember(1)]
        public string ShortGuidString { get; set; }

        public static implicit operator ShortGuidSurrogate(ShortGuid value)
        {
            return new ShortGuidSurrogate { ShortGuidString = value.Value };
        }

        public static implicit operator ShortGuid(ShortGuidSurrogate value)
        {
            return new ShortGuid(value.ShortGuidString);
        }
    }
}
