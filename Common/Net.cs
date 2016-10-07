using Common.Structures.Surrogates;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using ProtoBuf.Meta;
using SharpZipLibCompressor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using static NetworkCommsDotNet.NetworkComms;

namespace Common
{
    public static class Net
    {
        public static readonly SendReceiveOptions SEND_RECEIVE_OPTIONS = new SendReceiveOptions<ProtobufSerializer>();// SharpZipLibGzipCompressor
        public const PeerDiscovery.DiscoveryMethod PEER_DISCOVERY_METHOD = PeerDiscovery.DiscoveryMethod.UDPBroadcast;
        public const int DEFAULT_PORT = 54321;

        public const bool ENABLE_DISCOVERY = false;

        public static void On<T>(PacketHandlerCallBackDelegate<T> handler, string description = "")
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<T>(typeof(T).Name + description, handler, SEND_RECEIVE_OPTIONS);
        }

        public static void On<T>(this Connection c, PacketHandlerCallBackDelegate<T> handler, string description = "")
        {
            c.AppendIncomingPacketHandler<T>(typeof(T).Name + description, handler, SEND_RECEIVE_OPTIONS);
        }

        public static bool Send<T>(this Connection connection, T obj, string description = "")
        {
            if (connection != null)
            {
                try
                {
                    connection.SendObject<T>(typeof(T).Name + description, obj);
                    return true;
                }
                catch { }
            }
            return false;
        }

        public static T_Receive SendReceive<T_Receive>(this Connection connection, string sendType, string receiveDescription = "", int timeout = 0)
        {
            return connection.SendReceiveObject<T_Receive>(sendType, typeof(T_Receive).Name + receiveDescription, timeout);
        }

        public static void ConfigurePeerDiscovery()
        {
            PeerDiscovery.MaxTargetLocalIPPort = 20011;
            PeerDiscovery.MinTargetLocalIPPort = 20000;
        }

        public static void Init()
        {
            RuntimeTypeModel.Default.Add(typeof(ShortGuid), false).SetSurrogate(typeof(ShortGuidSurrogate));
        }
    }
}
