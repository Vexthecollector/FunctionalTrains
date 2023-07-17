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
        // Token: 0x06009B7A RID: 39802 RVA: 0x003854D0 File Offset: 0x003836D0
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Thing thing2 = map.thingGrid.ThingAt(loc, TrainStationDefOf.FT_TrainStation);
            if (thing2 == null || thing2.Position != loc)
            {
                return "MustPlaceOnTrainStation".Translate();
            }
            return true;
        }

        // Token: 0x06009B7B RID: 39803 RVA: 0x00385517 File Offset: 0x00383717
        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == TrainStationDefOf.FT_TrainStation;
        }
    }

}
