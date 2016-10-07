using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Structures.Remote
{

    public abstract class GameRuleSetBase
    {
        public virtual bool ContinueTurnUntilMiss { get { return true; } }
        public virtual int BoardSize { get { return 10; } }

        public virtual IEnumerable<ShipInfo> GetFleet()
        {
            return new ShipInfo[]
            {
                new ShipInfo("Destroyer", 1, 5),
                new ShipInfo("4", 1, 4),
                new ShipInfo("3", 2, 3),
                new ShipInfo("2", 2, 2),
                new ShipInfo("1", 2, 1)

            };
        }
    }
}
