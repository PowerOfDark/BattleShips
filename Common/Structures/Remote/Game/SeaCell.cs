using Common.Structures.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{
    public class SeaCell : SimpleSeaCell
    {
        public delegate void OnCellStateChangedHandler(SeaCell cell);
        public event OnCellStateChangedHandler OnCellStateChanged;

        //public int X { get; protected set; }
        //public int Y { get; protected set; }
        //public SeaCellState CellState { get; protected set; }
        //public Board Board { get; protected set; }
        public Ship Ship { get; protected set; }
        public bool WasHit { get { return this.CellState != SeaCellState.SEA && this.CellState != SeaCellState.SHIP; } }

        public SeaCell(int x, int y, SeaCellState state = SeaCellState.SEA)
        {
            this.X = x;
            this.Y = y;
            this.CellState = state;
        }

        public void BindShip(Ship ship)
        {
            this.CellState = SeaCellState.SHIP;
            this.Ship = ship;
        }

        //public void BindBoard(Board board)
        //{
        //    this.Board = board;
        //}

        public FireResult OnHit()
        {
            if (this.WasHit)
                return FireResult.ALREADY_FIRED;
            if(this.Ship != null)
            {
                return Ship.OnHit(this);
            }
            else
            {
                if(this.CellState == SeaCellState.SEA)
                {
                    SetState(SeaCellState.FIRE_MISSED);
                    return FireResult.MISSED;
                }
            }
            return FireResult.MISSED;
        }

        public void SetState(SeaCellState state)
        {
            this.CellState = state;
            OnCellStateChanged?.Invoke(this);
        }

        public SimpleSeaCell GetSimplifiedCell()
        {
            return new SimpleSeaCell(this.X, this.Y, this.CellState);
        }
    }
}
