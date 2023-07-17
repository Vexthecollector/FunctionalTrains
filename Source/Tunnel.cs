using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class Tunnel
    {

        public readonly Map startMap;
        public readonly Map endMap;
        public readonly int distance;
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
    }
}
