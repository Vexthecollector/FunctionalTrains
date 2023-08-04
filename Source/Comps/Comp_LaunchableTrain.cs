using FunctionalTrains;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace FunctionalTrains
{
    [StaticConstructorOnStartup]
    public class Comp_LaunchableTrain : ThingComp
    {
        private CompTransporter cachedCompTransporter;
        private Comp_TrainStation currentlyResidingStation;
        private static readonly Texture2D addWagonIcon = ContentFinder<Texture2D>.Get("Gizmos/FunctionalTrains/Icons/freight-wagon");
        private static readonly Texture2D removeWagonIcon = ContentFinder<Texture2D>.Get("Gizmos/FunctionalTrains/Icons/freight-wagon");
        private Command_Action addWagon;
        private Command_Action removeWagon;


        public CompProperties_LaunchableTrain Props
        {
            get
            {
                return (CompProperties_LaunchableTrain)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            addWagon = new Command_Action
            {
                action = delegate
                {
                    AddWagonFloatMenu();
                    //AddWagon();
                },
                defaultLabel = "Add Wagon",
                defaultDesc = "Add a Wagon to the Train",
                icon = addWagonIcon
            };
            removeWagon = new Command_Action
            {
                action = delegate
                {
                    RemoveWagonFloatMenu();
                    //RemoveWagon();
                },
                defaultLabel = "Remove Wagon",
                defaultDesc = "Remove a Wagon from the Train",
                icon = removeWagonIcon
            };

            currentlyResidingStation = GetCurrentlyResidingStation();
            if (currentlyResidingStation != null) currentlyResidingStation.isOccupied = true;
            if (respawningAfterLoad) return;
        }

        public override void PostDeSpawn(Map map)
        {
            currentlyResidingStation.isOccupied = false;
            base.PostDeSpawn(map);
        }

        public void Send(Rail rail)
        {
            Comp_TrainStation destinationStation = GetUseableDestinationStation();


            if (destinationStation != null)
            {

                if (this.Props.requireFuel)
                {
                    float fuelcost = CalculateFuelCost(destinationStation.Map);
                    //float fuelcost = Find.WorldGrid.TraversalDistanceBetween(currentlyResidingStation.Map.Tile, destinationStation.Map.Tile) * 2;
                    CompRefuelable currentFuel = currentlyResidingStation.parent.GetComp<CompRefuelable>();
                    if (currentFuel.Fuel >= fuelcost)
                    {
                        SendTrainToNewMap(destinationStation, destinationStation.Map, (Building_Train)this.parent, FunctionalTrainsDefOf.FT_Train, destinationStation.parent.Position, rail);
                        currentFuel.ConsumeFuel(fuelcost);
                    }
                    else Messages.Message($"Not Enough Fuel to send to destination. {currentFuel.Fuel} out of required {fuelcost}", MessageTypeDefOf.CautionInput);
                }
                else
                {

                    SendTrainToNewMap(destinationStation, destinationStation.Map, (Building_Train)this.parent, FunctionalTrainsDefOf.FT_Train, destinationStation.parent.Position, rail);
                }

            }
            else
            {
                Messages.Message("Can't send Train. Tunnel or Destination Unreachable", MessageTypeDefOf.CautionInput);
            }
        }

        private float CalculateFuelCost(Map map)
        {
            float fuelcost = Find.WorldGrid.TraversalDistanceBetween(currentlyResidingStation.Map.Tile, map.Tile) * 2;
            float cargo = 0;
            foreach (Thing thing in cachedCompTransporter.innerContainer)
            {
                cargo += thing.stackCount * thing.GetStatValue(StatDefOf.Mass);
            }
            fuelcost *= 1 + (cargo / 1000);
            return fuelcost;
        }

        private Thing SendTrainToNewMap(Comp_TrainStation destinationStation, Map map, Building_Train oldBuilding, ThingDef def, IntVec3 newPosition, Rail rail)
        {
            int hitPoints = oldBuilding.HitPoints;
            IntVec3 position = newPosition;
            Building_Train newBuilding = (Building_Train)ThingMaker.MakeThing(def);
            newBuilding.HitPoints = hitPoints;
            newBuilding.SetFaction(Faction.OfPlayer);
            cachedCompTransporter.innerContainer.TryTransferAllToContainer(newBuilding.GetComp<CompTransporter>().innerContainer);
            newBuilding.GetComp<CompTransporter>().groupID = 0;
            int time = PossibleTicksPerTile(this.Props.ticksPerTile, rail.RailType().ticksPerTile());
            time *= Find.WorldGrid.TraversalDistanceBetween(oldBuilding.Tile, map.Tile);
            time = Math.Abs(time);
            Messages.Message($"Train will arrive in {time} ticks", MessageTypeDefOf.NeutralEvent);
            newBuilding.PrepareArrive(1000, time, rail, destinationStation.parent.Rotation);
            newBuilding.wagonList = oldBuilding.wagonList;
            currentlyResidingStation.isOccupied = false;
            cachedCompTransporter.innerContainer.Clear();
            cachedCompTransporter.CancelLoad();
            oldBuilding.TrainLeave(1000);
            GenSpawn.Spawn(newBuilding, position, map, destinationStation.parent.Rotation);
            return newBuilding;
        }

        private int PossibleTicksPerTile(int trainSpeed, int railSpeed)
        {
            return Math.Max(trainSpeed, railSpeed);
        }


        public Comp_TrainStation GetUseableDestinationStation()
        {
            if (GetCurrentlyResidingStation() != null)
            {
                Comp_TrainStation station = currentlyResidingStation;
                if (station?.selectedStation != null && (station.currentTunnel?.IsUseable() ?? false))
                {
                    return station.selectedStation;
                }
            }
            return null;

        }

        public Comp_TrainStation GetCurrentlyResidingStation()
        {
            if (currentlyResidingStation != null)
            {
                currentlyResidingStation.isOccupied = true;
                return currentlyResidingStation;
            }
            List<Comp_TrainStation> stations = WorldComponent_StationList.Instance.Stations;
            for (int i = 0; i < WorldComponent_StationList.Instance.Stations.Count; i++)
            {

                if (stations[i].parent.Map == this.parent.Map && stations[i].parent.Position == this.parent.Position)
                {
                    stations[i].isOccupied = true;
                    return stations[i];
                }
            }
            return null;
        }

        public bool LoadingInProgressOrReadyToLaunch
        {
            get
            {
                return this.Transporter.LoadingInProgressOrReadyToLaunch;
            }
        }

        public bool AnythingLeftToLoad
        {
            get
            {
                return this.Transporter.AnythingLeftToLoad;
            }
        }

        public Thing FirstThingLeftToLoad
        {
            get
            {
                return this.Transporter.FirstThingLeftToLoad;
            }
        }

        public CompTransporter Transporter
        {
            get
            {
                if (this.cachedCompTransporter == null)
                {
                    this.cachedCompTransporter = this.parent.GetComp<CompTransporter>();
                }
                return this.cachedCompTransporter;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            if (this.LoadingInProgressOrReadyToLaunch)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "CommandLaunchGroup".Translate();
                command_Action.defaultDesc = "CommandLaunchGroupDesc".Translate();
                command_Action.icon = CompLaunchable.LaunchCommandTex;
                command_Action.alsoClickIfOtherInGroupClicked = false;
                command_Action.action = delegate ()
                {
                    if (this.AnythingLeftToLoad)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSendNotCompletelyLoadedPods".Translate(this.FirstThingLeftToLoad.LabelCapNoCount, this.FirstThingLeftToLoad), new Action(SelectRailToSendTrainOn), false, null, WindowLayer.Dialog));
                        return;
                    }
                    SelectRailToSendTrainOn();
                };
                yield return command_Action;
            }
            else
            {
                yield return addWagon;
                yield return removeWagon;
            }
            yield break;
        }

        private void AddWagonFloatMenu()
        {

            List<FloatMenuOption> list = new List<FloatMenuOption>();
            List<WagonDef> wagons = DefDatabase<WagonDef>.AllDefsListForReading;
            foreach (WagonDef def in wagons)
            {

                list.Add(new FloatMenuOption($"Add {def.label} ", () =>
                {
                    AddWagon(def);
                }));

            }
            if (list.Any<FloatMenuOption>())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }



        }

        private void AddWagon(WagonDef def)
        {
            ((Building_Train)this.parent).wagonList.Add(def);
            ((Building_Train)this.parent).RecalculateMassCapacity();
        }


        private void RemoveWagonFloatMenu()
        {
            if (!((Building_Train)this.parent).wagonList.NullOrEmpty())
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (WagonDef def in ((Building_Train)this.parent).wagonList)
                {

                    list.Add(new FloatMenuOption($"Remove {def.label}", () =>
                    {
                        RemoveWagon(def);
                    }));
                }


                if (list.Any<FloatMenuOption>())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }

            }
        }


        private void RemoveWagon(WagonDef def)
        {
            ((Building_Train)this.parent).wagonList.Remove(def);
            ((Building_Train)this.parent).RecalculateMassCapacity();
        }

        /// <summary>
        /// Goes through all possible Rails and returns those that are useable for the current train to be send on.
        /// </summary>
        private void SelectRailToSendTrainOn()
        {
            if (GetCurrentlyResidingStation() != null)
            {
                currentlyResidingStation = GetCurrentlyResidingStation();
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                if (currentlyResidingStation.currentTunnel?.Rails() != null && !GetUseableDestinationStation().isOccupied)
                {

                    for (int i = 0; i < currentlyResidingStation.currentTunnel.Rails().Count; i++)
                    {
                        int j = i;
                        if (currentlyResidingStation.currentTunnel.Rails()[j].IsUseable())
                        {

                            list.Add(new FloatMenuOption($"Select Rail {j + 1}, {currentlyResidingStation.currentTunnel.Rails()[j].RailType().RailName()}", () =>
                            {
                                Send(currentlyResidingStation.currentTunnel.Rails()[j]);
                            }));
                        }

                    }
                    if (list.Any<FloatMenuOption>())
                    {
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                    else
                    {
                        Messages.Message("No Useable Rails Found", MessageTypeDefOf.NegativeEvent);
                    }
                }
                else
                {
                    Messages.Message("No Useable Tunnel or Destination Found", MessageTypeDefOf.NegativeEvent);
                }
            }
        }
        public override string CompInspectStringExtra()
        {
            if (!this.LoadingInProgressOrReadyToLaunch)
            {
                return null;
            }
            if (this.AnythingLeftToLoad)
            {
                return "NotReadyForLaunch".Translate() + ": " + "TransportPodInGroupHasSomethingLeftToLoad".Translate().CapitalizeFirst() + ".";
            }
            return "ReadyForLaunch".Translate();
        }

    }
}
