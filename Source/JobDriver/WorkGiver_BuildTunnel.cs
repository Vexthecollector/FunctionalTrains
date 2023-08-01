using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace FunctionalTrains
{
    internal class WorkGiver_BuildTunnel : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            if (pawn.Spawned && pawn.Map != null)
            {
                List<Comp_TrainStation> stationlist = WorldComponent_StationList.Instance.Stations.Where(station => station.Map == pawn.Map).ToList();
                foreach (Comp_TrainStation station in stationlist)
                {
                    if (station.currentTunnel != null && !station.currentTunnel.IsFinished())
                    {

                        yield return station.parent;
                    }
                }
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            Building building = t as Building;
            return building != null && !building.IsForbidden(pawn) && pawn.CanReserve(building, 5, 0, null, forced) && building.TryGetComp<Comp_TrainStation>().CanWorkNow() && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null && !building.IsBurning();
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(FunctionalTrainsDefOf.FT_JobBuildTunnel, t, 1500, true);
        }
    }
}
