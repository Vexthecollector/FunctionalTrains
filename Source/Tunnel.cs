using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class Tunnel : IExposable
    {

        public Map startMap;
        public Map endMap;
        public int distance;
        List<Rail> rails;
        TunnelType tunnelType;
        int totalWorkRequired;
        int workRequired;
        bool finished;
        bool useable;

        public Tunnel(Map startMap, Map endMap, TunnelType tunnelType)
        {
            this.startMap = startMap;
            this.endMap = endMap;
            distance = Find.WorldGrid.TraversalDistanceBetween(startMap.Tile, endMap.Tile);
            this.tunnelType = tunnelType;
            totalWorkRequired = workRequired = tunnelType.workRequired() * distance;
        }



        public bool isUseable() { return useable; }
        public bool isFinished() { return finished; }

        public void workOnTunnel(int workdone)
        {
            workRequired -= workdone;
            if (workRequired < 1) { finished = true; useable = true; }
        }

        public void instantFinishWork() { finished = true; workRequired = 0; useable = true; }

        public int percentDone() { return (totalWorkRequired - workRequired / totalWorkRequired) * 100; }

        public void ExposeData()
        {
            Scribe_References.Look(ref startMap, "tunnelStartMap");
            Scribe_References.Look(ref endMap, "tunnelStartMap");
            Scribe_Deep.Look(ref tunnelType, "tunnelTunnelType");
            Scribe_Values.Look(ref totalWorkRequired, "tunnelTotalWorkRequired");
            Scribe_Values.Look(ref workRequired, "tunnelworkRequired");
            Scribe_Values.Look(ref finished, "tunnelfinished");
            Scribe_Values.Look(ref useable, "tunneluseable");
        }
    }
}
