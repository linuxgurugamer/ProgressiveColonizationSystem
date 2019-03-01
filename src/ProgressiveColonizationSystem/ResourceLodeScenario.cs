﻿using FinePrint;
using System.Collections.Generic;
using System.Linq;

namespace ProgressiveColonizationSystem
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT)]
    public class ResourceLodeScenario
        : ScenarioModule
    {
        public static ResourceLodeScenario Instance { get; private set; }

        private readonly List<ResourceLode> activeLodes = new List<ResourceLode>();

        public ResourceLodeScenario()
        {
            Instance = this;
        }

        public ResourceLode GetOrCreateResourceLoad(Vessel nearVessel)
        {
            // There's only allowed one resource load - you have to harvest it until it's gone
            // So find the thing first.
            var lode = this.activeLodes.FirstOrDefault(rl => rl.bodyName == nearVessel.mainBody.name);

            if (lode != null)
            {
                // Ensure that there's a waypoint
                if (!Waypoints.TryFindWaypointById(lode.Identifier, out _))
                {
                    Waypoints.CreateWaypointAt("Resource Lode", nearVessel.mainBody, lode.Latitude, lode.Longitude);
                }
            }
            else
            {
                var waypoint = Waypoints.CreateWaypointNear("Resource Lode", nearVessel, 1000, 3000);
                lode = new ResourceLode(waypoint);
                activeLodes.Add(lode);
            }

            return lode;
        }

        public bool TryFindResourceLodeInRange(Vessel vessel, out ResourceLode resourceLode)
        {
            // There's only allowed one resource load - you have to harvest it until it's gone
            // So find the thing first.
            resourceLode = this.activeLodes.FirstOrDefault(rl => rl.bodyName == vessel.mainBody.name);
            if (resourceLode == null)
            {
                return false;
            }

            // Ensure that there's a waypoint
            if (!Waypoints.TryFindWaypointById(resourceLode.Identifier, out Waypoint waypoint))
            {
                waypoint = Waypoints.CreateWaypointAt("Resource Lode", vessel.mainBody, resourceLode.Latitude, resourceLode.Longitude);
            }

            return Waypoints.StraightLineDistanceInMetersFromWaypoint(vessel, waypoint) < 150.0;
        }

        public bool TryConsume(ResourceLode resourceLode, double amountRequested, out double amountReceived)
        {
            if (resourceLode.Quantity <= amountRequested)
            {
                this.activeLodes.Remove(resourceLode);
                amountReceived = resourceLode.Quantity;
                return amountRequested == resourceLode.Quantity;
            }
            else
            {
                resourceLode.Quantity -= amountRequested;
                amountReceived = amountRequested;
                return true;
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            this.activeLodes.Clear();
            foreach (var childNode in node.GetNodes())
            {
                if (ResourceLode.TryLoad(node, out ResourceLode lode))
                {
                    this.activeLodes.Add(lode);
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            foreach (var lode in this.activeLodes)
            {
                node.AddNode(lode.Serialize());
            }
        }

        public class ResourceLode
        {
            internal ResourceLode(Waypoint waypoint)
            {
                this.Latitude = waypoint.latitude;
                this.Longitude = waypoint.longitude;
                this.bodyName = waypoint.celestialBody.name;
                this.Identifier = waypoint.id.ToString();
                this.DiscoveryTime = Planetarium.GetUniversalTime();
                this.Quantity = 5000;
            }

            internal ResourceLode(string bodyName, double latitude, double longitude, string identifier, double discoveryTime, double quantity)
            {
                this.Latitude = latitude;
                this.Longitude =longitude;
                this.bodyName = bodyName;
                this.Identifier = identifier;
                this.DiscoveryTime = discoveryTime;
                this.Quantity = 5000;
            }

            public static bool TryLoad(ConfigNode configNode, out ResourceLode resourceLode)
            {
                double latitude = 0;
                double longitude = 0;
                double discoveryTime = 0;
                double quantity = 0;
                if (!configNode.TryGetValue("latitude", ref latitude)
                 || !configNode.TryGetValue("longitude", ref longitude)
                 || !configNode.TryGetValue("discoveryTime", ref discoveryTime)
                 || !configNode.TryGetValue("quantity", ref quantity))
                {
                    resourceLode = null;
                    return false;
                }

                string bodyName = configNode.GetValue("body");
                string identifier = configNode.GetValue("id");
                resourceLode = new ResourceLode(bodyName, latitude, longitude, identifier, discoveryTime, quantity);
                return true;
            }

            public ConfigNode Serialize()
            {
                ConfigNode node = new ConfigNode();
                node.AddValue("id", this.Identifier);
                node.AddValue("body", this.bodyName);
                node.AddValue("latitude", this.Latitude);
                node.AddValue("longitude", this.Longitude);
                node.AddValue("discoveryTime", this.DiscoveryTime);
                node.AddValue("quantity", this.Quantity);
                return node;
            }

            public double Latitude { get; }
            public double Longitude { get; }
            public string bodyName { get; }
            public string Identifier { get; }
            public double DiscoveryTime { get; }
            public double Quantity { get; set; }
        }
    }
}
