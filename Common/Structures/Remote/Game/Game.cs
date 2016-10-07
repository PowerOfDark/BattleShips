using Common.Packets.S2C.Game;
using Common.Structures.Common;
using Common.Structures.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{
    public class Game
    {
        public class Player
        {
            public Board Board { get; protected set; }
            public PlayerSide PlayerSide { get; protected set; }
            public RemotePlayer RemotePlayer { get; protected set; }
            public Player Opponent { get; protected set; }
            public Game Game { get; protected set; }
            public bool IsReady { get; set; }

            public Player(PlayerSide side, RemotePlayer remotePlayer, Game game)
            {
                this.PlayerSide = side;
                this.RemotePlayer = remotePlayer;
                this.Game = game;
            }

            public void BindOpponent(Player p)
            {
                this.Opponent = p;
            }

            public void SetBoard(Board board)
            {
                this.Board = board;
                board.BindPlayer(this);
            }

            public FireResult FireAtEnemy(int x, int y)
            {
                return this.Game.OnFire(this, x, y);
            }
        }


        public Player BlueSide { get; protected set; }
        public Player RedSide { get; protected set; }
        public bool BothPlayersReady { get { return BlueSide.IsReady && RedSide.IsReady; } }

        public GameRuleSetBase RuleSet { get; protected set; }

        public GameState GameState { get; protected set; }
        public PlayerSide PlayerTurn { get; protected set; }

        public delegate void OnGameEndedHandler(Player winner, Player loser, GameResult result);
        public event OnGameEndedHandler OnGameEnded;

        private object _syncRoot = new object();


        public RemotePlayer GetOpponent(RemotePlayer player)
        {
            return (player.ID == BlueSide.RemotePlayer.ID) ? RedSide.RemotePlayer : BlueSide.RemotePlayer;
        }

        public Player GetPlayer(PlayerSide side)
        {
            return (side == PlayerSide.BLUE) ? BlueSide : RedSide;
        }
        public Player GetPlayer(RemotePlayer player)
        {
            return (player.ID == BlueSide.RemotePlayer.ID) ? BlueSide : RedSide;
        }

        public Game(RemotePlayer blueSide, RemotePlayer redSide, GameRuleSetBase ruleSet)
        {
            this.BlueSide = new Player(PlayerSide.BLUE, blueSide, this);
            this.RedSide = new Player(PlayerSide.RED, redSide, this);
            this.BlueSide.BindOpponent(RedSide);
            this.RedSide.BindOpponent(BlueSide);
            this.RuleSet = ruleSet;


            Initialize();//?
            this.GameState = GameState.WAITING_FOR_PLAYERS;
        }

        public void Initialize()
        {
            if (this.GameState == GameState.INITIALIZING)
                return;
            lock (_syncRoot)
            {
                this.GameState = GameState.INITIALIZING;
                var s = new SeededRandomBoardGenerator();
                var b1 = s.GetRandomBoard(this);
                var b2 = s.GetRandomBoard(this);
                this.BlueSide.SetBoard(b1);
                this.RedSide.SetBoard(b2);
                this.BlueSide.Board.OnShipSunk += OnShipSunk;
                this.RedSide.Board.OnShipSunk += OnShipSunk;
                this.BlueSide.Board.OnBoardCellStateChanged += OnBoardCellStateChanged;
                this.RedSide.Board.OnBoardCellStateChanged += OnBoardCellStateChanged;
            }
        }

        private FireResult OnFire(Player source, int x, int y)
        {
            if (source.PlayerSide == this.PlayerTurn)
            {
                var res = source.Opponent.Board.FireAt(x, y);
                if (res == FireResult.SHIP_HIT || res == FireResult.SHIP_SUNK)
                {
                    if(res == FireResult.SHIP_SUNK && source.Opponent.Board.IsDestroyed)
                    {
                        //the enemy board has been destroyed!woo
                        HandleGameEnd(source, source.Opponent);
                        return FireResult.SHIP_SUNK;
                    }
                    if (this.RuleSet.ContinueTurnUntilMiss)
                    {
                        this.ContinueTurn();
                        return res;
                    }
                }
                this.SetTurn(source.Opponent.PlayerSide);
                return res;
            }
            return FireResult.ALREADY_FIRED;
        }

        private void HandleGameEnd(Player winner, Player loser)
        {
            this.GameState = GameState.FINISHED;
            this.OnGameEnded?.Invoke(winner, loser, GameResult.ALL_SHIPS_SUNK);
        }

        private void OnBoardCellStateChanged(Board board, SeaCell cell)
        {
            if(cell.CellState == SeaCellState.SHIP_SUNK)
            {
                //There is a seperate event for that (OnShipSunk) to compress it down into one packet.
                return;
            }
            var packet = new S2C_GameBoardUpdated(BoardOwner.ME, new[] { cell.GetSimplifiedCell() });
            board.Player.RemotePlayer.Connection.Send(packet);
            packet.BoardOwner = BoardOwner.ENEMY;
            board.Player.Opponent.RemotePlayer.Connection.Send(packet);
        }

        private void OnShipSunk(Ship ship)
        {
            var packet = new S2C_GameBoardUpdated(BoardOwner.ME, ship.Cells.Select(t => t.GetSimplifiedCell()));
            ship.Board.Player.RemotePlayer.Connection.Send(packet);

            packet.BoardOwner = BoardOwner.ENEMY;
            ship.Board.Player.Opponent.RemotePlayer.Connection.Send(packet);
        }





        public void OnPlayerReady(Player p)
        {
            lock (_syncRoot)
            {
                if (!p.IsReady)
                {
                    p.IsReady = true;
                    p.RemotePlayer.Connection.Send(new S2C_GameStarted(new SimpleBoard(p.Board.GetSimplifiedBoard())));
                }
                if (this.BothPlayersReady)
                {
                    //Let's start the game!
                    this.GameState = GameState.IN_PROGRESS;
                    this.SetTurn((new Random().Next(0, 2) == 0) ? PlayerSide.BLUE : PlayerSide.RED);

                }
            }
        }

        public void SetTurn(PlayerSide side)
        {
            this.PlayerTurn = side;
            var packet = new S2C_GameTurnInfo(BoardOwner.ME, false);
            var packetEnemy = new S2C_GameTurnInfo(BoardOwner.ENEMY, false);
            if (side == PlayerSide.BLUE)
            {
                //
                BlueSide.RemotePlayer.Connection.Send(packet);
                RedSide.RemotePlayer.Connection.Send(packetEnemy);
            }
            else
            {
                RedSide.RemotePlayer.Connection.Send(packet);
                BlueSide.RemotePlayer.Connection.Send(packetEnemy);
            }
        }
        public void ContinueTurn()
        {
            var packet = new S2C_GameTurnInfo(BoardOwner.ME, false);
            var packetEnemy = new S2C_GameTurnInfo(BoardOwner.ENEMY, false);
            if (this.PlayerTurn == PlayerSide.BLUE)
            {
                BlueSide.RemotePlayer.Connection.Send(packet);
                RedSide.RemotePlayer.Connection.Send(packetEnemy);
            }
            else
            {
                RedSide.RemotePlayer.Connection.Send(packet);
                BlueSide.RemotePlayer.Connection.Send(packetEnemy);
            }
        }
    }
}
