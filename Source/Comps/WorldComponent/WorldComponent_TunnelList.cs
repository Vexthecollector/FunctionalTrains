using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTrains
{
    internal class WorldComponent_TunnelList : WorldComponent
    {
        public static WorldComponent_TunnelList Instance;
        public WorldComponent_TunnelList(World world) : base(world) => Instance = this;

        private List<Tunnel> tunnels = new List<Tunnel>();

        public List<Tunnel> Tunnels
        {
            get { return tunnels; }
        }
    }
}
