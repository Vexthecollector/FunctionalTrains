using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class Rail
    {
        bool useable;
        bool finished;
        int totalRailWorkRequired;
        int railWorkRequired;
        RailType railType;
        Tunnel parentTunnel;

        public Rail() { }
        public Rail(Tunnel tunnel,RailType railType)
        {
            this.parentTunnel = tunnel;
            this.railType = railType;
            totalRailWorkRequired = railWorkRequired = railType.WorkRequired() * parentTunnel.distance;
        }

        public RailType RailType() { return railType; }
        public bool IsUseable()
        {
            return useable;
        }
        public bool IsFinished()
        {
            return finished;
        }
        public void InstantFinishWork() { finished = true; railWorkRequired = 0; useable = true; }

        public int PercentDone() {
            return (totalRailWorkRequired - railWorkRequired / totalRailWorkRequired) * 100; }
    }
}
