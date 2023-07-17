using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTrains
{
    internal class WorldComponent_StationList : WorldComponent
    {
        public static WorldComponent_StationList Instance;
        public WorldComponent_StationList(World world) : base(world) => Instance = this;

        private List<Comp_TrainStation> stations = new List<Comp_TrainStation>();

        public List<Comp_TrainStation> Stations
        {
            get { return stations; }
        }


    }
}
