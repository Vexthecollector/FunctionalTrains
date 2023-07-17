using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using System.Threading;

namespace FunctionalTrains
{
    [StaticConstructorOnStartup]
    public class Comp_TrainStation : ThingComp
    {
        private static readonly Texture2D stationSelectIcon = ContentFinder<Texture2D>.Get("Things/Buildings/FunctionalTrains/stationSelectIcon");
        public ThingOwner innerContainer;
        public string name;
        public ThingWithComps selectedStationThing;
        public Comp_TrainStation selectedStation;
        private Command_Action setName;
        private Command_Action selectStation;
        private Command_Action createTunnel;
        public Tunnel currentTunnel;
        public Map Map => parent.MapHeld;



        public new CompProperties_TrainStation Props => (CompProperties_TrainStation)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            WorldComponent_StationList.Instance.Stations.AddDistinct(this);
            if (selectedStationThing != null) selectedStation = selectedStationThing.GetComp<Comp_TrainStation>();

            setName = new Command_Action
            {
                action = () => Find.WindowStack.Add(new Dialog_RenameStation(this)),
                defaultLabel = "Rename Station",
                defaultDesc = "Rename the Station"
            };
            selectStation = new Command_Action
            {
                action = delegate
                {
                    process_Stations();
                },
                defaultLabel = "Select Destination Station",
                defaultDesc = "Select the Destination Station",
                icon = stationSelectIcon
            };
            createTunnel = new Command_Action
            {
                defaultLabel = "Create Tunnel",
                defaultDesc = "Create a new Tunnel",
                action = delegate
                {
                    CreateTunnel(new TunnelType(this.Props.tunnelType));
                }
            };


            if (respawningAfterLoad) return;
            Find.WindowStack.Add(new Dialog_RenameStation(this));
        }

        public override void PostDeSpawn(Map map)
        {
            WorldComponent_StationList.Instance.Stations.Remove(this);
            base.PostDeSpawn(map);

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            yield return setName;
            yield return selectStation;
            if (selectedStation != null && GetTunnel() == null)
            {
                yield return createTunnel;
            }

            if (DebugSettings.ShowDevGizmos)
            {
                if (selectedStation != null && GetTunnel() != null)
                {
                    Command_Action command_Action = new Command_Action();
                    command_Action.defaultLabel = "DEV: Finish tunnel";
                    command_Action.action = delegate
                    {
                        InstantFinishTunnel();
                    };
                    yield return command_Action;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref name, "Name");
            Scribe_References.Look(ref selectedStationThing, "selectedStationThing");
        }

        public void process_Stations()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            for (int i = 0; i < WorldComponent_StationList.Instance.Stations.Count; i++)
            {
                Comp_TrainStation stat = WorldComponent_StationList.Instance.Stations[i];
                if (stat == this) continue;
                list.Add(new FloatMenuOption("Select " + stat.name, () =>
                {
                    selectedStation = stat;
                    selectedStationThing = stat.parent;
                    currentTunnel = GetTunnel();
                }));

            }
            if (list.Any<FloatMenuOption>())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = "Station Name: " + this.name;
            if (this.selectedStation != null)
            {
                text += "\nCurrently Selected Destination: " + selectedStation.name + " | Distance: " + GetDistance() + "";
            }

            if (this.currentTunnel != null)
            {
                if (currentTunnel.isFinished() == false) text += "\nPercent Done: " + currentTunnel.percentDone() + "";
                else if (currentTunnel.isUseable()) text += "\nTunnel is useable";
                else text += "\nTunnel is not useable";
            }

            return text + base.CompInspectStringExtra();
        }

        public int GetDistance()
        {
            return Find.WorldGrid.TraversalDistanceBetween(this.Map.Tile, selectedStation.Map.Tile);
        }

        public void InstantFinishTunnel()
        {
            currentTunnel.instantFinishWork();
        }

        public Tunnel GetTunnel()
        {
            if (this.selectedStation != null && this.Map != selectedStation.Map)
            {
                for (int i = 0; i < WorldComponent_TunnelList.Instance.Tunnels.Count; i++)
                {
                    Tunnel tunnel = WorldComponent_TunnelList.Instance.Tunnels[i];
                    if ((tunnel.startMap == this.Map || tunnel.endMap == this.Map) && (tunnel.startMap == selectedStation.Map || tunnel.endMap == selectedStation.Map))
                    {
                        return tunnel;
                    }
                }
            }
            return null;
        }

        private void CreateTunnel(TunnelType tunnelType)
        {
            if (selectedStation != null)
            {
                Tunnel tunnel = new Tunnel(this.Map, selectedStation.Map, tunnelType);
                WorldComponent_TunnelList.Instance.Tunnels.AddDistinct(tunnel);
                currentTunnel = tunnel;
            }
        }

        public void CreateTunnelIfNotExist(TunnelType tunnelType)
        {
            if (GetTunnel() == null) CreateTunnel(tunnelType);
        }



        public void SendTrain()
        {
            
            
        }

    }

    public class Dialog_RenameStation : Dialog_Rename
    {
        public Comp_TrainStation Station;

        public Dialog_RenameStation(Comp_TrainStation station)
        {
            this.Station = station;
            this.curName = station.name ?? "Train Station " + " #" + Rand.Range(1, 99).ToString("D2");
        }


        protected override void SetName(string name)
        {
            for (int i = 0; i < WorldComponent_StationList.Instance.Stations.Count; i++)
            {
                if (WorldComponent_StationList.Instance.Stations[i].name == name)
                {
                    Messages.Message("Station with the same Name already exists, appending 1 to the name", MessageTypeDefOf.RejectInput);
                    this.Station.name = name + 1;
                    return;
                }
            }
            this.Station.name = name;
        }
    }
}
