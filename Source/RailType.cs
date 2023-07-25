using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class RailType : IExposable
    {
        int type;

        public RailType() { }
        public RailType(int type)
        {
            this.type = type;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref type, "railTypeType");
        }

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

        public int Type() { return type; }
        public string RailName()
        {
            if (type == 1) return "Neolithic Rail";
            else if (type == 2) return "Medieval Rail";
            else if (type == 3) return "Industrial Rail";
            else if (type == 4) return "Spacer Rail";
            else if (type == 5) return "Ultratech Rail";
            else if (type == 6) return "Archotech Rail";
            return null;
        }
    }
}
