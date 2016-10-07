using Common.Structures.Remote;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Structures.Common;
using Common.Packets.S2C.Lobby;
using Common;

namespace Server
{
    public static class LobbyServer
    {
        public static Dictionary<ShortGuid, RemotePlayer> Players = new Dictionary<ShortGuid, RemotePlayer>();
        private static object _enterLock = new object();
        public static void EnterLobby(RemotePlayer player)
        {
            lock (_enterLock)
            {
                bool added = false;
                List<RemotePlayer> players;
                lock (Players)
                {
                    players = Players.Values.ToList();
                    if (!Players.ContainsKey(player.ID))
                    {
                        added = true;
                        Players.Add(player.ID, player);
                    }
                }
                if (added)
                {
                    player.PlayerState = PlayerState.IN_LOBBY;

                    var packet = new S2C_LobbyPlayerJoined(player.GetDisplay());
                    foreach (var p in players)
                    {
                        p.Connection.Send(packet);
                    }
                    SendInitialLobbyData(player);
                }
            }
        }

        public static void LeaveLobby(RemotePlayer player)
        {
            lock (_enterLock)
            {
                bool removed = false;
                lock (Players)
                {
                    removed = Players.Remove(player.ID);
                    //if (!removed)
                    //    return;
                }
                if (removed)
                {
                    player.ClearAllInvites();
                }
                var packet = new S2C_LobbyPlayerLeft(player.GetDisplay());
                var players = GetPlayerList();
                foreach (var p in players)
                {
                    p.Connection.Send(packet);
                }
            }

        }

        public static void SendInitialLobbyData(RemotePlayer p)
        {
            var players = GetPlayerList().Where(t => t.ID != p.ID);
            S2C_InitialLobbyData data = new S2C_InitialLobbyData();
            data.AvailablePlayers = players.Select(t=>t.GetDisplay()).ToArray();
            data.PlayersOnline = ProtoServer.GetPlayerCount();
            p.Connection.Send(data);
        }

        public static IEnumerable<RemotePlayer> GetPlayerList()
        {
            List<RemotePlayer> list;
            lock(Players)
            {
                list = Players.Values.ToList();
            }
            return list;
        }

        public static void BroadcastPlayerCount()
        {
            var players = GetPlayerList();
            var packet = new S2C_ServerPlayersOnlineCount(ProtoServer.GetPlayerCount());
            foreach(var p in players)
            {
                p.Connection.Send(packet);
            }
        }
    }
}
