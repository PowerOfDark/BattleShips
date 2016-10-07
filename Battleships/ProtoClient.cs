using Common;
using Common.Packets.C2S.Auth;
using Common.Packets.C2S.Game;
using Common.Packets.C2S.Lobby;
using Common.Packets.S2C;
using Common.Packets.S2C.Game;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static NetworkCommsDotNet.NetworkComms;

namespace Battleships
{
    public static class ProtoClient
    {
        public const string VERSION = "v0.0.1";

        public delegate void DataHandler<in T>(T data);
        public static event DataHandler<EndPoint> OnServerDiscovered;

        /* VARIABLES */
        public static PlayerDisplay LocalPlayer { get; private set; }

        /* MAIN */
        public static event DataHandler<Connection> OnConnectionLost;

        /* AUTH */
        public static event PacketHandlerCallBackDelegate<S2C_HandshakeResponse> OnHandshakeReceived;

        /* LOBBY */
        public static event DataHandler<S2C_InitialLobbyData> OnInitialLobbyDataReceived;
        public static event DataHandler<S2C_LobbyPlayerJoined> OnLobbyPlayerJoined;
        public static event DataHandler<S2C_LobbyPlayerLeft> OnLobbyPlayerLeft;
        public static event DataHandler<S2C_ServerPlayersOnlineCount> OnServerPlayerCountChanged;

        public static event DataHandler<S2C_SentPlayerInvite> OnLobbyPlayerInviteSent;
        public static event DataHandler<S2C_RevokedSentPlayerInvite> OnLobbySentPlayerInviteRevoked;

        public static event DataHandler<S2C_IncomingPlayerInvite> OnLobbyPlayerInviteReceived;
        public static event DataHandler<S2C_RevokedIncomingPlayerInvite> OnLobbyIncomingPlayerInviteRevoked;

        public static event DataHandler<S2C_InitNewGame> OnInitNewGame;
        public static event DataHandler<S2C_GameStarted> OnGameStarted;
        public static event DataHandler<S2C_GameTurnInfo> OnGameTurnInfo;
        public static event DataHandler<S2C_GameBoardUpdated> OnGameBoardUpdated;
        public static event DataHandler<S2C_GameEnded> OnGameEnded;

        public static TCPConnection ServerConnection { get; private set; }

        /// <summary>
        /// Establish a connection to the server via TCP.
        /// </summary>
        /// <param name="query">IP:Port formatted host address eg. 127.0.0.1:12345</param>
        /// <returns>TCPConnection</returns>
        public static TCPConnection Connect(string query)
        {
            string[] split = query.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            int port;
            if (split.Length == 1)
            {
                port = Net.DEFAULT_PORT;
            }
            else if(split.Length == 2)
            {
                bool portOk = int.TryParse(split[1], out port);
                if (!portOk)
                    throw new FormatException("Incorrect host");
            }
            else
            {
                throw new FormatException("Incorrect host");
            }
            return ServerConnection = TCPConnection.GetConnection(new NetworkCommsDotNet.ConnectionInfo(new IPEndPoint(IPAddress.Parse(split[0]), port)), Net.SEND_RECEIVE_OPTIONS);
        }

