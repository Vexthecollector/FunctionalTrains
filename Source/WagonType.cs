using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class WagonType : IExposable
    {
        public void ExposeData()
        {
            throw new NotImplementedException();
        }

        int type;
        public static readonly int maxTypes = 1;

        public WagonType()
        {

        }
        public WagonType(int type)
        {
            this.type = type;
        }

        public int GetType() { return type; }
    }
}
