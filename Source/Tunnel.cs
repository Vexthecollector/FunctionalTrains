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
        List<Rail> rails = new List<Rail>();
        TunnelType tunnelType;
        int totalTunnelWorkRequired;
        int tunnelWorkRequired;
        bool finished;
        bool useable;

        public Tunnel() { }
        public Tunnel(Map startMap, Map endMap, TunnelType tunnelType)
        {
            this.startMap = startMap;
            this.endMap = endMap;
            distance = Find.WorldGrid.TraversalDistanceBetween(startMap.Tile, endMap.Tile);
            this.tunnelType = tunnelType;
            totalTunnelWorkRequired = tunnelWorkRequired = tunnelType.WorkRequired() * distance;
        }



        public bool IsFinished() { return finished; }

        public void WorkOnTunnel(int workdone)
        {
            tunnelWorkRequired -= workdone;
            if (tunnelWorkRequired < 1) { finished = true; useable = true; }
        }

        public bool IsUseable()
        {
            if(/*rails.Any(rail=>rail.useable) && */useable) return true;
            return false;
        }
        public void InstantFinishWork() { finished = true; tunnelWorkRequired = 0; useable = true; }

        public int PercentDone() { return (totalTunnelWorkRequired - tunnelWorkRequired / totalTunnelWorkRequired) * 100; }

        public List<Rail> Rails() {  return rails; }
        public TunnelType TunnelType() { return tunnelType; }


        public void ExposeData()
        {
            Scribe_References.Look(ref startMap, "tunnelStartMap");
            Scribe_References.Look(ref endMap, "tunnelEndMap");
            Scribe_Deep.Look(ref tunnelType, "tunnelTunnelType");
            Scribe_Values.Look(ref totalTunnelWorkRequired, "tunnelTotalWorkRequired");
            Scribe_Values.Look(ref tunnelWorkRequired, "tunnelworkRequired");
            Scribe_Values.Look(ref finished, "tunnelfinished");
            Scribe_Values.Look(ref useable, "tunneluseable");
        }
    }
}
