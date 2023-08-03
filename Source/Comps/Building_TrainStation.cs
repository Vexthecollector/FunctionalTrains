using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FunctionalTrains
{
    public  class Building_TrainStation : Building
    {
        bool isOccupied;

        public bool IsOccupied() { return isOccupied; }
        public void setOccupation(bool occupied) { isOccupied = occupied; }
    }
}
