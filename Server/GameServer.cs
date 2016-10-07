using Common;
using Common.Packets.C2S.Game;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using Common.Structures.Remote;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public static class GameServer
    {
        public static List<Game> Games = new List<Game>(); 


        public static Game InitNewGame(RemotePlayer BlueSide, RemotePlayer RedSide)
        {
            var game = new Game(BlueSide, RedSide, new StandardGameRuleSet());
            game.OnGameEnded += EndGame;
            BlueSide.JoinGame(game);
            RedSide.JoinGame(game);
            lock(Games)
            {
                Games.Add(game);
            }
            return game;
        }

        public static void EndGame(Game.Player Winner, Game.Player Loser, GameResult result = GameResult.ALL_SHIPS_SUNK)
        {
            var game = Winner.Game;
            Winner.RemotePlayer.EndGame(BoardOwner.ME, result);
            if (result != GameResult.PLAYER_DISCONNECTED)
            {
                Loser.RemotePlayer.EndGame(BoardOwner.ENEMY, result);
            }
            lock(Games)
            {
                Games.Remove(game);
            }
            game = null;
            GC.Collect(); //just in case
        }

    }
}
