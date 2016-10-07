using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{
    public class ShipInfo
    {
        public string Name { get; protected set; }
        public int Count { get; protected set; }
        public int Size { get; protected set; }

        public ShipInfo(string name, int count, int size)
        {
            this.Name = name;
            this.Count = count;
            this.Size = size;
        }
    }
}
