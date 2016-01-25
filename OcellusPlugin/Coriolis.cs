using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


// **********************************************************
// *  Functions for displaying ship on Coriolis.io website  *
// **********************************************************

class Coriolis
{
    public enum mounts { Fixed, Gimballed, Turrent }

    [DataContractAttribute]
    public class CargoHatch
    {
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerPlant
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Thrusters
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FrameShiftDrive
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class LifeSupport
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerDistributor
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Sensors
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FuelTank
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Standard
    {
        [DataMemberAttribute]
        public string bulkheads { get; set; }
        [DataMemberAttribute]
        public CargoHatch cargoHatch { get; set; }
        [DataMemberAttribute]
        public PowerPlant powerPlant { get; set; }
        [DataMemberAttribute]
        public Thrusters thrusters { get; set; }
        [DataMemberAttribute]
        public FrameShiftDrive frameShiftDrive { get; set; }
        [DataMemberAttribute]
        public LifeSupport lifeSupport { get; set; }
        [DataMemberAttribute]
        public PowerDistributor powerDistributor { get; set; }
        [DataMemberAttribute]
        public Sensors sensors { get; set; }
        [DataMemberAttribute]
        public FuelTank fuelTank { get; set; }
    }

    [DataContractAttribute]
    public class Hardpoint
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
        [DataMemberAttribute]
        public string group { get; set; }
        [DataMemberAttribute]
        public string mount { get; set; }
    }

    [DataContractAttribute]
    public class Utility
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
        [DataMemberAttribute]
        public string group { get; set; }
        [DataMemberAttribute]
        public string name { get; set; }
    }

    [DataContractAttribute]
    public class Internal
    {
        [DataMemberAttribute]
        public int @class { get; set; }
        [DataMemberAttribute]
        public string rating { get; set; }
        [DataMemberAttribute]
        public bool enabled { get; set; }
        [DataMemberAttribute]
        public int priority { get; set; }
        [DataMemberAttribute]
        public string group { get; set; }
        [DataMemberAttribute]
        public string name { get; set; }
    }

    [DataContractAttribute]
    public class Components
    {
        [DataMemberAttribute]
        public Standard standard = new Standard();
        [DataMemberAttribute]
        public List<Hardpoint> hardpoints = new List<Hardpoint>();
        [DataMemberAttribute]
        public List<Utility> utility = new List<Utility>();
        [DataMemberAttribute]
        public List<Internal> @internal = new List<Internal>();
    }

    [DataContract]
    public class RootObject
    {
        [DataMember(Name="$schema", Order = 0)]
        public string schema { get; set; }
        [DataMember(Order = 1)]
        public string name { get; set; }
        [DataMember(Order = 2)]
        public string ship { get; set; }
        [DataMember (Order = 3)]
        public Components components = new Components();
    }

    public static string createCoriolisJson(ref Dictionary<string, object> state)
    {

        try
        {
            if (!state.ContainsKey("VAEDcompanionDict"))
            {
                return null;
            }
            Dictionary<string, dynamic> companion = (Dictionary<string, dynamic>)state["VAEDcompanionDict"];
            RootObject coriolis = new RootObject();
            coriolis.schema = "http://json-schema.org/draft-04/schema#";
            coriolis.name = "Anaconda";
            coriolis.ship = "Anaconda";
            coriolis.components.standard.bulkheads = "sdf";
            Hardpoint gun = new Hardpoint();
            gun.rating = "A";
            coriolis.components.hardpoints.Add(gun);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, coriolis);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            Debug.Write("json " + json);
            return json;
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
        return null;
    }
    
}