using Battleships.Interfaces.UI;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using Common.Structures.Local;
using Common.Structures.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl, IEmbeddedViewShowAnimationEnd, IEmbeddedViewShowAnimationStart, IEmbeddedViewHideAnimationStart
    {
        public PlayerDisplay Opponent { get; protected set; }
        public int BoardSize { get; protected set; }
        public bool ContinueTurnUntilMiss { get; protected set; }
        public IEnumerable<ShipInfo> Fleet { get; protected set; }
        public PlayerSide Side { get; protected set; }

        public LocalGameState GameState { get; protected set; }

        private Color _myColor;
        private Color _enemyColor;

        private Label _turnAnimationLabel;
        private ColorAnimation _turnAnimation;
        private BoardOwner? _turnAnimationTarget;

        public GameView()
        {
            InitializeComponent();
        }

        public GameView(S2C_InitNewGame data) : this()
        {
            this.Opponent = data.Opponent;
            this.BoardSize = data.BoardSize;
            this.ContinueTurnUntilMiss = data.ContinueTurnUntilMiss;
            this.Fleet = data.Fleet;
            this.Side = data.Side;
            this.GameState = LocalGameState.INITIALIZING;
            this.labelMyNickname.Content = ProtoClient.LocalPlayer.Nickname;
            this.labelEnemyNickname.Content = data.Opponent.Nickname;

            this._myColor = data.Side == PlayerSide.BLUE ? Colors.Blue : Colors.Red;
            this._enemyColor = data.Side == PlayerSide.BLUE ? Colors.Red : Colors.Blue;

            this.labelMyNickname.BorderBrush = new SolidColorBrush(this._myColor);
            this.labelEnemyNickname.BorderBrush = new SolidColorBrush(this._enemyColor);
        }

        public void OnShowAnimationStart()
        {
            ProtoClient.OnGameStarted += ProtoClient_OnGameStarted;
            ProtoClient.OnGameTurnInfo += ProtoClient_OnGameTurnInfo;
            ProtoClient.OnGameBoardUpdated += ProtoClient_OnGameBoardUpdated;
            ProtoClient.OnGameEnded += ProtoClient_OnGameEnded;
            this.enemyBoard.OnCellMouseEnter += EnemyBoard_OnCellMouseEnter;
            this.enemyBoard.OnCellMouseLeave += EnemyBoard_OnCellMouseLeave;
            this.enemyBoard.OnCellMouseUp += EnemyBoard_OnCellMouseUp;
        }
        public void OnShowAnimationEnd()
        {
            ProtoClient.GameReady();
            //myBoard[0, 0].AnimateState(Common.Structures.Remote.SeaCellState.SHIP_HIT);
        }
        public void OnHideAnimationStart()
        {
            ProtoClient.OnGameStarted -= ProtoClient_OnGameStarted;
            ProtoClient.OnGameTurnInfo -= ProtoClient_OnGameTurnInfo;
            ProtoClient.OnGameBoardUpdated -= ProtoClient_OnGameBoardUpdated;
            ProtoClient.OnGameEnded -= ProtoClient_OnGameEnded;
        }

        private void ProtoClient_OnGameStarted(Common.Packets.S2C.Game.S2C_GameStarted data)
        {
            this.Invoke((d) =>
            {
                SeaCellState[,] rect = d.YourBoard.ToRectangularArray();
                int sz = this.BoardSize;
                for (int x = 0; x < sz; x++)
                {
                    for (int y = 0; y < sz; y++)
                    {
                        myBoard[x, y].AnimateState(rect[x, y]);
                    }
                }
            }, data);
        }
        private void ProtoClient_OnGameTurnInfo(Common.Packets.S2C.Game.S2C_GameTurnInfo data)
        {
            if (data.Player == BoardOwner.ME)
            {
                this.GameState = LocalGameState.MY_TURN;
            }
            else
            {
                this.GameState = LocalGameState.ENEMY_TURN;
            }
            this.Invoke((d) => { AnimateTurn(d.Player); }, data);
            
        }
        private void ProtoClient_OnGameBoardUpdated(Common.Packets.S2C.Game.S2C_GameBoardUpdated data)
        {
            UpdateCells(data.BoardOwner, data.UpdatedCells);
        }
        private void ProtoClient_OnGameEnded(Common.Packets.S2C.Game.S2C_GameEnded data)
        {
            this.Invoke((d) => 
            {
                var outcome = (data.Winner == BoardOwner.ME) ? "WON" : "LOST";
                var str = $"You have {outcome}";
                this.buttonGameResult.Content = str;
                DoubleAnimation animation = new DoubleAnimation(0, 100, new Duration(TimeSpan.FromSeconds(0.5)));
                animation.Completed += (s, e) => 
                {
                    this.buttonGameResult.IsEnabled = true;
                };
                this.buttonGameResult.BeginAnimation(Button.OpacityProperty, animation); 
            }, data);
        }

        private void EnemyBoard_OnCellMouseUp(object sender, SeaBoard.SeaBoardCellEventArgs e)
        {
            if (this.GameState == LocalGameState.MY_TURN && e.Cell.Value == SeaCellState.SEA)
            {
                ProtoClient.FireAt(e.Cell.X, e.Cell.Y);
                this.GameState = LocalGameState.FIRED_WAIT;
            }
        }
        private void EnemyBoard_OnCellMouseLeave(object sender, SeaBoard.SeaBoardCellEventArgs e)
        {
            e.Cell.BorderValue = SeaCellTargetMode.NONE;
        }
        private void EnemyBoard_OnCellMouseEnter(object sender, SeaBoard.SeaBoardCellEventArgs e)
        {
            if(GameState == LocalGameState.MY_TURN)
            {
                if(e.Cell.Value == SeaCellState.SEA && e.Cell.BorderValue == SeaCellTargetMode.NONE)
                {
                    e.Cell.BorderValue = SeaCellTargetMode.FIRE_TARGET;
                }
            }
        }

        public void AnimateTurn(BoardOwner side)
        {
            if (this._turnAnimationTarget.HasValue && this._turnAnimationTarget.Value == side)
                return;
            if (this._turnAnimationTarget.HasValue)
            {
                this._turnAnimation.BeginTime = null;
                this._turnAnimationLabel.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, this._turnAnimation);
                ColorAnimation revert = new ColorAnimation((this._turnAnimationLabel.BorderBrush as SolidColorBrush).Color, this._turnAnimation.From.Value, new Duration(TimeSpan.FromSeconds(0.1)));
                this._turnAnimationLabel.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, revert);

            }
            var sideColor = (side == BoardOwner.ME) ? this._myColor : this._enemyColor;
            ColorAnimation animation = new ColorAnimation(sideColor, Colors.White, new Duration(TimeSpan.FromSeconds(0.75)));
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.AutoReverse = true;
            var label = (side == BoardOwner.ME) ? labelMyNickname : labelEnemyNickname;
            label.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            this._turnAnimation = animation;
            this._turnAnimationLabel = label;
            this._turnAnimationTarget = side;
        }
        public void UpdateCells(BoardOwner board, IEnumerable<SimpleSeaCell> cells)
        {
            this.Invoke((b, c) => 
            {
                var _board = (b == BoardOwner.ME) ? myBoard : enemyBoard;
                foreach(var cell in c)
                {
                    _board[cell.X, cell.Y].AnimateState(cell.CellState);
                }
            }, board, cells);

        }

        private void buttonGameResult_Click(object sender, RoutedEventArgs e)
        {
            //..ayy
            this.GetMainWindow().TransitionInto(new LobbyView());
        }
    }
}
