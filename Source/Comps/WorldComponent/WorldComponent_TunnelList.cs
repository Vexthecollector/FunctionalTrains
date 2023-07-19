using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    public class WorldComponent_TunnelList : WorldComponent
    {

        public static WorldComponent_TunnelList Instance;
        public WorldComponent_TunnelList(World world) : base(world) => Instance = this;

        private List<Tunnel> tunnels = new List<Tunnel>();

        public List<Tunnel> Tunnels
        {
            get { return tunnels; }
        }

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Collections.Look(ref tunnels, "FunctionalTrainsTunnels", LookMode.Deep);
            if (tunnels == null) tunnels = new List<Tunnel>();
        }
    }
}
