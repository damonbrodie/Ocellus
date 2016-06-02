using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Linq;


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
            coriolis.components.hardpoints = shipObj.hardpoints;
            coriolis.components.utility = shipObj.utility;
            coriolis.components.@internal = shipObj.@internal;

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