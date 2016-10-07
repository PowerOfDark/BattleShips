using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{
    public class Ship
    {
        public delegate void OnShipSunkHandler(Ship ship);
        public event OnShipSunkHandler OnShipSunk;
        public event SeaCell.OnCellStateChangedHandler OnCellStateChanged;

        public Board Board { get; protected set; }
        public List<SeaCell> Cells { get; protected set; }
        public int Size { get; protected set; }
        public int Health { get { lock (Cells) { return Cells.Count(t => t.CellState == SeaCellState.SHIP); } } }

        public Ship(int size)
        {
            this.Size = size;
            this.Cells = new List<SeaCell>();
        }

        public void BindBoard(Board board)
        {
            this.Board = board;
        }

        public void AddCell(SeaCell cell)
        {
            AddCells(new SeaCell[] { cell });
        }

        public void AddCells(IEnumerable<SeaCell> cells)
        {
            lock (Cells)
            {
                foreach (var cell in cells)
                {
                    if (Cells.Count < Size)
                    {
                        Cells.Add(cell);
                        cell.BindShip(this);
                        cell.OnCellStateChanged += Cell_OnCellStateChanged;
                    }
                    else
                    {
                        throw new InvalidOperationException("Ship already has enough cells.");
                    }
                }
            }
        }

        private void Cell_OnCellStateChanged(SeaCell cell)
        {
            this.OnCellStateChanged?.Invoke(cell);
        }

        public FireResult OnHit(SeaCell cell)
        {
            if(cell.CellState == SeaCellState.SHIP)
            {
                if(this.Health == 1)
                {
                    this.Sink();
                    return FireResult.SHIP_SUNK;
                }
                else
                {
                    cell.SetState(SeaCellState.SHIP_HIT);
                    return FireResult.SHIP_HIT;
                }
            }
            return FireResult.ALREADY_FIRED;
        }

        public void Sink()
        {
            List<SeaCell> cells;
            lock(Cells)
            {
                cells = Cells.ToList();
            }
            foreach(var cell in cells)
            {
                cell.SetState(SeaCellState.SHIP_SUNK);
            }
            OnShipSunk?.Invoke(this);
        }
    }
}
