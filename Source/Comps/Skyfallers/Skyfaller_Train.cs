﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FunctionalTrains
{
    internal class Skyfaller_Train : Skyfaller, IActiveDropPod, IThingHolder
    {
        public int groupID = -1;
        public int destinationTile = -1;
        public TransportPodsArrivalAction arrivalAction;
        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveDropPod)this.innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveDropPod)this.innerContainer[0]).Contents = value;
            }
        }

        protected override void Impact()
        {
            SpawnThings();
            innerContainer.ClearAndDestroyContents();
        }
    }
}