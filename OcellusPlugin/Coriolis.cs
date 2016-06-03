using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


// **********************************************************
// *  Functions for displaying ship on Coriolis.io website  *
// **********************************************************
class Coriolis
{
    [DataContract]
    public class CoriolisObj
    {
        [DataMember(Name = "$schema", Order = 1)]
        public string schema = "http://cdn.coriolis.io/schemas/ship-loadout/2.json#";
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3)]
        public string ship { get; set; }
        [DataMember(Order = 4)]
        public Ship.Components components = new Ship.Components();
    }

    public static string export(Ship.Components shipObj)
    {
        try
        {
            CoriolisObj coriolis = new CoriolisObj();

            coriolis.components.standard = shipObj.standard;
            List<Ship.Hardpoint> sortedHardpoints = shipObj.hardpoints.OrderBy(o => o.slotSize).ToList();
            foreach(Ship.Hardpoint hardpoint in sortedHardpoints)
            {
                if (hardpoint.rating == null)
                {
                    coriolis.components.hardpoints.Add(null);
                }
                else
                {
                    coriolis.components.hardpoints.Add(hardpoint);
                }
            }
            List<Ship.Internal> sortedInternals = shipObj.@internal.OrderByDescending(o => o.slotSize).ToList();
            foreach (Ship.Internal @internal in sortedInternals)
            {
                if (@internal.rating == null)
                {
                    coriolis.components.@internal.Add(null);
                }
                else
                {
                    coriolis.components.@internal.Add(@internal);
                }
            }
            List<Ship.Utility> sortedUtilities = shipObj.utility.OrderBy(o => o.slot).ToList();
            foreach (Ship.Utility utility in sortedUtilities)
            {
                if (utility.rating == null)
                {
                    coriolis.components.utility.Add(null);
                }
                else
                {
                    coriolis.components.utility.Add(utility);
                }
            }


            string shipType = Elite.frontierShipToCoriolis(shipObj.attributes.shiptype);
            coriolis.name = "Ocellus - " + shipType;
            coriolis.ship = shipType;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CoriolisObj));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, coriolis);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            return json;
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
        return null; 
    }
}