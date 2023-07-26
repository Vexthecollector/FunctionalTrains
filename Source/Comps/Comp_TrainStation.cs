using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using System.Threading;
using System.Net.Http;

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
        private Command_Action createRail;
        private Command_Action instantFinishRail;
        public Tunnel currentTunnel;
        public Map Map => parent.MapHeld;



        public new CompProperties_TrainStation Props => (CompProperties_TrainStation)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            WorldComponent_StationList.Instance.Stations.AddDistinct(this);
            if (selectedStationThing != null)
            {
                DoPostSpawnStuff();

            }

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
                    Process_Stations();
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
                    PossibleTunnelCreations();
                }
            };
            createRail = new Command_Action
            {
                defaultLabel = "Create Rail",
                defaultDesc = "Creates a new Rail of the specific type",
                action = delegate
                {
                    CreateRail();
                }
            };
            instantFinishRail = new Command_Action
            {
                defaultLabel = "DEV: Finish Rail",
                action = delegate
                {
                    InstantFinishRail();
                }
            };

            CreateStringContent();
            if (respawningAfterLoad) return;
            Find.WindowStack.Add(new Dialog_RenameStation(this));
        }

        public void DoPostSpawnStuff()
        {
            selectedStation = selectedStationThing.GetComp<Comp_TrainStation>();
            currentTunnel = GetTunnel();
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
            if (currentTunnel?.IsUseable() ?? false /*&& currentTunnel?.MaxRails()>currentTunnel?.CurrentRails()*/)
            {
                yield return createRail;
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

                if (currentTunnel.Rails().Count() > 0)
                {
                    yield return instantFinishRail;
                }

            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref name, "Name");
            Scribe_References.Look(ref selectedStationThing, "selectedStationThing");
            //Scribe_Deep.Look(ref currentTunnel, "stationCurrentTunnel");
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (currentTunnel == null) currentTunnel = GetTunnel();
            CreateStringContent();
        }


        string stringContent;
        public override string CompInspectStringExtra()
        {
            //CreateStringContent();
            return stringContent + base.CompInspectStringExtra();
        }

        public void CreateStringContent()
        {
            string text = "Station Name: " + this.name;
            if (this.selectedStation != null)
            {
                text += "\nCurrently Selected Destination: " + selectedStation.name + " | Distance: " + GetDistance() + "";
            }

            if (this.currentTunnel != null)
            {
                if (currentTunnel.IsFinished() == false) text += "\nTunnel is " + currentTunnel.PercentDone() + "% done.";
                else if (currentTunnel.IsUseable()) text += $"\n{this.currentTunnel.TunnelType().TunnelName()} is useable";
                else text += $"\n{this.currentTunnel.TunnelType().TunnelName()} is not useable";
            }
            if (this.currentTunnel?.Rails()?.Count > 0)
            {
                for (int i = 0; i < this.currentTunnel.Rails().Count; i++)
                {
                    Rail rail = this.currentTunnel.Rails()[i];
                    if (rail.IsFinished() == false) text += $"\nRail {i + 1} is " + rail.PercentDone() + "% done";
                    else if (rail.IsUseable()) text += $"\nRail {i + 1} is useable";
                    else text += $"\nRail {i + 1} is not useable";
                }
            }
            stringContent = text;
        }

        public int GetDistance()
        {
            try
            {

                return Find.WorldGrid.TraversalDistanceBetween(this.Map.Tile, selectedStation.Map.Tile);
            }
            catch (Exception ex)
            {
                Log.Warning("FT_Train: Couldn't find distance between the two stations. Returning -1.");
            }
            return -1;
        }

        public void InstantFinishTunnel()
        {
            currentTunnel.InstantFinishWork();
            CreateStringContent();
        }

        public Tunnel GetTunnel()
        {
            if (selectedStation?.Map != null)
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

        public void CreateRail()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            for (int i = 0; i < this.currentTunnel.TunnelType().Type(); i++)
            {
                RailType railType = new RailType(i + 1);
                list.Add(new FloatMenuOption("Create " + railType.RailName(), () =>
                {
                    //Code to write
                    Rail rail = new Rail(currentTunnel, railType);
                    currentTunnel.Rails().Add(rail);
                    CreateStringContent();
                }));

            }
            if (list.Any<FloatMenuOption>())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        public void InstantFinishRail()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            for (int i = 0; i < this.currentTunnel.Rails().Count; i++)
            {
                int j = i;
                list.Add(new FloatMenuOption($"Finish Rail {j + 1}", () =>
                {
                    this.currentTunnel.Rails()[j].InstantFinishWork();
                    CreateStringContent();
                }));

            }
            if (list.Any<FloatMenuOption>())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        public void Process_Stations()
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

        private void CreateTunnel(TunnelType tunnelType)
        {
            Tunnel tunnel = new Tunnel(this.Map, selectedStation.Map, tunnelType);
            WorldComponent_TunnelList.Instance.Tunnels.AddDistinct(tunnel);
            currentTunnel = tunnel;
            CreateStringContent();
        }

        private void PossibleTunnelCreations()
        {
            if (selectedStation != null)
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int i = 0; i < 6; i++)
                {
                    TunnelType tunnelType = new TunnelType(i + 1);
                    list.Add(new FloatMenuOption("Create " + tunnelType.TunnelName(), () =>
                    {
                        CreateTunnel(tunnelType);
                    }));

                }
                if (list.Any<FloatMenuOption>())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }


            }
        }

        public void CreateTunnelIfNotExist(TunnelType tunnelType)
        {
            if (GetTunnel() == null) CreateTunnel(tunnelType);
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
                    this.Station.CreateStringContent();
                    return;
                }
            }
            this.Station.name = name;
            this.Station.CreateStringContent();
        }
    }
}