        public static void Bind()
        {
            Net.Init();

            Net.On<S2C_HandshakeResponse>((header, connection, obj) => 
            {
                if (!obj.OK)
                    connection.CloseConnection(false);
                LocalPlayer = obj.LocalPlayer;
                //Now we're authed, so losing connection is fatal.
                connection.AppendShutdownHandler((c) => 
                {
                    Reset();
                    OnConnectionLost?.Invoke(c);
                });
                OnHandshakeReceived?.Invoke(header, connection, obj);
            });

            Net.On<S2C_InitialLobbyData>((header, connection, obj) => 
            {
                OnInitialLobbyDataReceived?.Invoke(obj);
            });
            Net.On<S2C_LobbyPlayerJoined>((header, connection, obj) =>
            {
                OnLobbyPlayerJoined?.Invoke(obj);
            });
            Net.On<S2C_LobbyPlayerLeft>((header, connection, obj) => 
            {
                OnLobbyPlayerLeft?.Invoke(obj);
            });
            Net.On<S2C_ServerPlayersOnlineCount>((header, connection, obj) =>
            {
                OnServerPlayerCountChanged?.Invoke(obj);
            });
            Net.On<S2C_SentPlayerInvite>((header, connection, obj) => 
            {
                OnLobbyPlayerInviteSent?.Invoke(obj);
            });
            Net.On<S2C_RevokedSentPlayerInvite>((header, connection, obj) => 
            {
                OnLobbySentPlayerInviteRevoked?.Invoke(obj);
            });
            Net.On<S2C_IncomingPlayerInvite>((header, connection, obj) => 
            {
                OnLobbyPlayerInviteReceived?.Invoke(obj);
            });
            Net.On<S2C_RevokedIncomingPlayerInvite>((header, connection, obj) =>
            {
                OnLobbyIncomingPlayerInviteRevoked?.Invoke(obj);
            });

            Net.On<S2C_InitNewGame>((header, connection, obj) =>
            {
                OnInitNewGame?.Invoke(obj);
            });
            Net.On<S2C_GameStarted>((header, connection, obj) => 
            {
                OnGameStarted?.Invoke(obj);
            });
            Net.On<S2C_GameTurnInfo>((header, connection, obj) => 
            {
                OnGameTurnInfo?.Invoke(obj);
            });
            Net.On<S2C_GameBoardUpdated>((header, connection, obj) => 
            {
                OnGameBoardUpdated?.Invoke(obj);
            });
            Net.On<S2C_GameEnded>((header, connection, obj) => 
            {
                OnGameEnded?.Invoke(obj);
            });
        }

        public static void DiscoverServer()
        {
            Net.ConfigurePeerDiscovery();
            if (!PeerDiscovery.IsDiscoverable(Net.PEER_DISCOVERY_METHOD))
            {
                PeerDiscovery.EnableDiscoverable(Net.PEER_DISCOVERY_METHOD);
            }
            PeerDiscovery.OnPeerDiscovered += PeerDiscovery_OnPeerDiscovered;
            PeerDiscovery.DiscoverPeersAsync(Net.PEER_DISCOVERY_METHOD);
        }

        private static void PeerDiscovery_OnPeerDiscovered(ShortGuid peerIdentifier, Dictionary<ConnectionType, List<EndPoint>> discoveredListenerEndPoints)
        {
            var matching = discoveredListenerEndPoints.Where(t => t.Key == ConnectionType.TCP).Select(t => t.Value);
            foreach(var list in matching)
            {
                foreach(var endpoint in list)
                {
                    if(endpoint.ToString().EndsWith(":" + Net.DEFAULT_PORT))
                    {
                        Console.WriteLine(endpoint.ToString());
                        OnServerDiscovered(endpoint);
                    }
                }
            }
        }

        public static void SendHandshake(string version, string nickname)
        {
            ServerConnection.Send(new C2S_Handshake(version, nickname));
        }

        public static void JoinJobby()
        {
            ServerConnection.Send(new C2S_JoinLobby());
        }

        public static void SendPlayerInvite(ShortGuid destination)
        {
            ServerConnection.Send(new C2S_SendPlayerInvite(destination));
        }

        public static void RevokeSentPlayerInvite(ShortGuid destination)
        {
            ServerConnection.Send(new C2S_RevokeSentPlayerInvite(destination));
        }

        public static void AcceptIncomingPlayerInvite(ShortGuid source)
        {
            ServerConnection.Send(new C2S_AcceptIncomingPlayerInvite(source));
        }

        public static void Reset()
        {
            LocalPlayer = null;
            ServerConnection = null;
        }

        public static void GameReady()
        {
            ServerConnection.Send(new C2S_GameReady());
        }

        public static void FireAt(int x, int y)
        {
            ServerConnection.Send(new C2S_FireAt(x, y));
        }

    }
}
