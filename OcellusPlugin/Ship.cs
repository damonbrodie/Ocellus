using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization;

class Ship
{
    [DataContractAttribute]
    public class CargoHatch
    {
        [DataMemberAttribute(Order = 1)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 2)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerPlant
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Thrusters
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FrameShiftDrive
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class LifeSupport
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class PowerDistributor
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class Sensors
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
    }

    [DataContractAttribute]
    public class FuelTank
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
        public int capacity { get; set; }
    }

    [DataContractAttribute]
    public class Standard
    {
        [DataMemberAttribute(Order = 1)]
        public string bulkheads { get; set; }
        [DataMemberAttribute(Order = 2)]
        public CargoHatch cargoHatch { get; set; }
        [DataMemberAttribute(Order = 3)]
        public PowerPlant powerPlant { get; set; }
        [DataMemberAttribute(Order = 4)]
        public Thrusters thrusters { get; set; }
        [DataMemberAttribute(Order = 5)]
        public FrameShiftDrive frameShiftDrive { get; set; }
        [DataMemberAttribute(Order = 6)]
        public LifeSupport lifeSupport { get; set; }
        [DataMemberAttribute(Order = 7)]
        public PowerDistributor powerDistributor { get; set; }
        [DataMemberAttribute(Order = 8)]
        public Sensors sensors { get; set; }
        [DataMemberAttribute(Order = 9)]
        public FuelTank fuelTank { get; set; }
    }

    [DataContractAttribute]
    public class Hardpoint
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute(Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string name { get; set; }
        [DataMemberAttribute(Order = 7)]
        public string mount { get; set; }
        public int slotOrder { get; set; }
        public string slotSize { get; set; }
    }

    [DataContractAttribute]
    public class Utility
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute(Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string name { get; set; }
        public int slot { get; set; }
    }

    [DataContractAttribute]
    public class Internal
    {
        [DataMemberAttribute(Order = 1)]
        public int @class { get; set; }
        [DataMemberAttribute(Order = 2)]
        public string rating { get; set; }
        [DataMemberAttribute(Order = 3)]
        public bool enabled { get; set; }
        [DataMemberAttribute(Order = 4)]
        public int priority { get; set; }
        [DataMemberAttribute(Order = 5)]
        public string group { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string name { get; set; }
        public int slot { get; set; }
        public int slotSize { get; set; }
        public int capacity { get; set; }
    }

    [DataContract]
    public class Attributes
    {
        [DataMember(Order = 1)]
        public string shiptype { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 2)]
        public bool hasCargoScanner { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 3)]
        public bool hasFrameShiftWakeScanner { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 4)]
        public bool hasKillWarrantScanner { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 5)]
        public bool hasShieldBooster { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 6)]
        public bool hasChaffLauncher { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 7)]
        public bool hasElectronicCountermeasures { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 8)]
        public bool hasHeatSinkLauncher { get; set; }
        [DataMemberAttribute(EmitDefaultValue = false, Order = 9)]
        public bool hasPointDefence { get; set; }
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
        [DataMemberAttribute(Order = 4)]
        public List<Internal> @internal = new List<Internal>();
        public Attributes attributes = new Attributes();
    }

    private static void addHardpoint(string slotSize, int slotPos, Dictionary<string, dynamic> currModule, ref Components shipObj)
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
            { "hpt_beamlaser_fixed_huge", new Hardpoint { @class=4, rating="A", group="Beam Laser", mount="Fixed" } },
            { "hpt_beamlaser_gimbal_small", new Hardpoint { @class=1, rating="E", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_gimbal_medium", new Hardpoint { @class=2, rating="D", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_gimbal_large", new Hardpoint { @class=3, rating="C", group="Beam Laser", mount="Gimballed" } },
            { "hpt_beamlaser_gimbal_huge", new Hardpoint { @class=4, rating="A", group="Beam Laser", mount="Gimballed" } },
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
            { "hpt_mininglaser_fixed_medium", new Hardpoint { @class=2, rating="D", group="Mining Laser", mount="Fixed" } },
            { "hpt_multicannon_fixed_small", new Hardpoint { @class=1, rating="F", group="Multi-cannon", mount="Fixed" } },
            { "hpt_multicannon_fixed_medium", new Hardpoint { @class=2, rating="E", group="Multi-cannon", mount="Fixed" } },
            { "hpt_multicannon_gimbal_small", new Hardpoint { @class=1, rating="G", group="Multi-cannon", mount="Gimballed" } },
            { "hpt_multicannon_gimbal_medium", new Hardpoint { @class=2, rating="F", group="Multi-cannon", mount="Gimballed" } },
            { "hpt_multicannon_turret_small", new Hardpoint { @class=1, rating="G", group="Multi-cannon", mount="Turret" } },
            { "hpt_multicannon_turret_medium", new Hardpoint { @class=2, rating="F", group="Multi-cannon", mount="Turret" } },
            { "hpt_plasmaaccelerator_fixed_medium", new Hardpoint { @class=2, rating="C", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_plasmaaccelerator_fixed_large", new Hardpoint { @class=3, rating="B", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_plasmaaccelerator_fixed_huge", new Hardpoint { @class=4, rating="A", group="Plasma Accelerator", mount="Fixed" } },
            { "hpt_pulselaser_fixed_small", new Hardpoint { @class=1, rating="F", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_fixed_medium", new Hardpoint { @class=2, rating="E", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_fixed_large", new Hardpoint { @class=3, rating="D", group="Pulse Laser", mount="Fixed" } },
            { "hpt_pulselaser_fixed_huge", new Hardpoint { @class=4, rating="A", group="Beam Laser", mount="Fixed" } },
            { "hpt_pulselaser_gimbal_small", new Hardpoint { @class=1, rating="G", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_gimbal_medium", new Hardpoint { @class=2, rating="F", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_gimbal_large", new Hardpoint { @class=3, rating="E", group="Pulse Laser", mount="Gimballed" } },
            { "hpt_pulselaser_gimbal_huge", new Hardpoint { @class=4, rating="A", group="Beam Laser", mount="Gimballed" } },
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
            { "hpt_railgun_fixed_medium_burst", new Hardpoint { @class=2, rating="E", group="Imperial Hammer", mount="Fixed" } },
            { "hpt_xxx1", new Hardpoint { @class=3, rating="C", group="Fragment Cannon", mount="Fixed", name="Pacifier" } },
            { "hpt_xxx2", new Hardpoint { @class=1, rating="E", group="Beam Laser", mount="Fixed", name="Retributor" } },
            { "hpt_xxx3", new Hardpoint { @class=1, rating="F", group="Multi-cannon", mount="Fixed", name="Enforder" } },
            { "hpt_xxx4", new Hardpoint { @class=2, rating="C", group="Pulse Laser", mount="Fixed", name="Disruptor" } },
            { "hpt_xxx5", new Hardpoint { @class=2, rating="B", group="Missle Rack", mount="Fixed", name="Pack-Hound" } },
            { "hpt_xxx6", new Hardpoint { @class=1, rating="I", group="Mine Launcher", mount="Fixed", name="Shock Mine Launcher" } },
            { "hpt_xxx7", new Hardpoint { @class=1, rating="F", group="Burst Laser", mount="Fixed", name="Cytoscrambler" } },
            { "hpt_xxx8", new Hardpoint { @class=1, rating="D", group="Mining Laser", mount="Fixed", name="Mining Lance" } },
        }; // XXX Add Advanced Plasma Accelerator, Cytoscrambler Burst Laser, Pacifier Frag-Cannon, Mining Lance Beam Laser, Enforcer Cannon

        Hardpoint newHardpoint = new Hardpoint();
        newHardpoint.slotSize = slotSize;
        newHardpoint.slotOrder = slotPos;
        if (currModule == null)
        {
            shipObj.hardpoints.Add(newHardpoint);
            return;
        }
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
                newHardpoint.name = weaponMap[currWeapon].name;
                newHardpoint.priority = mapPriority(currModule["priority"]);
                shipObj.hardpoints.Add(newHardpoint);
                return;
            }
        }
        Debug.Write("Error:  Unable to map to ShipObj hardpoint: " + currModule["name"]);
    }

    private static int mapFuelCapacity(int @class)
    {
        switch (@class)
        {
            case (8):
                return 256;
            case (7):
                return 128;
            case (6):
                return 64;
            case (5):
                return 32;
            case (4):
                return 16;
            case (3):
                return 8;
            case (2):
                return 4;
            case (1):
                return 2;
            default:
                Debug.Write("Error:  Unable to map fuel tank capacity");
                return 0;
        }
    }

    private static int mapCargoCapacity(int @class)
    {
        switch (@class)
        {
            case (8):
                return 256;
            case (7):
                return 128;
            case (6):
                return 64;
            case (5):
                return 32;
            case (4):
                return 16;
            case (3):
                return 8;
            case (2):
                return 4;
            case (1):
                return 2;
            default:
                Debug.Write("Error:  Unable to map cargo rack capacity");
                return 0;
        }
    }

    private static void addUtility(int slotPos, Dictionary<string, dynamic> currModule, ref Components shipObj)
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
        newUtility.slot = slotPos;
        if (currModule == null)
        {
            shipObj.utility.Add(newUtility);
            return;
        }
        else 
        {
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
                    shipObj.utility.Add(newUtility);
                    switch (newUtility.name)
                    {
                        case "Chaff Launcher":
                            shipObj.attributes.hasChaffLauncher = true;
                            break;
                        case "Electronic Countermeasure":
                            shipObj.attributes.hasElectronicCountermeasures = true;
                            break;
                        case "Heat Sink Launcher":
                            shipObj.attributes.hasHeatSinkLauncher = true;
                            break;
                        case "Point Defence":
                            shipObj.attributes.hasPointDefence = true;
                            break;
                        default:
                            Debug.Write("ERROR: Unknown countermeasure");
                            break;
                    }
                    return;
                }
            }

            foreach (string currUtility in mapUtility.Keys)
            {
                if (currName.Contains(currUtility))
                {
                    newUtility.group = mapUtility[currUtility];
                    shipObj.utility.Add(newUtility);
                    switch (newUtility.name)
                    {
                        case "Cargo Scanner":
                            shipObj.attributes.hasCargoScanner = true;
                            break;
                        case "Frame Shift Wake Scanner":
                            shipObj.attributes.hasFrameShiftWakeScanner = true;
                            break;
                        case "Kill Warrant Scanner":
                            shipObj.attributes.hasKillWarrantScanner = true;
                            break;
                        case "Shield Booster":
                            shipObj.attributes.hasShieldBooster = true;
                            break;
                    }
                    return;
                }
            }
            Debug.Write("Error:  Unable to map to ShipObj utility: " + currModule["name"]);
        }

    }

    private static void addInternal(int slot, int slotSize, Dictionary<string, dynamic> currModule, ref Components shipObj)
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
        newInternal.slot = slot;
        newInternal.slotSize = slotSize;
        if (currModule == null)
        {
            shipObj.@internal.Add(newInternal);
            return;
        }
        else
        {
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
                    if (newInternal.group == "Fuel Tank")
                    {
                        newInternal.capacity = mapFuelCapacity(newInternal.@class);
                    }
                    else if (newInternal.group == "Cargo Rack")
                    {
                        newInternal.capacity = mapCargoCapacity(newInternal.@class);
                    }
                    shipObj.@internal.Add(newInternal);
                    return;
                }
            }

            foreach (string currScanner in mapScanner.Keys)
            {
                if (currName.Contains(currScanner))
                {
                    newInternal.name = mapScanner[currScanner];
                    newInternal.group = "Scanner";
                    shipObj.@internal.Add(newInternal);
                    return;
                }
            }
            Debug.Write("Error:  Unable to map to ShipObj internal: " + currModule["name"]);
        }
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

        Debug.Write("Error:  Unable to determine ShipObj bulkhead for:  " + currName);
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

        Debug.Write("Error:  Unable to determine ShipObj Rating for:  " + moduleName);
        return null;
    }

    private static int mapPriority(int priorityIn)
    {
        // Increase by one (API is zero based);
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
            { "plasmapointdefence", 0 },
            { "electroniccountermeasure", 0 }
        };

        foreach (string currClass in dictClass.Keys)
        {
            if (moduleName.Contains(currClass))
            {
                return dictClass[currClass];
            }
        }

        Debug.Write("Error:  Unable to determine ShipObj Class for:  " + moduleName);
        return -1;
    }

    public static Components decode(Dictionary<string, dynamic> companion)
    {
        try
        {
            //RootObject shipObj = new RootObject();
            Components shipObj = new Components();

            if (!companion.ContainsKey("ship") && !companion["ship"].ContainsKey("modules"))
            {
                Debug.Write("Companion JSON is missing ship information");
                return null;
            }

            int shipId = companion["commander"]["currentShipId"];
            string currentShipId = shipId.ToString();
            string currentShip = companion["ships"][currentShipId]["name"];

            // Store in Frontier format, helper methods will expect that.
            shipObj.attributes.shiptype = currentShip;

            // Cargo hatch isn't in the companion API
            CargoHatch newCargoHatch = new CargoHatch();
            newCargoHatch.enabled = true;
            newCargoHatch.priority = 5;

            shipObj.standard.cargoHatch = newCargoHatch;

            foreach (KeyValuePair<string, dynamic> currModule in companion["ship"]["modules"])
            {
                Dictionary<string, dynamic> mod;
                string moduleName = "";
                if (currModule.Value.GetType() == typeof(System.Collections.ArrayList))
                {
                    // Companion returns an empty array of there is nothing in the slot
                    mod = null;
                }
                else
                {
                    mod = currModule.Value["module"];
                    moduleName = mod["name"].ToLower();
                }
                if (currModule.Key.Contains("Armour"))
                {
                    shipObj.standard.bulkheads = mapBulkhead(moduleName);
                }
                else if (currModule.Key.Contains("PowerPlant"))
                {
                    PowerPlant newPowerPlant = new PowerPlant();
                    newPowerPlant.@class = mapClass(moduleName);
                    newPowerPlant.rating = mapRating(moduleName);
                    newPowerPlant.priority = 1;
                    newPowerPlant.enabled = mod["on"];
                    shipObj.standard.powerPlant = newPowerPlant;
                }
                else if (currModule.Key.Contains("MainEngines"))
                {
                    Thrusters newThrusters = new Thrusters();
                    newThrusters.@class = mapClass(moduleName);
                    newThrusters.rating = mapRating(moduleName);
                    newThrusters.priority = mapPriority(mod["priority"]);
                    newThrusters.enabled = mod["on"];
                    shipObj.standard.thrusters = newThrusters;
                }
                else if (currModule.Key.Contains("FrameShiftDrive"))
                {
                    FrameShiftDrive newFrameShiftDrive = new FrameShiftDrive();
                    newFrameShiftDrive.@class = mapClass(moduleName);
                    newFrameShiftDrive.rating = mapRating(moduleName);
                    newFrameShiftDrive.priority = mapPriority(mod["priority"]);
                    newFrameShiftDrive.enabled = mod["on"];
                    shipObj.standard.frameShiftDrive = newFrameShiftDrive;
                }
                else if (currModule.Key.Contains("LifeSupport"))
                {
                    LifeSupport newLifeSupport = new LifeSupport();
                    newLifeSupport.@class = mapClass(moduleName);
                    newLifeSupport.rating = mapRating(moduleName);
                    newLifeSupport.priority = mapPriority(mod["priority"]);
                    newLifeSupport.enabled = mod["on"];
                    shipObj.standard.lifeSupport = newLifeSupport;
                }
                else if (currModule.Key.Contains("PowerDistributor"))
                {
                    PowerDistributor newPowerDistributor = new PowerDistributor();
                    newPowerDistributor.@class = mapClass(moduleName);
                    newPowerDistributor.rating = mapRating(moduleName);
                    newPowerDistributor.priority = mapPriority(mod["priority"]);
                    newPowerDistributor.enabled = mod["on"];
                    shipObj.standard.powerDistributor = newPowerDistributor;
                }
                else if (currModule.Key.Contains("Radar"))
                {
                    Sensors newSensors = new Sensors();
                    newSensors.@class = mapClass(moduleName);
                    newSensors.rating = mapRating(moduleName);
                    newSensors.priority = mapPriority(mod["priority"]);
                    newSensors.enabled = mod["on"];
                    shipObj.standard.sensors = newSensors;
                }
                else if (currModule.Key.Contains("FuelTank"))
                {
                    FuelTank newFuelTank = new FuelTank();
                    newFuelTank.@class = mapClass(moduleName);
                    newFuelTank.rating = mapRating(moduleName);
                    newFuelTank.priority = mapPriority(mod["priority"]);
                    newFuelTank.enabled = mod["on"];
                    newFuelTank.capacity = mapFuelCapacity(newFuelTank.@class);
                    shipObj.standard.fuelTank = newFuelTank;

                }
                else if (currModule.Key.StartsWith("Slot"))
                {
                    Regex re = new Regex(@"Slot([0-9]+)_Size([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        int slotSize = Int32.Parse(m.Groups[2].ToString());
                        addInternal(slotPos, slotSize, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                    
                }
                else if (currModule.Key.StartsWith("Tiny"))
                {
                    Regex re = new Regex(@"[^0-9]*([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        addUtility(slotPos, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                }
                if (currModule.Key.StartsWith("Huge"))
                {
                    Regex re = new Regex(@"[^0-9]*([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        addHardpoint("Huge", slotPos, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                }
                else if (currModule.Key.StartsWith("Large"))
                {
                    Regex re = new Regex(@"[^0-9]*([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        addHardpoint("Large", slotPos, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                }
                else if (currModule.Key.StartsWith("Medium"))
                {
                    Regex re = new Regex(@"[^0-9]*([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        addHardpoint("Medium", slotPos, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                }
                else if (currModule.Key.StartsWith("Small"))
                {
                    Regex re = new Regex(@"[^0-9]*([0-9]+)");
                    Match m = re.Match(currModule.Key);
                    if (m.Success)
                    {
                        int slotPos = Int32.Parse(m.Groups[1].ToString());
                        addHardpoint("Small", slotPos, mod, ref shipObj);
                    }
                    else
                    {
                        Debug.Write("Error:  Unexpected module in Companion Output: " + currModule.Key);
                    }
                }

            }
            return shipObj;

        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
        return null;
    }
}
