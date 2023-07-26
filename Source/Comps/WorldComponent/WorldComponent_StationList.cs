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
    internal class WorldComponent_StationList : WorldComponent
    {
        public static WorldComponent_StationList Instance;
        public WorldComponent_StationList(World world) : base(world) => Instance = this;

        private List<Comp_TrainStation> stations = new List<Comp_TrainStation>();

        public List<Comp_TrainStation> Stations
        {
            get { return stations; }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Collections.Look(ref stations, "FunctionalTrainsStations", LookMode.Deep);
            //if (stations == null) stations = new List<Comp_TrainStation>();
        }

    }
}
