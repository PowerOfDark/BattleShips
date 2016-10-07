using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Common.Structures.Remote.Game;

namespace Common.Structures.Remote
{
    public class Board
    {
        public event Ship.OnShipSunkHandler OnShipSunk;
        public delegate void OnBoardCellChangedHandler(Board board, SeaCell cell);
        //public delegate void OnShipCellChangedHandler(Ship ship, SeaCell cell);
        public event OnBoardCellChangedHandler OnBoardCellStateChanged;
        //public event OnShipCellChangedHandler OnShipCellStateChanged;

        public Player Player { get; protected set; }

        public int BoardSize { get; protected set; }
        public List<Ship> Ships { get; protected set; }
        public bool IsDestroyed
        {
            get
            {
                bool won = true;
                lock (Ships)
                {
                    foreach (var ship in Ships)
                    {
                        if (ship.Health > 0)
                        {
                            won = false;
                            break;
                        }
                    }
                }
                return won;
            }
        }

        protected SeaCell[,] Cells { get; set; }
        public SeaCell this[int x, int y]
        {
            get { return Cells[x, y]; }
            set
            {
                lock (this.Cells.SyncRoot)
                {
                    this.Cells[x, y] = value;
                    value.OnCellStateChanged += Board_OnCellStateChanged;
                }
            }
        }

        public Board(int boardSize)
        {
            this.BoardSize = boardSize;
            this.Ships = new List<Ship>();
            this.Cells = new SeaCell[boardSize, boardSize];
            SeaCell tmp;
            for(int x = 0; x < boardSize; x++)
            {
                for(int y = 0; y < boardSize; y++)
                {
                    tmp = new SeaCell(x, y);
                    tmp.OnCellStateChanged += Board_OnCellStateChanged;
                    this.Cells[x, y] = tmp;
                }
            }
        }

        private void Board_OnCellStateChanged(SeaCell cell)
        {
            this.OnBoardCellStateChanged?.Invoke(this, cell);
        }

        public void AddShip(Ship ship)
        {
            List<SeaCell> cells;
            lock (ship.Cells)
            {
                cells = ship.Cells.ToList();
            }
            lock (Cells.SyncRoot)
            {
                foreach (var cell in cells)
                {
                    cell.OnCellStateChanged += Board_OnCellStateChanged;
                    this.Cells[cell.X, cell.Y] = cell;
                }
            }
            lock (Ships)
            {
                ship.OnShipSunk += Ship_OnShipSunk;
                //ship.OnCellStateChanged += Ship_OnCellStateChanged;
                ship.BindBoard(this);
                this.Ships.Add(ship);
            }
        }

        public void BindPlayer(Player player)
        {
            this.Player = player;
        }

        //private void Ship_OnCellStateChanged(SeaCell cell)
        //{
        //    OnShipCellStateChanged?.Invoke(cell.Ship, cell);
        //}

        private void Ship_OnShipSunk(Ship ship)
        {
            OnShipSunk?.Invoke(ship);
        }


        public SeaCellState[,] GetSimplifiedBoard()
        {
            SeaCellState[,] ret = new SeaCellState[this.BoardSize, this.BoardSize];
            lock(this.Cells)
            {
                for(int x = 0; x < this.BoardSize; x++)
                {
                    for (int y = 0; y < this.BoardSize; y++)
                    {
                        ret[x, y] = this.Cells[x, y].CellState;
                    }
                }
            }
            return ret;
        }

        public FireResult FireAt(int x, int y)
        {
            return this[x, y].OnHit();
        }

    }
}
