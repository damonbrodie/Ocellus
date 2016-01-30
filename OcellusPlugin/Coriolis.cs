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
    [DataContractAttribute]
    public class CargoHatch
    {
        [DataMemberAttribute (Order = 1)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 2)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerPlant
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Thrusters
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FrameShiftDrive
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class LifeSupport
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerDistributor
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Sensors
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3) ]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FuelTank
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Standard
    {
        [DataMemberAttribute (Order = 1)]
        public string bulkheads { get; set; }
        [DataMemberAttribute (Order = 2)]
        public CargoHatch cargoHatch { get; set; }
        [DataMemberAttribute (Order = 3)]
        public PowerPlant powerPlant { get; set; }
        [DataMemberAttribute (Order = 4)]
        public Thrusters thrusters { get; set; }
        [DataMemberAttribute (Order = 5)]
        public FrameShiftDrive frameShiftDrive { get; set; }
        [DataMemberAttribute (Order = 6)]
        public LifeSupport lifeSupport { get; set; }
        [DataMemberAttribute (Order = 7)]
        public PowerDistributor powerDistributor { get; set; }
        [DataMemberAttribute (Order = 8)]
        public Sensors sensors { get; set; }
        [DataMemberAttribute (Order = 9)]
        public FuelTank fuelTank { get; set; }
    }

    [DataContractAttribute]
    public class Hardpoint
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute (Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute (Order = 6)]
        public string mount { get; set; }
    }

    [DataContractAttribute]
    public class Utility
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute (Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute (EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string name { get; set; }
    }

    [DataContractAttribute]
    public class Internal
    {
        [DataMemberAttribute (Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute (Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute (Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute (Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute (Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute (EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string name { get; set; }
    }

    [DataContractAttribute]
    public class Components
    {
        [DataMemberAttribute(Order = 1)]
        public Standard standard = new Standard();
        [DataMemberAttribute(Order = 2)]
        public List<Hardpoint> hardpoints = new List<Hardpoint>();
        [DataMemberAttribute(Order = 3)]
        public List<Utility> utility = new List<Utility>();
        [DataMemberAttribute (Order = 4)]
        public List<Internal> @internal = new List<Internal>();
    }

    [DataContract]
    public class RootObject
    {
        [DataMember(Name = "$schema", Order = 1)]
        public string schema = "http://cdn.coriolis.io/schemas/ship-loadout/2.json#";
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3)]
        public string ship { get; set; }
        [DataMember(Order = 4)]
        public Components components = new Components();
    }

    private static string mapShip(string shipType)
    {
        Dictionary<string, string> typeMap = new Dictionary<string, string>
        {
            { "Adder", "Adder" },
            { "Anaconda", "Anaconda" },
            { "Asp", "Asp Explorer" },
            { "Asp_Scout", "Asp Scout" },
            { "CobraMkIII", "Cobra Mk III" },
            { "CobraMkIV", "Cobra Mk IV" },
            { "DiamondBackXL", "Diamondback Explorer" },
            { "DiamondBack", "Diamondback Scout" },
            { "Eagle", "Eagle" },
            { "Federation_Dropship", "Federal Dropship" },
            { "Federation_Dropship_MkII", "Federal Assault Ship" },
            { "Federal Corvette", "Federal Corvette" } ,
            { "Federation_Gunship", "Federal Gunship" },
            { "FerDeLance", "Fer-De-Lance" },
            { "Hauler", "Hauler" },
            { "Empire_Trader", "Imperial Clipper" },
            { "Empire_Courier", "Imperial Courier" },
            { "Imperial Cutter", "Imperial Cutter" },
            { "Empire_Eagle", "Imperial Eagle" },
            { "Independant_Trader", "Keelback" },
            { "Orca", "Orca" },
            { "Python", "Python" },
            { "SideWinder", "Sidewinder" },
            { "Type6", "Type-6 Transporter" },
            { "Type7", "Type-7 Transporter" },
            { "Type9", "Type-9 Heavy" },
            { "Viper", "Viper" },
            { "Viper_MkIV", "Viper Mk IV" },
            { "Vulture", "Vulture" }
        };

        if (typeMap.ContainsKey(shipType))
        {
            return typeMap[shipType];
        }
        else
        {
            return null;
        }
    }

    private static string ModuleMap(string moduleName)
    {
        Dictionary<string, string> moduleMap = new Dictionary<string, string>
        {
            { "hugehardpoint", "hardpoints" },
            { "largehardpoint", "hardpoints" },
            { "mediumhardpoint", "hardpoints" },
            { "smallhardpoint", "hardpoints" },
            { "tinyhardpoint", "utility" },
            { "armour", "standard" },
            { "powerplant", "standard" },
            { "mainengines", "standard" },
            { "frameshiftdrive", "standard" },
            { "lifesupport", "standard" },
            { "powerdistributor", "standard" },
            { "radar", "standard" },
            { "fueltank", "standard" },
            { "slot", "internal" }
        };

        moduleName = moduleName.ToLower();

        foreach (string modType in moduleMap.Keys)
        {
            if (moduleName.StartsWith(modType))
            {
                return moduleMap[modType];
            }
        }
        return null;
    }

    private static void addHardpoint(Dictionary<string, dynamic> currModule, ref RootObject coriolis)
    {
        Dictionary<string, Hardpoint> weaponMap = new Dictionary<string, Hardpoint>
        {
            { "hpt_advancedtorppylon_fixed_small", new Hardpoint { @class=1, rating="I", group="Torpedo Pylon", mount="Fixed" } },
            { "hpt_advancedtorppylon_fixed_medium", new Hardpoint { @class=2, rating="I", group="Torpedo Pylon", mount="Fixed" } },
            { "hpt_basicmissilerack_fixed_small", new Hardpoint { @class=1, rating="B", group="Missle Rack", mount="Fixed" } },
            { "hpt_basicmissilerack_fixed_medium" , new Hardpoint { @class=2, rating="B", group="Missle Rack", mount="Fixed" } },
            { "hpt_beamlaser_fixed_small", new Hardpoint { @class=1, rating="E", group="Beam Laser", mount="Fixed" } },
            { "hpt_beamlaser_fixed_medium", new Hardpoint { @class=2, rating="D", group="Beam Laer", mount="Fixed" } },
            { "hpt_beamlaser_fixed_large", new Hardpoint { @class=3, rating="C", group="Beam Laser", mount="Fixed" } },
            { "hpt_beamlaser_gimbal_small", new Hardpoint { @class=1, rating="E", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_gimbal_medium", new Hardpoint { @class=2, rating="D", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_gimbal_large", new Hardpoint { @class=3, rating="C", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_turret_small", new Hardpoint { @class=1, rating="F", group="Beam Laser", mount="Turret" } },
            { "hpt_beamlaser_turret_medium", new Hardpoint { @class=2, rating="E", group="Beam Laser", mount="Turret" } },
            { "hpt_beamlaser_turret_large", new Hardpoint { @class=3, rating="D", group="Beam Laser", mount="Turret" } },
            { "hpt_cannon_fixed_small", new Hardpoint { @class=1, rating="D", group="Cannon", mount="Fixed" } },
            { "hpt_cannon_fixed_medium", new Hardpoint { @class=2, rating="D", group="Cannon", mount="Fixed" } },
            { "hpt_cannon_fixed_large", new Hardpoint { @class=3, rating="C", group="Cannon", mount="Fixed" } },
            { "hpt_cannon_fixed_huge", new Hardpoint { @class=4, rating="B", group="Cannon", mount="Fixed" } },
            { "hpt_cannon_gimbal_small", new Hardpoint { @class=1, rating="E", group="Cannon", mount="Gimballed" } },
            { "hpt_cannon_gimbal_medium", new Hardpoint { @class=2, rating="D", group="Cannon", mount="Gimballed" } },
            { "hpt_cannon_gimbal_large", new Hardpoint { @class=3, rating="C", group="Cannon", mount="Gimballed" } },
            { "hpt_cannon_gimbal_huge", new Hardpoint { @class=4, rating="B", group="Cannon", mount="Gimballed" } },
            { "hpt_cannon_turret_small", new Hardpoint { @class=1, rating="F", group="Cannon", mount="Turret" } },
            { "hpt_cannon_turret_medium", new Hardpoint { @class=2, rating="E", group="Cannon", mount="Turret" } },
            { "hpt_cannon_turret_large", new Hardpoint { @class=3, rating="D", group="Cannon", mount="Turret" } },
            { "hpt_drunkmissilerack_fixed_medium", new Hardpoint { @class=2, rating="B", group="Pack-Hound Missle Rack", mount="Fixed" } },
            { "hpt_dumbfiremissilerack_fixed_small", new Hardpoint { @class=1, rating="B", group="Missle Rack", mount="Fixed" } },
            { "hpt_dumbfiremissilerack_fixed_medium", new Hardpoint { @class=2, rating="B", group="Missle Rack", mount="Fixed" } },
            { "hpt_minelauncher_fixed_small", new Hardpoint { @class=1, rating="I", group="Mine Launcher", mount="Fixed" } },
            { "hpt_minelauncher_fixed_medium", new Hardpoint { @class=2, rating="I", group="Mine Launcher", mount="Fixed" } },
            { "hpt_mininglaser_fixed_small", new Hardpoint { @class=1, rating="D", group="Mining Laser", mount="Fixed" } },
            { "hpt_mininglaser_fixed_medium", new Hardpoint { @class=2, rating="D", group="Multi-Cannon", mount="Fixed" } },
            { "hpt_multicannon_fixed_small", new Hardpoint { @class=1, rating="F", group="Multi-Cannon", mount="Fixed" } },
            { "hpt_multicannon_fixed_medium", new Hardpoint { @class=2, rating="E", group="Multi-Cannon", mount="Fixed" } },
            { "hpt_multicannon_gimbal_small", new Hardpoint { @class=1, rating="G", group="Multi-Cannon", mount="Gimballed" } },
            { "hpt_multicannon_gimbal_medium", new Hardpoint { @class=2, rating="F", group="Multi-Cannon", mount="Gimballed" } },
            { "hpt_multicannon_turret_small", new Hardpoint { @class=1, rating="G", group="Multi-Cannon", mount="Turret" } },
            { "hpt_multicannon_turret_medium", new Hardpoint { @class=2, rating="F", group="Multi-Cannon", mount="Turret" } },
            { "hpt_plasmaaccelerator_fixed_medium", new Hardpoint { @class=2, rating="C", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_plasmaaccelerator_fixed_large", new Hardpoint { @class=3, rating="B", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_plasmaaccelerator_fixed_huge", new Hardpoint { @class=4, rating="A", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_pulselaser_fixed_small", new Hardpoint { @class=1, rating="F", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_fixed_medium", new Hardpoint { @class=2, rating="E", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_fixed_large", new Hardpoint { @class=3, rating="D", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_gimbal_small", new Hardpoint { @class=1, rating="G", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_gimbal_medium", new Hardpoint { @class=2, rating="F", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_gimbal_large", new Hardpoint { @class=3, rating="E", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_turret_small", new Hardpoint { @class=1, rating="G", group="Pulse Laser", mount="Turret" } },
            { "hpt_pulselaser_turret_medium", new Hardpoint { @class=2, rating="F", group="Pulse Laser", mount="Turret" } },
            { "hpt_pulselaser_turret_large", new Hardpoint { @class=3, rating="F", group="Pulse Laser", mount="Turret" } },
            { "hpt_pulselaserburst_fixed_small", new Hardpoint { @class=1, rating="F", group="Burst Laser", mount="Fixed" } },
            { "hpt_pulselaserburst_fixed_medium", new Hardpoint { @class=2, rating="E", group="Burst Laser", mount="Fixed" } },
            { "hpt_pulselaserburst_fixed_large", new Hardpoint { @class=3, rating="D", group="Burst Laser", mount="Fixed" } },
            { "hpt_pulselaserburst_gimbal_small", new Hardpoint { @class=1, rating="G", group="Burst Laser", mount="Gimballed" } },
            { "hpt_pulselaserburst_gimbal_medium", new Hardpoint { @class=2, rating="F", group="Burst Laser", mount="Gimballed" } },
            { "hpt_pulselaserburst_gimbal_large", new Hardpoint { @class=3, rating="E", group="Burst Laser", mount="Gimballed" } },
            { "hpt_pulselaserburst_turret_small", new Hardpoint { @class=1, rating="G", group="Burst Laser", mount="Turret" } },
            { "hpt_pulselaserburst_turret_medium", new Hardpoint { @class=2, rating="F", group="Burst Laser", mount="Turret" } },
            { "hpt_pulselaserburst_turret_large", new Hardpoint { @class=3, rating="E", group="Burst Laser", mount="Turret" } },
            { "hpt_railgun_fixed_small", new Hardpoint { @class=1, rating="D", group="Rail Gun", mount="Fixed" } },
            { "hpt_railgun_fixed_medium", new Hardpoint { @class=2, rating="B", group="Rail Gun", mount="Fixed" } },
            { "hpt_slugshot_fixed_small", new Hardpoint { @class=1, rating="E", group="Fragment Cannon", mount="Fixed" } },
            { "hpt_slugshot_fixed_medium", new Hardpoint { @class=2, rating="A", group="Fragment Cannon", mount="Fixed" } },
            { "hpt_slugshot_fixed_large", new Hardpoint { @class=3, rating="C", group="Fragment Cannon", mount="Fixed" } },
            { "hpt_slugshot_gimbal_small", new Hardpoint { @class=1, rating="E", group="Fragment Cannon", mount="Gimballed" } },
            { "hpt_slugshot_gimbal_medium", new Hardpoint { @class=2, rating="D", group="Fragment Cannon", mount="Gimballed" } },
            { "hpt_slugshot_gimbal_large", new Hardpoint { @class=3, rating="C", group="Fragment Cannon", mount="Gimballed" } },
            { "hpt_slugshot_turret_small", new Hardpoint { @class=1, rating="E", group="Fragment Cannon", mount="Turret" } },
            { "hpt_slugshot_turret_medium", new Hardpoint { @class=2, rating="D", group="Fragment Cannon", mount="Turret" } },
            { "hpt_slugshot_turret_large", new Hardpoint { @class=3, rating="C", group="Fragment Cannon", mount="Turret" } },
        }; // XXX Add Advanced Plasma Accelerator, Cytoscrambler Burst Laser, Imperial Hammer Rail Gun, Pacifier Frag-Cannon, Mining Lance Beam Laser, Enforcer Cannon

        Hardpoint newHardpoint = new Hardpoint();
        string currName = currModule["name"].ToLower();
        
        foreach (string currWeapon in weaponMap.Keys)
        {
            if (currName.Contains(currWeapon))
            {
                newHardpoint.group = weaponMap[currWeapon].group;
                newHardpoint.rating = weaponMap[currWeapon].rating;
                newHardpoint.@class = weaponMap[currWeapon].@class;
                newHardpoint.mount = weaponMap[currWeapon].mount;
                newHardpoint.enabled = currModule["on"];
                newHardpoint.priority = mapPriority(currModule["priority"]);
                coriolis.components.hardpoints.Add(newHardpoint);
                return;
            }
        }
        Debug.Write("Error:  Unable to map to Coriolis hardpoint: " + currModule["name"]);
    }

    private static void addUtility(Dictionary<string, dynamic> currModule, ref RootObject coriolis)
    {
        Dictionary<string, string> mapCountermeasure = new Dictionary<string, string>
        {
            { "chafflauncher", "Chaff Launcher" },
            { "electroniccountermeasure", "Electronic Countermeasure" },
            { "heatsinklauncher", "Heat Sink Launcher" },
            { "plasmapointdefence", "Point Defence" }
        };
        Dictionary<string, string> mapUtility = new Dictionary<string, string>
        {
            { "cargoscanner", "Cargo Scanner" },
            { "cloudscanner", "Frame Shift Wake Scanner" },
            { "crimescanner", "Kill Warrant Scanner" },
            { "shieldbooster", "Shield Booster" },
        };

        Utility newUtility = new Utility();
        string currName = currModule["name"].ToLower();

        newUtility.@class = mapClass(currName);
        newUtility.rating = mapRating(currName);
        newUtility.priority = mapPriority(currModule["priority"]);
        newUtility.enabled = currModule["on"];

        foreach (string currCM in mapCountermeasure.Keys)
        {
            if (currName.Contains(currCM))
            {
                newUtility.name = mapCountermeasure[currCM];
                newUtility.group = "Countermeasure";
                coriolis.components.utility.Add(newUtility);
                return;
            }
        }

        foreach (string currUtility in mapUtility.Keys)
        {
            if (currName.Contains(currUtility))
            {
                newUtility.group = mapUtility[currUtility];
                coriolis.components.utility.Add(newUtility);
                return;
            }
        }
        Debug.Write("Error:  Unable to map to Coriolis utility: " + currModule["name"]);
    }

    private static void addInternal(Dictionary<string, dynamic> currModule, ref RootObject coriolis)
    {
        Dictionary<string, string> mapInternalName = new Dictionary<string, string>
        {
            { "buggybay", "Planetary Vehicle Hangar" },
            { "cargorack", "Cargo Rack" },
            { "collection", "Collector Limpet Controller" },
            { "fsdinterdictor", "Frame Shift Drive Interdictor" },
            { "fuelscoop", "Fuel Scoop" },
            { "fueltransfer", "Fuel Transfer Limpet Controller" },
            { "hullreinforcement", "Hull Reinforcement Package" },
            { "prospector", "Prospector Limpet Controller" },
            { "refinery", "Refinery" },
            { "repairer", "Auto Field-Maintenance Unit" },
            { "resourcesiphon", "Hatch Breaker Limpet Controller" },
            { "shieldcellbank", "Shield Cell Bank" },
            { "shieldgenerator", "Shield Generator" },
            { "dockingcomputer", "Standard Docking Computer" }

        }; // XXX add Bi-Weave, Prismatic

        Dictionary<string, string> mapScanner = new Dictionary<string, string>
        {
            { "stellarbodydiscoveryscanner_advanced", "Advanced Discovery Scanner" },
            { "stellarbodydiscoveryscanner_intermediate", "Intermediate Discovery Scanner" },
            { "stellarbodydiscoveryscanner_standard", "Basic Discovery Scanner" },
            { "detailedsurfacescanner", "Detailed Surface Scanner" }
        };

        Internal newInternal = new Internal();

        string currName = currModule["name"].ToLower();
        newInternal.rating = mapRating(currName);
        newInternal.@class = mapClass(currName);
        newInternal.priority = mapPriority(currModule["priority"]);
        newInternal.enabled = currModule["on"];

        foreach (string currInternal in mapInternalName.Keys)
        {
            if (currName.Contains(currInternal))
            {
                newInternal.group = mapInternalName[currInternal];
                coriolis.components.@internal.Add(newInternal);
                return;
            }
        }

        foreach (string currScanner in mapScanner.Keys)
        {
            if (currName.Contains(currScanner))
            {
                newInternal.name = mapScanner[currScanner];
                newInternal.group = "Scanner";
                coriolis.components.@internal.Add(newInternal);
                return;
            }
        }
        Debug.Write("Error:  Unable to map to Coriolis internal: " + currModule["name"]);
    }

    private static void addStandard(Dictionary<string, dynamic> currModule, ref RootObject coriolis)
    {
        string currName = currModule["name"].ToLower();

        if (currName.Contains("engine"))
        {
            Thrusters newThrusters = new Thrusters();
            newThrusters.@class = mapClass(currName);
            newThrusters.rating = mapRating(currName);
            newThrusters.priority = mapPriority(currModule["priority"]);
            newThrusters.enabled = currModule["on"];
            coriolis.components.standard.thrusters = newThrusters;
        }
        else if (currName.Contains("fueltank"))
        {
            FuelTank newFuelTank = new FuelTank();
            newFuelTank.@class = mapClass(currName);
            newFuelTank.rating = mapRating(currName);
            newFuelTank.priority = mapPriority(currModule["priority"]);
            newFuelTank.enabled = currModule["on"];
            coriolis.components.standard.fuelTank = newFuelTank;
        }
        else if (currName.Contains("hyperdrive"))
        {
            FrameShiftDrive newFrameShiftDrive = new FrameShiftDrive();
            newFrameShiftDrive.@class = mapClass(currName);
            newFrameShiftDrive.rating = mapRating(currName);
            newFrameShiftDrive.priority = mapPriority(currModule["priority"]);
            newFrameShiftDrive.enabled = currModule["on"];
            coriolis.components.standard.frameShiftDrive = newFrameShiftDrive;
        }
        else if (currName.Contains("lifesupport"))
        {
            LifeSupport newLifeSupport = new LifeSupport();
            newLifeSupport.@class = mapClass(currName);
            newLifeSupport.rating = mapRating(currName);
            newLifeSupport.priority = mapPriority(currModule["priority"]);
            newLifeSupport.enabled = currModule["on"];
            coriolis.components.standard.lifeSupport = newLifeSupport;
        }
        else if (currName.Contains("powerdistributor"))
        {
            PowerDistributor newPowerDistributor = new PowerDistributor();
            newPowerDistributor.@class = mapClass(currName);
            newPowerDistributor.rating = mapRating(currName);
            newPowerDistributor.priority = mapPriority(currModule["priority"]);
            newPowerDistributor.enabled = currModule["on"];
            coriolis.components.standard.powerDistributor = newPowerDistributor;
        }
        else if (currName.Contains("powerplant"))
        {
            PowerPlant newPowerPlant = new PowerPlant();
            newPowerPlant.@class = mapClass(currName);
            newPowerPlant.rating = mapRating(currName);
            newPowerPlant.priority = mapPriority(currModule["priority"]);
            newPowerPlant.enabled = currModule["on"];
            coriolis.components.standard.powerPlant = newPowerPlant;
        }
        else if (currName.Contains("sensors"))
        {
            Sensors newSensors = new Sensors();
            newSensors.@class = mapClass(currName);
            newSensors.rating = mapRating(currName);
            newSensors.priority = mapPriority(currModule["priority"]);
            newSensors.enabled = currModule["on"];
            coriolis.components.standard.sensors = newSensors;
        }
        else if (currName.Contains("armour"))
        {
            coriolis.components.standard.bulkheads = mapBulkhead(currName);
        }
        //  Ignopre the rest (decals, paint, bobble heads etc.)
    }

    private static string mapBulkhead(string currName)
    {
        Dictionary<string, string> dictBulkhead = new Dictionary<string, string>
        {
            { "grade1", "Lightweight Alloy" },
            { "grade2", "Reinforced Allow" },
            { "grade3", "Military Grade Composite" },
            { "mirrored", "Mirrored Surface Composite" },
            { "reactive", "Reactive Surface Composite" }
        };

        foreach (string currBulkhead in dictBulkhead.Keys)
        {
            if (currName.Contains(currBulkhead))
            {
                return dictBulkhead[currBulkhead];
            }
        }

        Debug.Write("Error:  Unable to determine Coriolis bulkhead for:  " + currName);
        return null;
    }

    private static string mapRating(string moduleName)
    {
        Dictionary<string, string> dictRating = new Dictionary<string, string>
        {
            { "class5", "A" },
            { "class4", "B" },
            { "class3", "C" },
            { "class2", "D" },
            { "class1", "E" },
            { "stellarbodydiscoveryscanner_advanced", "C" },
            { "stellarbodydiscoveryscanner_intermediate", "D" },
            { "stellarbodydiscoveryscanner_standard", "E" },
            { "detailedsurfacescanner", "C" },
            { "chafflauncher", "I" },
            { "electroniccountermeasure", "F" },
            { "heatsinklauncher", "I" },
            { "plasmapointdefence", "I" },
            { "dockingcomputer", "E" }
        };

        // Planetary Hangars break the normal rules
        if (moduleName.Contains("buggybay"))
        {
            if (moduleName.Contains("class1"))
            {
                return "H";
            }
            else if (moduleName.Contains("class2"))
            {
                return "G";
            }
        }

        foreach (string currRating in dictRating.Keys)
        {
            if (moduleName.Contains(currRating))
            {
                return dictRating[currRating];
            }
        }

        Debug.Write("Error:  Unable to determine Coriolis Rating for:  " + moduleName);
        return null;
    }

    private static int mapPriority(int priorityIn)
    {
        priorityIn++;
        return priorityIn;
    }

    private static int mapClass(string moduleName)
    {
        Dictionary<string, int> dictClass = new Dictionary<string, int>
        {
            { "size0", 0 },
            { "size1", 1 },
            { "size2", 2 },
            { "size3", 3 },
            { "size4", 4 },
            { "size5", 5 },
            { "size6", 6 },
            { "size7", 7 },
            { "size8", 8 },
            { "stellarbodydiscoveryscanner", 1 },
            { "detailedsurfacescanner", 1 },
            { "heatsinklauncher", 0 },
            { "chafflauncher", 0 },
            { "plasmapointdefence", 0 }
        };

        foreach (string currClass in dictClass.Keys)
        {
            if (moduleName.Contains(currClass))
            {
                return dictClass[currClass];
            }
        }

        Debug.Write("Error:  Unable to determine Coriolis Class for:  " + moduleName);
        return -1;
    }

    public static string createCoriolisJson(ref Dictionary<string, object> state)
    {
        if (!state.ContainsKey("VAEDcompanionDict"))
        {
            return null;
        }

        Dictionary<string, dynamic> companion = (Dictionary<string, dynamic>)state["VAEDcompanionDict"];
        RootObject coriolis = new RootObject();

        if (!companion.ContainsKey("ship") && !companion["ship"].ContainsKey("modules"))
        {
            Debug.Write("Companion JSON missing ship information");
            return null;
        }

        int shipId = companion["commander"]["currentShipId"];
        string currentShipId = shipId.ToString();
        string currentShip = companion["ships"][currentShipId]["name"];

        string coriolisShipName = mapShip(currentShip);

        coriolis.name = coriolisShipName;
        coriolis.ship = coriolisShipName;

        // Cargo hatch isn't in the companion API
        CargoHatch newCargoHatch = new CargoHatch();
        newCargoHatch.enabled = true;
        newCargoHatch.priority = 5;

        coriolis.components.standard.cargoHatch = newCargoHatch;

        foreach (KeyValuePair<string, dynamic> currModule in companion["ship"]["modules"])
        {
            string currMapping = ModuleMap(currModule.Key);
            Dictionary<string, dynamic> mod = null;
            switch (currMapping)
            {
                case "hardpoints":
                    mod = currModule.Value["module"];
                    addHardpoint(mod, ref coriolis);
                    break;
                case "utility":
                    mod = currModule.Value["module"];
                    addUtility(mod, ref coriolis);
                    break;
                case "internal":
                    mod = currModule.Value["module"];
                    addInternal(mod, ref coriolis);
                    break;
                case "standard":
                    mod = currModule.Value["module"];
                    addStandard(mod, ref coriolis);
                    break;
            }
        }

        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));
        MemoryStream stream = new MemoryStream();
        serializer.WriteObject(stream, coriolis);
        stream.Position = 0;
        StreamReader sr = new StreamReader(stream);
        string json = sr.ReadToEnd();
        return json;
    }
}