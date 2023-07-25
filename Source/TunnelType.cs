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

        public TunnelType() { }
        public TunnelType(int type)
        {
            this.type = type;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref type, "tunnelTypeType");
        }

        public int Type() { return type; }

        public int WorkRequired()
        {
            if (type == 1) return 500;
            else if (type == 2) return 1000;
            else if (type == 3) return 2000;
            else if (type == 4) return 5000;
            else if (type == 5) return 10000;
            else if (type == 6) return 20000;
            return -1;
        }

        public int RailsAvailable()
        {
            if (type == 1) return 1;
            else if (type == 2) return 2;
            else if (type == 3) return 3;
            else if (type == 4) return 4;
            else if (type == 5) return 5;
            else if (type == 6) return 6;
            return -1;
        }

        public string TunnelName()
        {
            if (type == 1) return "Neolithic Tunnel";
            else if (type == 2) return "Medieval Tunnel";
            else if (type == 3) return "Industrial Tunnel";
            else if (type == 4) return "Spacer Tunnel";
            else if (type == 5) return "Ultratech Tunnel";
            else if (type == 6) return "Archotech Tunnel";
            return null;
        }

    }
}
