using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Structures.Remote
{
    public class SeededRandomBoardGenerator
    {
        public enum CellTemplateState
        {
            EMPTY, ZONE, SHIP
        }
        public enum Direction
        {
            UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
        }
        
        public class CellTemplate
        {
            public int ShipID { get; set; }
            public CellTemplateState State { get; set; }
            public int X { get; protected set; }
            public int Y { get; protected set; }

            public CellTemplate(int id, CellTemplateState state, Point pos)
            {
                this.ShipID = id;
                this.State = state;
                this.X = (int)pos.X;
                this.Y = (int)pos.Y;
            }

            public bool IsNotColliding(int shipId)
            {
                if (this.State == CellTemplateState.SHIP && this.ShipID == shipId)
                    return true;
                //if (this.State == CellTemplateState.ZONE && this.ShipID == shipId)
                    return true;
                if (this.State == CellTemplateState.EMPTY)
                    return true;
                return false;
            }
        }
        public class BoardTemplate
        {
            public CellTemplate[,] Cells { get; protected set; }
            public int BoardSize { get; protected set; }
            public BoardTemplate(int BoardSize)
            {
                this.Cells = new CellTemplate[BoardSize, BoardSize];
                for(int x = 0; x < BoardSize; x++)
                {
                    for(int y = 0; y < BoardSize; y++)
                    {
                        this.Cells[x, y] = new CellTemplate(-1, CellTemplateState.EMPTY, new Point(x, y));
                    }
                }
                this.BoardSize = BoardSize;
            }

            public IEnumerable<CellTemplate> GetZone(int x, int y, int r = 1)
            {
                List<CellTemplate> zone = new List<CellTemplate>();
                for(int xx = x-r; xx <= x+r; xx++)
                {
                    for(int yy = y-r; yy <= y+r; yy++)
                    {
                        if (yy == y && xx == x)
                            continue;
                        if (xx >= 0 && xx < BoardSize && yy >= 0 && yy < BoardSize)
                        {
                            zone.Add(this.Cells[xx, yy]);
                        }
                    }
                }
                return zone;
            }
            public CellTemplate GetAt(int x, int y, Direction dir)
            {
                if (dir == Direction.UP)
                    y--;
                if (dir == Direction.DOWN)
                    y++;
                if (dir == Direction.LEFT)
                    x--;
                if (dir == Direction.RIGHT)
                    x++;
                if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize)
                    return this.Cells[x, y];
                return null;
            }
            public IEnumerable<CellTemplate> GetSides(int x, int y)
            {
                List<CellTemplate> tmp = new List<CellTemplate>();
                tmp.Add(GetAt(x, y, Direction.UP));
                tmp.Add(GetAt(x, y, Direction.DOWN));
                tmp.Add(GetAt(x, y, Direction.LEFT));
                tmp.Add(GetAt(x, y, Direction.RIGHT));
                return tmp.Where(t => t != null);
            }

            public void PlaceCore(int x, int y, int id)
            {
                this.Cells[x, y].ShipID = id;
                this.Cells[x, y].State = CellTemplateState.SHIP;
                foreach(var cell in GetZone(x, y))
                {
                    if (cell.State == CellTemplateState.ZONE)
                        continue;
                    if (cell.State != CellTemplateState.SHIP)
                    {
                        cell.ShipID = id;
                        cell.State = CellTemplateState.ZONE;
                        
                    }
                }
            }

            

        }

        private Random _random;
        private Game _game;

        private Direction randomDir()
        {
            return (Direction)_random.Next(0, 4);
        }

        private void randomxy(out int x, out int y)
        {
            x = _random.Next(0, _game.RuleSet.BoardSize);
            y = _random.Next(0, _game.RuleSet.BoardSize);
        }

        public Board GetRandomBoard(Game game)
        {
            _game = game;
            
            var template = new BoardTemplate(game.RuleSet.BoardSize);
            var fleet = game.RuleSet.GetFleet().OrderByDescending(t=>t.Size);
            List<List<Point>> mapped = new List<List<Point>>();
            int shipId = 0;
            int x, y;
            List<Point> fallback;
            int fallbackIndex;
            foreach(var ship in fleet)
            {
                for (int i = 0; i < ship.Count; i++)
                {
                    List<Point> placed = new List<Point>();
                    IEnumerable<CellTemplate> tmp;
                    int toPlace = ship.Size;
                    int it = 0;
                    do
                    {
                        randomxy(out x, out y);
                        if (++it > 1000000)
                            return GetRandomBoard(game);
                    } while (template.Cells[x, y].State != CellTemplateState.EMPTY || template.GetZone(x, y).Count(t => t.State == CellTemplateState.EMPTY) < toPlace);

                    template.PlaceCore(x, y, shipId);
                    placed.Add(new Point(x, y));
                    toPlace--;

                    while(toPlace > 0)
                    {
                        fallback = new List<Point>(placed);
                        do
                        {
                            fallbackIndex = _random.Next(0, fallback.Count);
                            Point item = fallback[fallbackIndex];
                            fallback.RemoveAt(fallbackIndex);
                            tmp = template.GetSides((int)item.X, (int)item.Y).Where(t => t.State == CellTemplateState.EMPTY || (t.State == CellTemplateState.ZONE && t.ShipID == shipId));//.Where(t => template.GetZone(t.X, t.Y).All(t2 => t2.IsNotColliding(shipId)));
                        }
                        while ((tmp == null || tmp.Count() == 0) && fallback.Count > 0);
                        if(tmp.Count() == 0)
                            throw new Exception();
                        CellTemplate target = tmp.ElementAt(_random.Next(0, tmp.Count()));
                        template.PlaceCore(target.X, target.Y, shipId);
                        placed.Add(new Point(target.X, target.Y));

                            
                        toPlace--;
                    }
                    mapped.Add(placed);
                    shipId++;
                }
            }

            Board board = new Board(game.RuleSet.BoardSize);
            for(int i = 0; i < mapped.Count; i++)
            {
                var ship = new Ship(mapped[i].Count);
                foreach(var pt in mapped[i])
                {
                    ship.AddCell(new SeaCell((int)pt.X, (int)pt.Y));
                }
                board.AddShip(ship);
            }
            //var ship1 = new Ship(5);
            //ship1.AddCells(new SeaCell[] { new SeaCell(0, 0), new SeaCell(0, 1), new SeaCell(0, 2), new SeaCell(0, 3), new SeaCell(0, 4) });
            //board.AddShip(ship1);
            //var ship2 = new Ship(4);
            //ship2.AddCells(new SeaCell[] { new SeaCell(2, 0), new SeaCell(2, 1), new SeaCell(2, 2), new SeaCell(2, 3) });
            //board.AddShip(ship2);
            //var ship3 = new Ship(3);
            //ship3.AddCells(new SeaCell[] { new SeaCell(4, 0), new SeaCell(4, 1), new SeaCell(4, 2) });
            //board.AddShip(ship3);
            //var ship4 = new Ship(2);
            //ship4.AddCells(new SeaCell[] { new SeaCell(6, 0), new SeaCell(6, 1) });
            //board.AddShip(ship4);
            //var ship5 = new Ship(1);
            //ship5.AddCells(new SeaCell[] { new SeaCell(8, 0) });
            //board.AddShip(ship5);

            return board;
        }

        public SeededRandomBoardGenerator()
        {
            _random = new Random();
        }
    }
}
