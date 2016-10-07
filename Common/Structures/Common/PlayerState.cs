using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Common
{
    [Flags]
    public enum PlayerState
    {
        NONE = 0, HANDSHAKE_OK = 1, IN_LOBBY = 2, IN_GAME = 4, POST_GAME = 8,
    }
}
