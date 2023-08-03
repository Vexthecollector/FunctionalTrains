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
        List<Rail> rails = new List<Rail>();
        TunnelType tunnelType;
        float totalTunnelWorkRequired;
        float tunnelWorkRequired;
        bool finished;
        bool useable;

        public Tunnel() { }
        public Tunnel(Map startMap, Map endMap, TunnelType tunnelType)
        {
            this.startMap = startMap;
            this.endMap = endMap;
            this.tunnelType = tunnelType;
            totalTunnelWorkRequired = tunnelWorkRequired = tunnelType.WorkRequired() * GetDistance();
        }

        public bool IsFinished() { return finished; }

        public void WorkOnTunnel(float workdone)
        {
            tunnelWorkRequired -= workdone;
            if (tunnelWorkRequired < 1) { finished = true; useable = true; }
        }

        public bool IsUseable()
        {
            if((rails?.Any(rail=>rail.IsUseable()) ?? false)&& useable) return true;
            return false;
        }

        public int GetDistance()
        {
            return Find.WorldGrid.TraversalDistanceBetween(startMap.Tile, endMap.Tile);
        }
        public void InstantFinishWork() { finished = true; tunnelWorkRequired = 0; useable = true; }

        public int PercentDone() { return (int)(((totalTunnelWorkRequired-tunnelWorkRequired) / totalTunnelWorkRequired) * 100); }

        public List<Rail> Rails() {  return rails; }
        public TunnelType TunnelType() { return tunnelType; }

        public int MaxRails() { return tunnelType.RailsAvailable(); }
        public int CurrentRails() { return rails.Count(); }
        public float TunnelWorkRequired () { return tunnelWorkRequired; }
        public float TotalTunnelWorkRequired() { return totalTunnelWorkRequired; }


        public void ExposeData()
        {
            Scribe_References.Look(ref startMap, "tunnelStartMap");
            Scribe_References.Look(ref endMap, "tunnelEndMap");
            Scribe_Collections.Look(ref rails, "tunnelRails", LookMode.Deep);
            if (rails == null) rails = new List<Rail>();
            Scribe_Deep.Look(ref tunnelType, "tunnelTunnelType");
            Scribe_Values.Look(ref totalTunnelWorkRequired, "tunnelTotalWorkRequired");
            Scribe_Values.Look(ref tunnelWorkRequired, "tunnelworkRequired");
            Scribe_Values.Look(ref finished, "tunnelfinished");
            Scribe_Values.Look(ref useable, "tunneluseable");
        }
    }
}
