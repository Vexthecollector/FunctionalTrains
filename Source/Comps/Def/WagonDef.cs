using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FunctionalTrains
{
    public class WagonDef : Def
    {
        public WagonDef() { }

        public string graphic;
        public float massCapacity;
        private Material wagonImage;
        public GraphicData graphicData;
        public Material WagonImage()
        {
            if(wagonImage == null)
            {
                wagonImage = MaterialPool.MatFrom(graphic);
                return MaterialPool.MatFrom(graphic);
            }
            return wagonImage;
            
        } 



    }
}
