using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class TunnelType :IExposable
    {
        int type;

        public TunnelType(int type)
        {
            this.type = type;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref type, "tunnelTypeType");
        }

        public int workRequired()
        {
            if (type == 1) return 500;
            else if (type == 2) return 1000;
            else if (type == 3) return 2000;
            else if (type == 4) return 5000;
            else if (type == 5) return 10000;
            else if (type == 8) return 20000;
            return -1;
        }
    }
}
