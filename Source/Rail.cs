using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Diagnostics;
using Verse;

namespace FunctionalTrains
{
    public class Rail : IExposable
    {
        bool useable;
        bool finished;
        float totalRailWorkRequired;
        float railWorkRequired;
        public bool inUse;
        RailType railType;
        Tunnel parentTunnel;

        public Rail() { }
        public Rail(Tunnel tunnel,RailType railType)
        {
            this.parentTunnel = tunnel;
            this.railType = railType;
            totalRailWorkRequired = railWorkRequired = railType.WorkRequired() * parentTunnel.GetDistance();
        }

        public RailType RailType() { return railType; }
        public bool IsUseable()
        {
            if (inUse) return false;
            return useable;
        }
        public bool IsFinished()
        {
            return finished;
        }
        public void InstantFinishWork() { finished = true; railWorkRequired = 0; useable = true; }

        public void WorkOnRail(float workdone)
        {
            railWorkRequired -= workdone;
            if (railWorkRequired < 1) { finished = true; useable = true; }
        }

        public int PercentDone() {
            return (int)(((totalRailWorkRequired-railWorkRequired) / totalRailWorkRequired) * 100); }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref railType, "railRailTypee");
            Scribe_Values.Look(ref totalRailWorkRequired, "railTotalWorkRequired");
            Scribe_Values.Look(ref railWorkRequired, "railworkRequired");
            Scribe_Values.Look(ref finished, "railfinished");
            Scribe_Values.Look(ref useable, "railuseable");
        }

        
    }
}
