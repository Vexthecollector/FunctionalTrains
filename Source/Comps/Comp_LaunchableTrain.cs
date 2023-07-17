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

        public CompProperties_LaunchableTrain Props
        {
            get
            {
                return (CompProperties_LaunchableTrain)this.props;
            }
        }
        public void Send()
        {
            Comp_TrainStation destinationStation = getDestinationStation();
            if (destinationStation !=null)
            {
                Map map = this.parent.Map;
                Log.Message("FunctionalTrains: Test");
                this.Transporter.TryRemoveLord(map);
                int groupID = this.Transporter.groupID;
                ThingOwner directlyHeldThings = cachedCompTransporter.GetDirectlyHeldThings();
                ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod, null);
                activeDropPod.Contents = new ActiveDropPodInfo();
                activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true, true);
                Log.Message("FunctionalTrains: Test2");
                Skyfaller_Train flyShipLeaving = (Skyfaller_Train) SkyfallerMaker.MakeSkyfaller(ThingDefOf.DropPodLeaving, activeDropPod);
                Log.Message("FunctionalTrains: Test3");
                flyShipLeaving.groupID = groupID;
                flyShipLeaving.destinationTile = destinationStation.Map.Tile;
                flyShipLeaving.arrivalAction = new TransportPodsArrivalAction_LandInSpecificCell(destinationStation.Map.Parent, destinationStation.parent.Position);
                cachedCompTransporter.CleanUpLoadingVars(map);
                cachedCompTransporter.parent.Destroy(DestroyMode.Vanish);
                GenSpawn.Spawn(flyShipLeaving, cachedCompTransporter.parent.Position, map, WipeMode.Vanish);
            }
        }



        public Comp_TrainStation getDestinationStation()
        {
            Comp_TrainStation station= getStation();
            if (station!=null && station.selectedStation!=null)
            {
                if(station.currentTunnel!=null && station.currentTunnel.isUseable())
                {
                    return station.selectedStation;
                }
            }
            return null;
            
        }

        public Comp_TrainStation getStation()
        {
            List<Comp_TrainStation> stations = WorldComponent_StationList.Instance.Stations;
            for (int i = 0; i < WorldComponent_StationList.Instance.Stations.Count; i++)
            {
                if (stations[i].parent.Position == this.parent.Position)
                {
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
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSendNotCompletelyLoadedPods".Translate(this.FirstThingLeftToLoad.LabelCapNoCount, this.FirstThingLeftToLoad), new Action(this.Send), false, null, WindowLayer.Dialog));
                        return;
                    }
                    this.Send();
                };
                yield return command_Action;
            }
            yield break;
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
