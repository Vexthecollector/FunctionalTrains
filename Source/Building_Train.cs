﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FunctionalTrains
{
    public class Building_Train : Building
    {
        bool leaving;
        int ticksToLeave;
        DestroyMode destroyMode;
        bool arriving;
        int ticksToArrive = 0;
        int leavingTicks = 0;
        private Comp_TrainStation currentlyResidingStation;
        private Rail usedRail;
        Vector3 initialAngleVector;
        Vector3 angleVector;

        public override void Draw()
        {
            if (leaving)
            {
                Vector3 v3 = this.DrawPos;
                v3 += angleVector;
                DrawAt(v3);
                return;
            }
            if (arriving)
            {
                if (leavingTicks > 0) { return; }

                else
                {

                    Vector3 v3 = this.DrawPos;
                    v3 += angleVector;
                    DrawAt(v3);
                }

                return;
            }
            base.Draw();
        }

        public override void Tick()
        {
            base.Tick();
            if (leaving)
            {
                increaseV3();
                ticksToLeave--;


                if (ticksToLeave < 1)
                {
                    DeSpawn(destroyMode);
                }

            }
            if (arriving)
            {
                if (leavingTicks > 0)
                {
                    leavingTicks--;

                }
                else
                {
                    if (ticksToArrive < 1)
                    {
                        usedRail.inUse = false;
                        arriving = false;
                        this.def.destroyable = true;
                        this.def.selectable = true;
                    }
                    increaseV3();
                    ticksToArrive--;
                }

            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            getStation();
            if (currentlyResidingStation != null)
                currentlyResidingStation.isOccupied = false;
            base.DeSpawn(mode);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (respawningAfterLoad || currentlyResidingStation == null)
                getStation();
        }

        /// <summary>
        /// Sets the train to be leaving. This causes it to draw a leaving animation and destroy the train after it is finished.
        /// </summary>
        public void TrainLeave(int ticksToLeave, DestroyMode mode = DestroyMode.Vanish)
        {
            Rot4 r4 = this.Rotation;
            initialAngleVector = r4.FacingCell.ToVector3();
            angleVector = initialAngleVector;
            getStation();
            destroyMode = mode;
            leaving = true;
            this.ticksToLeave = ticksToLeave;
        }



        private void increaseV3()
        {
            Rot4 r4 = this.Rotation;
            angleVector += initialAngleVector;
        }

        private void decreaseV3()
        {
            Rot4 r4 = this.Rotation;
            angleVector -= initialAngleVector;
        }



        /// <summary>
        /// Prepares the arrival of the train. 
        /// It is set to be not selectable and not destroyable until it arrives.
        /// </summary>
        /// <param name="ticksToArrive">How long the drawing animation should take</param>
        /// <param name="leavingTicks">The time it takes for the train to travel until the drawing animation begins</param>
        /// <param name="rail">The rail that is being used by the train</param>
        /// <param name="r4">The rotation the train should have on arrival</param>
        public void PrepareArrive(int ticksToArrive, int leavingTicks, Rail rail, Rot4 r4)
        {
            usedRail = rail;
            rail.inUse = true;
            initialAngleVector = r4.FacingCell.ToVector3();
            angleVector = initialAngleVector;
            angleVector.Scale(new Vector3(-ticksToArrive, -ticksToArrive, -ticksToArrive));

            arriving = true;
            this.ticksToArrive = ticksToArrive;
            this.leavingTicks = leavingTicks;

            this.def.destroyable = false;
            this.def.selectable = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref currentlyResidingStation, "currentlyResidingStation");
        }

        /// <summary>
        /// Gets the train station this train is currently on top of.
        /// </summary>
        public Comp_TrainStation getStation()
        {
            if (currentlyResidingStation != null)
            {
                currentlyResidingStation.isOccupied = true;
                return currentlyResidingStation;
            }
            List<Comp_TrainStation> stations = WorldComponent_StationList.Instance.Stations;
            for (int i = 0; i < WorldComponent_StationList.Instance.Stations.Count; i++)
            {
                if (stations[i].parent.Position == this.Position)
                {
                    stations[i].isOccupied = true;
                    return stations[i];
                }
            }
            return null;
        }
    }

}
