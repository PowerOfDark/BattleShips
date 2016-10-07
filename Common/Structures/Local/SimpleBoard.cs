using Common.Structures.Remote;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Local
{
    [ProtoContract]
    public class SimpleBoard
    {
        [ProtoMember(1)]
        public IEnumerable<SeaCellState> Board { get; protected set; }
        [ProtoMember(2)]
        public int BoardSize { get; protected set; }

        public SimpleBoard(SeaCellState[,] board)
        {
            if ((this.BoardSize = board.GetLength(0)) != board.GetLength(1))
                throw new InvalidCastException();
            var tmp =  new SeaCellState[BoardSize * BoardSize];
            int i = 0;
            for(int x = 0; x < this.BoardSize; x++)
            {
                for(int y = 0; y < this.BoardSize; y++, i++)
                {
                    tmp[i] = board[x, y];
                }
            }
            this.Board = tmp;
        }

        public SeaCellState[,] ToRectangularArray()
        {
            SeaCellState[,] tmp = new SeaCellState[this.BoardSize, this.BoardSize];
            int i = 0;
            for(int x = 0; x < this.BoardSize; x++)
            {
                for (int y = 0; y < this.BoardSize; y++, i++)
                {
                    tmp[x, y] = this.Board.ElementAt(i);
                }
            }
            return tmp;
        }

        protected SimpleBoard() { }
    }
}
