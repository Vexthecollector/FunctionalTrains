using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class CompProperties_TrainStation : CompProperties
    {
        public CompProperties_TrainStation()
        {
            compClass = typeof(Comp_TrainStation);
        }
        public int tunnelType;
    }
}
