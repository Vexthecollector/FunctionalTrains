using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class CompProperties_LaunchableTrain : CompProperties
    {
        public CompProperties_LaunchableTrain()
        {
            this.compClass = typeof(Comp_LaunchableTrain);
        }
        public bool requireFuel = true;
    }
}
