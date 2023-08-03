using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class PlaceWorker_OnTrainStation : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Thing thing2 = map.thingGrid.ThingAt(loc, FunctionalTrainsDefOf.FT_TrainStation);
            if (thing2 == null || thing2.Position != loc)
            {
                return "MustPlaceOnTrainStation".Translate();
            }
            return true;
        }

        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == FunctionalTrainsDefOf.FT_TrainStation;
        }
    }

}
