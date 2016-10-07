using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Common
{
    public static class Extensions
    {
        public static ShortGuid GetID(this Connection connection)
        {
            return connection.ConnectionInfo.NetworkIdentifier;
        }
    }
}
