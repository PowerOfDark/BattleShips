using Common;
using Common.Packets.C2S.Auth;
using Common.Packets.C2S.Game;
using Common.Packets.C2S.Lobby;
using Common.Packets.S2C;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using Common.Structures.Remote;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ProtoServer
    {
        public const string VERSION = "v0.0.1";
        public const int SERVER_PORT = Net.DEFAULT_PORT;

        public static TCPConnectionListener Listener;

        public static Dictionary<ShortGuid, RemotePlayer> Players = new Dictionary<ShortGuid, RemotePlayer>();


        /// <summary>
        /// Start the TCP listener
        /// </summary>
        public static void Listen()
        {
            if (Net.ENABLE_DISCOVERY)
            {
                Net.ConfigurePeerDiscovery();
                PeerDiscovery.EnableDiscoverable(Net.PEER_DISCOVERY_METHOD);
            }
            NetworkComms.EnableLogging(new LiteLoggerDebug(LiteLoggerDebug.LogMode.ConsoleOnly));
            Connection.StartListening(Listener = new TCPConnectionListener(Net.SEND_RECEIVE_OPTIONS, ApplicationLayerProtocolStatus.Enabled, true), new IPEndPoint(IPAddress.Any, Net.DEFAULT_PORT));
        }

        public static void Bind()
        {
            Net.Init();
            NetworkComms.AppendGlobalConnectionEstablishHandler((connection) =>
            {
                // todo: Automatically disconnect after 60 seconds without a handshake
            });

            NetworkComms.AppendGlobalConnectionCloseHandler((connection) => 
            {
                DisconnectPlayer(connection.GetID());
            });


            Net.On<C2S_Handshake>((header, connection, obj) =>
            {
                var response = new S2C_HandshakeResponse(obj.Version == VERSION, IsNicknameValid(obj.Nickname), "OK", null);
                if (IsValidPlayer(connection.GetID()))
                {
                    //handshake for second time.. let's disconnect
                    DisconnectPlayer(connection.GetID());
                }
                if (!response.OK)
                {
                    response.Message = $"Error:\n Version: {response.VersionOK}\n Nickname: {response.NicknameOK}";
                    if(!response.NicknameOK)
                    {
                        response.Message += "\n Nickname length must be within 3-16 characters and MUST be unique.";
                    }
                }
                else
                {
                    // OK!
                    var player = new RemotePlayer() { Connection = connection, Nickname = obj.Nickname, PlayerState = PlayerState.HANDSHAKE_OK };
                    response.LocalPlayer = player.GetDisplay();
                    AddPlayer(player);
                }
                connection.Send(response);
            });

            Net.On<C2S_JoinLobby>((header, connection, obj) => 
            {
                RemotePlayer player;
                if (IsValidPlayer(connection.GetID(), out player))
                {
                    LobbyServer.EnterLobby(player);
                }
            });

            Net.On<C2S_SendPlayerInvite>((header, connection, obj) => 
            {
                RemotePlayer source, destination;
                if(IsValidPlayer(connection.GetID(), out source, PlayerState.IN_LOBBY)
                    && IsValidPlayer(obj.Destination, out destination, PlayerState.IN_LOBBY))
                {
                    source.SendInvite(destination);
                }
            });

            Net.On<C2S_RevokeSentPlayerInvite>((header, connection, obj) => 
            {
                RemotePlayer source, destination;
                if (IsValidPlayer(connection.GetID(), out source, PlayerState.IN_LOBBY)
                     && IsValidPlayer(obj.Destination, out destination, PlayerState.IN_LOBBY))
                {
                    source.RevokeSentInvite(destination);
                }
            });

            Net.On<C2S_AcceptIncomingPlayerInvite>((header, connection, obj) =>
            {
                RemotePlayer source, destination;
                if (IsValidPlayer(obj.Source, out source, PlayerState.IN_LOBBY)
                     && IsValidPlayer(connection.GetID(), out destination, PlayerState.IN_LOBBY))
                {
                    if(source.HasSentInviteTo(destination))
                    {
                        //We have a pair :)
                        LobbyServer.LeaveLobby(source);
                        LobbyServer.LeaveLobby(destination);
                        GameServer.InitNewGame(source, destination);
                    }
                }
            });

            Net.On<C2S_GameReady>((header, connection, obj) =>
            {
                RemotePlayer player;
                if (IsValidPlayer(connection.GetID(), out player, PlayerState.IN_GAME))
                {
                    var p = player.Game.GetPlayer(player);
                    player.Game.OnPlayerReady(p);
                }
            });

            Net.On<C2S_FireAt>((header, connection, obj) => 
            {
                RemotePlayer player;
                if(IsValidPlayer(connection.GetID(), out player, PlayerState.IN_GAME))
                {
                    var p = player.Game.GetPlayer(player);
                    p.FireAtEnemy(obj.X, obj.Y);
                }
            });

        }
    
        public static void Broadcast<T>(T obj, PlayerState selector = PlayerState.NONE)
        {
            foreach(var Player in GetPlayerList())
            {
                if((Player.PlayerState & selector) == selector)
                {
                    Player.Connection.Send(obj);
                }
            }
        }

        public static bool IsValidPlayer(ShortGuid id, out RemotePlayer playerOut, PlayerState selector = PlayerState.NONE)
        {
            lock(Players)
            {
                if(Players.ContainsKey(id))
                {
                    var player = Players[id];
                    playerOut = player;
                    return (player.PlayerState & selector) == selector;
                }
            }
            playerOut = null;
            return false;
        }
        public static bool IsValidPlayer(ShortGuid id, PlayerState selector = PlayerState.NONE)
        {
            lock (Players)
            {
                if (Players.ContainsKey(id))
                {
                    var player = Players[id];
                    return (player.PlayerState & selector) == selector;
                }
            }
            return false;
        }
        
        public static RemotePlayer GetPlayer(ShortGuid id)
        {
            lock (Players)
            {
                try
                {
                    return Players[id];
                }
                catch
                {
                    return null;
                }
            }
        }

        public static List<RemotePlayer> GetPlayerList()
        {
            List<RemotePlayer> list;
            lock(Players)
            {
                list = Players.Values.ToList();
            }
            return list;
        }

        public static int GetPlayerCount()
        {
            int i = 0;
            lock(Players)
            {
                i = Players.Count;
            }
            return i;
        }

        public static bool IsNicknameValid(string nickname)
        {
            var tmp = new RemotePlayer() { Nickname = nickname.ToLower().Replace(" ", "").Trim() };
            if(tmp.Nickname.Length >= 3 && tmp.Nickname.Length <= 16)
            {
                //return true if this player's name hasn't appeared before
                return !GetPlayerList().Any(t => t.Nickname.ToLower().Replace(" ", "").Trim() == tmp.Nickname);
            }
            return false;
        }

        public static void AddPlayer(RemotePlayer player)
        {
            lock (Players)
            {
                Players.Add(player.ID, player);
            }
            LobbyServer.BroadcastPlayerCount();

        }

        /// <summary>
        /// Handles all logic when player disconnects
        /// </summary>
        /// <param name="netId"></param>
        public static void DisconnectPlayer(ShortGuid netId)
        {
            bool removed = false;
            lock (Players)
            {
                if (Players.ContainsKey(netId))
                {
                    removed = true;
                    var player = Players[netId];
                    //player.ClearAllInvites();
                    LobbyServer.LeaveLobby(player);
                    if (player.IsInGame && player.PlayerState == PlayerState.IN_GAME)
                    {
                        var gamePlayer = player.Game.GetPlayer(player);
                        GameServer.EndGame(gamePlayer.Opponent, gamePlayer, GameResult.PLAYER_DISCONNECTED);
                        }
                    try
                    {
                        Players[netId].Connection.CloseConnection(false);
                    }
                    catch { }
                    Players.Remove(netId);

                }
            }
            if(removed)
            {
                LobbyServer.BroadcastPlayerCount();
            }
            
        }

    }
}
