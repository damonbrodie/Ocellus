using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


// **************************************
// *  Functions for doing EDDN updates  *
// **************************************
class Eddn
{
    [DataContractAttribute]
    public class Module
    {
        [DataMember(Order = 1)]
        public string category { get; set; }
        [DataMember(Order = 2)]
        public string rating { get; set; }
        [DataMember(Order = 3)]
        public string @class { get; set; }
        [DataMember(Order = 4)]
        public string name { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5)]
        public string mount { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6)]
        public string guidance { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7)]
        public string ship { get; set; }
    }

    [DataContractAttribute]
    public class Commodity
    {
        [DataMember(Order = 1)]
        public string name { get; set; }
        [DataMember(Order = 2)]
        public int buyPrice { get; set; }
        [DataMember(Order = 3)]
        public int supply { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4)]
        public string supplyLevel { get; set; }
        [DataMember(Order = 5)]
        public int sellPrice { get; set; }
        [DataMember(Order = 6)]
        public int demand { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7)]
        public string demandLevel { get; set; }
    }

   [DataContractAttribute]
    public class MessageCommodity
    {
        [DataMember(Order = 1)]
        public string systemName { get; set; }
        [DataMember(Order = 2)]
        public string stationName { get; set; }
        [DataMember(Order = 3)]
        public string timestamp { get; set; }
        [DataMemberAttribute(Order = 4)]
        public List<Commodity> commodities = new List<Commodity>();
    }

    [DataContractAttribute]
    public class MessageOutfitting
    {
        [DataMember(Order = 1)]
        public string systemName { get; set; }
        [DataMember(Order = 2)]
        public string stationName { get; set; }
        [DataMember(Order = 3)]
        public string timestamp { get; set; }
        [DataMemberAttribute(Order = 4)]
        public List<Module> modules = new List<Module>();
    }

    [DataContractAttribute]
    public class MessageShips
    {
        [DataMember(Order = 1)]
        public string systemName { get; set; }
        [DataMember(Order = 2)]
        public string stationName { get; set; }
        [DataMember(Order = 3)]
        public string timestamp { get; set; }
        [DataMember(Order = 4)]
        public List<string> ships = new List<string>();
    }

    [DataContractAttribute]
    public class Header
    {
        [DataMember(Order = 1)]
        public string uploaderID { get; set; }
        [DataMember(Order = 1)]
        public string softwareName = OcellusPlugin.OcellusPlugin.pluginName;
        [DataMember(Order = 2)]
        public string softwareVersion = OcellusPlugin.OcellusPlugin.pluginVersion;
    }

    [DataContract]
    public class RootObjectCommodities
    {
        [DataMember(Name = "$schemaRef", Order = 1)]
        public string schemaRef = "http://schemas.elite-markets.net/eddn/commodity/2";
        [DataMember(Order = 2)]
        public Header header = new Header();
        [DataMember(Order = 3)]
        public MessageCommodity message = new MessageCommodity();
    }

    [DataContract]
    public class RootObjectOutfitting
    {
        [DataMember(Name = "$schemaRef", Order = 1)]
        public string schemaRef = "http://schemas.elite-markets.net/eddn/outfitting/1";
        [DataMember(Order = 2)]
        public Header header = new Header();
        [DataMember(Order = 3)]
        public MessageOutfitting message = new MessageOutfitting();
    }

    [DataContract]
    public class RootObjectShips
    {
        [DataMember(Name = "$schemaRef", Order = 1)]
        public string schemaRef = "http://schemas.elite-markets.net/eddn/shipyard/1";
        [DataMember(Order = 2)]
        public Header header = new Header();
        [DataMember(Order = 3)]
        public MessageShips message = new MessageShips();
    }

    private const string uploadURL = "http://eddn-gateway.elite-markets.net:8080/upload/";

    private static string mapBracket(int bracket)
    {
        switch (bracket)
        {
            case 0:
                return null;
            case 1:
                return "Low";
            case 2:
                return "Med";
            case 3:
                return "High";
        }
        Debug.Write(@"Error:  Invalid demand/supply bracket from Companion API");
        return null;
    }

    private static Commodity extractCommodity(Dictionary<string, object> currCommodity)
    {
        Commodity newCommodity = new Commodity();
        newCommodity.name = currCommodity["name"].ToString();
        newCommodity.buyPrice = (int) currCommodity["buyPrice"];
        switch (currCommodity["stock"].GetType().ToString())
        {
            case "System.Int32":
                newCommodity.supply = (int)currCommodity["stock"];
                break;
            case "System.Decimal":
                newCommodity.supply =  Decimal.ToInt32((decimal)currCommodity["stock"]);
                break;
        }
        newCommodity.sellPrice = (int)currCommodity["sellPrice"];
        switch (currCommodity["demand"].GetType().ToString())
        {
            case "System.Int32":
                newCommodity.demand = (int)currCommodity["demand"];
                break;
            case "System.Decimal":
                newCommodity.demand = Decimal.ToInt32((decimal)currCommodity["demand"]);
                break;
        }
        newCommodity.buyPrice = (int)currCommodity["buyPrice"];
        if (currCommodity["demandBracket"].GetType() == typeof(string) && (string)currCommodity["demandBracket"] != "")
        {
            string demandLevel = mapBracket((int)currCommodity["demandBracket"]);
            if (demandLevel != null)
            {
                newCommodity.demandLevel = demandLevel;
            }
        }
        if (currCommodity["stockBracket"].GetType() == typeof(string) && (string)currCommodity["stockBracket"] != "")
        {
            string supplyLevel = mapBracket((int)currCommodity["stockBracket"]);
            if (supplyLevel != null)
            {
                newCommodity.supplyLevel = supplyLevel;
            }
        }
        return newCommodity;
    }

    private static Tuple<string, string, string> createEddnJson(Dictionary<string, dynamic> companion)
    {
        DateTime timestamp = DateTime.Now;
        string companionTime = timestamp.ToString("yyyy-MM-dd") + "T" + timestamp.ToString("H:m:szzz");

        bool isDocked = companion["commander"]["docked"];
        if (!isDocked)
        {
            Debug.Write("Not Docked - won't submit stale data to EDDN");
            return Tuple.Create<string, string, string>(null, null, null);
        }

        try
        {
            string system = companion["lastSystem"]["name"];
            string starport = companion["lastStarport"]["name"];

            RootObjectCommodities eddnCommodities = new RootObjectCommodities();
            RootObjectOutfitting eddnOutfitting = new RootObjectOutfitting();
            RootObjectShips eddnShips = new RootObjectShips();

            eddnCommodities.header.uploaderID = companion["commander"]["name"];
            eddnCommodities.message.systemName = system;
            eddnCommodities.message.stationName = starport;
            eddnCommodities.message.timestamp = companionTime;

            eddnOutfitting.header.uploaderID = companion["commander"]["name"];
            eddnOutfitting.message.systemName = system;
            eddnOutfitting.message.stationName = starport;
            eddnOutfitting.message.timestamp = companionTime;

            eddnShips.header.uploaderID = companion["commander"]["name"];
            eddnShips.message.systemName = system;
            eddnShips.message.stationName = starport;
            eddnShips.message.timestamp = companionTime;

            string commodityJson = null;
            string outfittingJson = null;
            string shipsJson = null;

            if (companion.ContainsKey("lastStarport"))
            {
                if (companion["lastStarport"].ContainsKey("commodities"))
                {
                    ArrayList commodities = companion["lastStarport"]["commodities"];

                    // We don't submit if there are no commodities here
                    if (commodities.Count > 0)
                    {
                        foreach (Dictionary<string, object> currCommodity in commodities)
                        {
                            Commodity toAdd = extractCommodity(currCommodity);
                            if (toAdd != null)
                            {
                                eddnCommodities.message.commodities.Add(toAdd);
                            }
                        }
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObjectCommodities));
                        MemoryStream stream = new MemoryStream();
                        serializer.WriteObject(stream, eddnCommodities);
                        stream.Position = 0;
                        StreamReader sr = new StreamReader(stream);
                        commodityJson = sr.ReadToEnd();
                    }
                }

                if (companion["lastStarport"].ContainsKey("modules"))
                {
                    Dictionary<string, dynamic> modules = companion["lastStarport"]["modules"];
                    List<string> keys = new List<string>(modules.Keys);

                    foreach (string key in keys)
                    {
                        Tuple<bool, Module> tResponse = decodeModule(key);
                        if (tResponse.Item1 == true)
                        {
                            eddnOutfitting.message.modules.Add(tResponse.Item2);
                        }
                    }
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObjectOutfitting));
                    MemoryStream stream = new MemoryStream();
                    serializer.WriteObject(stream, eddnOutfitting);
                    stream.Position = 0;
                    StreamReader sr = new StreamReader(stream);
                    outfittingJson = sr.ReadToEnd();
                }

                if (companion["lastStarport"].ContainsKey("ships") && companion["lastStarport"]["ships"].ContainsKey("shipyard_list"))
                {
                    Dictionary<string, dynamic> ships = companion["lastStarport"]["ships"]["shipyard_list"];
                    List<string> keys = new List<string>(ships.Keys);

                    foreach (string key in keys)
                    {
                        int shipId = companion["lastStarport"]["ships"]["shipyard_list"][key]["id"];
                        string toAdd = decodeShip(shipId);
                        if (toAdd != null)
                        {
                            eddnShips.message.ships.Add(toAdd);
                        }
                    }
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObjectShips));
                    MemoryStream stream = new MemoryStream();
                    serializer.WriteObject(stream, eddnShips);
                    stream.Position = 0;
                    StreamReader sr = new StreamReader(stream);
                    shipsJson = sr.ReadToEnd();
                }
            }

            return Tuple.Create<string, string, string>(commodityJson, outfittingJson, shipsJson);
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
        return null;
            
    }

    public static void updateEddn(Dictionary<string, dynamic> companion)
    {
        Tuple<string, string, string> tResponse = createEddnJson(companion);
        if (tResponse.Item3 != null)
        {
            Debug.Write("About to submit Shipyard to EDDN");
            Web.sendRequest(uploadURL, null, null, tResponse.Item3);
        }
        else
        {
            Debug.Write("No Shipyard");
        }
        if (tResponse.Item2 != null)
        {
            Debug.Write("About to submit Outfitting to EDDN");
            Web.sendRequest(uploadURL, null, null, tResponse.Item2);
        }
        else
        {
            Debug.Write("No Outfitting");
        }
        if (tResponse.Item1 != null)
        {
            Debug.Write("About to submit Commodities to EDDN");
            Web.sendRequest(uploadURL, null, null, tResponse.Item1);
        }
        else
        {
            Debug.Write("No Commodities");
        }
    }

    public static string decodeShip(int shipId)
    {
        string ship = null;
        switch (shipId)
        {
            case 128049267:
                ship = "Adder";
                break;
            case 128049363:
                ship = "Anaconda";
                break;
            case 128049303:
                ship = "Asp";
                break;
            case 128672276:
                ship = "Asp Scout";
                break;
            case 128049279:
                ship = "Cobra Mk III";
                break;
            case 128672262:
                ship = "Cobra MkIV";
                break;
            case 128671217:
                ship = "DiamondBack Scout";
                break;
            case 128671831:
                ship = "Diamondback Explorer";
                break;
            case 128049255:
                ship = "Eagle";
                break;
            case 128672145:
                ship = "Federal Assault Ship";
                break;
            case 128049369:
                ship = "Federal Corvette";
                break;
            case 128049321:
                ship = "Federal Dropship";
                break;
            case 128672152:
                ship = "Federal Gunship";
                break;
            case 128049351:
                ship = "Fer-de-Lance";
                break;
            case 128049261:
                ship = "Hauler";
                break;
            case 128049315:
                ship = "Imperial Clipper";
                break;
            case 128671223:
                ship = "Imperial Courier";
                break;
            case 128049375:
                ship = "Imperial Cutter";
                break;
            case 128672138:
                ship = "Imperial Eagle";
                break;
            case 128672269:
                ship = "Keelback";
                break;
            case 128049327:
                ship = "Orca";
                break;
            case 128049339:
                ship = "Python";
                break;
            case 128049249:
                ship = "Sidewinder";
                break;
            case 128049285:
                ship = "Type-6 Transporter";
                break;
            case 128049297:
                ship = "Type-7 Transporter";
                break;
            case 128049333:
                ship = "Type-9 Heavy";
                break;
            case 128049273:
                ship = "Viper";
                break;
            case 128672255:
                ship = "Viper MkIV";
                break;
            case 128049309:
                ship = "Vulture";
                break;
            default:
                ship = null;
                break;
        }
        return ship;
    }
    public static Tuple<bool, Module> decodeModule(string moduleId)
    {
        Module module = new Module();
        bool success = true;
        switch (moduleId)
        {

            // Beam Lasers
            case "128049430":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Beam Laser";
                break;
            case "128049434":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Beam Laser";
                break;
            case "128049437":
                module.@class = "3";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Beam Laser";
                break;
            case "128049429":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Beam Laser";
                break;
            case "128049433":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Beam Laser";
                break;
            case "128049436":
                module.@class = "2";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Beam Laser";
                break;
            case "128049428":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Beam Laser";
                break;
            case "128049432":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Beam Laser";
                break;
            case "128049435":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Beam Laser";
                break;
            case "XXX":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Retributor";
                break;

            // Burst Lasers
            case "128049402":
                module.@class = "3";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Burst Laser";
                break;
            case "128049406":
                module.@class = "3";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Burst Laser";
                break;
            case "128049409":
                module.@class = "3";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Burst Laser";
                break;
            case "128049401":
                module.@class = "2";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Burst Laser";
                break;
            case "128049405":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Burst Laser";
                break;
            case "128049408":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Burst Laser";
                break;
            case "128049400":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Burst Laser";
                break;
            case "128049404":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Burst Laser";
                break;
            case "128049407":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Burst Laser";
                break;
            case "XX":
            //case "128671449":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Cytoscrambler";
                break;

            // Cannons
            case "128049441":
                module.@class = "4";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Cannon";
                break;
            case "128049444":
                module.@class = "4";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Cannon";
                break;
            case "128049440":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Cannon";
                break;
            case "128671120":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Cannon";
                break;
            case "128049447":
                module.@class = "3";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Cannon";
                break;
            case "128049439":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Cannon";
                break;
            case "128049443":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Cannon";
                break;
            case "128049446":
                module.@class = "2";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Cannon";
                break;
            case "128049438":
                module.@class = "1";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Cannon";
                break;
            case "128049442":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Cannon";
                break;
            case "128049445":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Cannon";
                break;

            // Fragment Cannon
            case "128049450":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Fragment Cannon";
                break;
            case "128671321":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Fragment Cannon";
                break;
            case "128671322":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Fragment Cannon";
                break;
            case "128049449":
                module.@class = "2";
                module.rating = "A";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Fragment Cannon";
                break;
            case "128049452":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Fragment Cannon";
                break;
            case "128049454":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Fragment Cannon";
                break;
            case "128049448":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Fragment Cannon";
                break;
            case "128049451":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Fragment Cannon";
                break;
            case "128049453":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Fragment Cannon";
                break;
            case "X":
            //case "128671343":
                module.@class = "3";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Pacifier";
                break;



            // Mine Launcher
            case "128049500":
                module.@class = "1";
                module.rating = "I";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Mine Launcher";
                break;
            case "128049501":
                module.@class = "2";
                module.rating = "I";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Mine Launcher";
                break;
            case "128671448":
                module.@class = "1";
                module.rating = "I";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Shock Mine Launcher";
                break;


            // Mining Laser
            case "128049525":
                module.@class = "1";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Mining Laser";
                break;
            case "128049526":
                module.@class = "2";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Mining Laser";
                break;
            case "XXXX":
                module.@class = "1";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Mining Lance";
                break;

            // Missle Rack
            case "128666725":
                module.@class = "2";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Missile Rack";
                module.guidance = "Dumbfire";
                break;
            case "128049493":
                module.@class = "2";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Missile Rack";
                module.guidance = "Seeker";
                break;
            case "128666724":
                module.@class = "1";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Missile Rack";
                module.guidance = "Dumbfire";
                break;
            case "128049492":
                module.@class = "1";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Missile Rack";
                module.guidance = "Seeker";
                break;
            case "XXXXXXXXXXX":
            //case "128671344":
                module.@class = "2";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Pack-Hound";
                module.guidance = "Seeker";
                break;

            //Multi-Cannon
            case "128049456":
                module.@class = "2";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Multi-Cannon";
                break;
            case "128049460":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Multi-Cannon";
                break;
            case "128049463":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Multi-Cannon";
                break;
            case "128049455":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Multi-Cannon";
                break;
            case "128049459":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Multi-Cannon";
                break;
            case "128049462":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Multi-Cannon";
                break;
            case "XXXXXX":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Enforcer";
                break;

            //Plasma Accelerator
            case "128049467":
                module.@class = "4";
                module.rating = "A";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Plasma Accelerator";
                break;
            case "128049466":
                module.@class = "3";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Plasma Accelerator";
                break;
            case "128049465":
                module.@class = "2";
                module.rating = "C";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Plasma Accelerator";
                break;
            //case "128671339":
            case "XXXXXXXXXX":
                module.@class = "3";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Advanced Plasma Accelerator";
                break;

            // Pulse Lasers
            case "128049383":
                module.@class = "3";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Pulse Laser";
                break;
            case "128049387":
                module.@class = "3";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Pulse Laser";
                break;
            case "128049390":
                module.@class = "3";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Pulse Laser";
                break;
            case "128049382":
                module.@class = "2";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Pulse Laser";
                break;
            case "128049386":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Pulse Laser";
                break;
            case "128049389":
                module.@class = "2";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Pulse Laser";
                break;
            case "128049381":
                module.@class = "1";
                module.rating = "F";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Pulse Laser";
                break;
            case "128049385":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Gimballed";
                module.name = "Pulse Laser";
                break;
            case "128049388":
                module.@class = "1";
                module.rating = "G";
                module.category = "hardpoint";
                module.mount = "Turreted";
                module.name = "Pulse Laser";
                break;
            case "XXXXXXX":
                module.@class = "1";
                module.rating = "E";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Disruptor";
                break;


            // Rail Gun
            case "128049488":
                module.@class = "1";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Rail Gun";
                break;
            case "128049489":
                module.@class = "2";
                module.rating = "B";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Rail Gun";
                break;
            case "xxxxx":
            //case "128671341":
                module.@class = "1";
                module.rating = "D";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Imperial Hammer";
                break;


            // Torpedo Pylon
            case "128049509":
                module.@class = "1";
                module.rating = "I";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Torpedo Pylon";
                module.guidance = "Seeker";
                break;
            case "128049510":
                module.@class = "2";
                module.rating = "I";
                module.category = "hardpoint";
                module.mount = "Fixed";
                module.name = "Torpedo Pylon";
                module.guidance = "Seeker";
                break;


            // Countermeasures
            case "128049513":
                module.@class = "0";
                module.rating = "I";
                module.category = "utility";
                module.name = "Chaff Launcher";
                break;
            case "128049516":
                module.@class = "0";
                module.rating = "F";
                module.category = "utility";
                module.name = "Electronic Countermeasure";
                break;
            case "128049519":
                module.@class = "0";
                module.rating = "I";
                module.category = "utility";
                module.name = "Heat Sink Launcher";
                break;
            case "128049522":
                module.@class = "0";
                module.rating = "I";
                module.category = "utility";
                module.name = "Point Defence";
                break;

            // Frame Shift Wake Scanner
            case "128662525":
                module.@class = "0";
                module.rating = "E";
                module.category = "utility";
                module.name = "Frame Shift Wake Scanner";
                break;
            case "128662526":
                module.@class = "0";
                module.rating = "D";
                module.category = "utility";
                module.name = "Frame Shift Wake Scanner";
                break;
            case "128662527":
                module.@class = "0";
                module.rating = "C";
                module.category = "utility";
                module.name = "Frame Shift Wake Scanner";
                break;
            case "128662528":
                module.@class = "0";
                module.rating = "B";
                module.category = "utility";
                module.name = "Frame Shift Wake Scanner";
                break;
            case "128662529":
                module.@class = "0";
                module.rating = "A";
                module.category = "utility";
                module.name = "Frame Shift Wake Scanner";
                break;

            // Kill Warrant Scanner
            case "128662530":
                module.@class = "0";
                module.rating = "E";
                module.category = "utility";
                module.name = "Kill Warrant Scanner";
                break;
            case "128662531":
                module.@class = "0";
                module.rating = "D";
                module.category = "utility";
                module.name = "Kill Warrant Scanner";
                break;
            case "128662532":
                module.@class = "0";
                module.rating = "C";
                module.category = "utility";
                module.name = "Kill Warrant Scanner";
                break;
            case "128662533":
                module.@class = "0";
                module.rating = "B";
                module.category = "utility";
                module.name = "Kill Warrant Scanner";
                break;
            case "128662534":
                module.@class = "0";
                module.rating = "A";
                module.category = "utility";
                module.name = "Kill Warrant Scanner";
                break;

           // Shield Booster
            case "128668532":
                module.@class = "0";
                module.rating = "E";
                module.category = "utility";
                module.name = "Shield Booster";
                break;
            case "128668533":
                module.@class = "0";
                module.rating = "D";
                module.category = "utility";
                module.name = "Shield Booster";
                break;
            case "128668534":
                module.@class = "0";
                module.rating = "C";
                module.category = "utility";
                module.name = "Shield Booster";
                break;
            case "128668535":
                module.@class = "0";
                module.rating = "B";
                module.category = "utility";
                module.name = "Shield Booster";
                break;
            case "128668536":
                module.@class = "0";
                module.rating = "A";
                module.category = "utility";
                module.name = "Shield Booster";
                break;


            //Auto Field-Maintenance Unit
            case "128667605":
                module.@class = "8";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667613":
                module.@class = "8";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667621":
                module.@class = "8";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667629":
                module.@class = "8";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667637":
                module.@class = "8";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667604":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667612":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667620":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667628":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667636":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667603":
                module.@class = "6";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667611":
                module.@class = "6";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667619":
                module.@class = "6";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667627":
                module.@class = "6";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667635":
                module.@class = "6";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667602":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667610":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667618":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667626":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667634":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667601":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667609":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667617":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667625":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667633":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667600":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667608":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667616":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667624":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667632":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667599":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667607":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667615":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667623":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667631":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667598":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667606":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667614":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667622":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;
            case "128667630":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Auto Field-Maintenance Unit";
                break;

            //Bi-Weave Shield Generator
            case "128671331":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671332":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671333":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671334":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671335":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671336":
                module.@class = "6";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671337":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;
            case "128671338":
                module.@class = "8";
                module.rating = "C";
                module.category = "internal";
                module.name = "Bi-Weave Shield Generator";
                break;

            //Cargo Rack
            case "128064338":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064339":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064340":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064341":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064342":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064343":
                module.@class = "6";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064344":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;
            case "128064345":
                module.@class = "8";
                module.rating = "E";
                module.category = "internal";
                module.name = "Cargo Rack";
                break;


            //Cargo Scanner
            case "128662520":
                module.@class = "0";
                module.rating = "E";
                module.category = "utility";
                module.name = "Cargo Scanner";
                break;
            case "128662521":
                module.@class = "0";
                module.rating = "D";
                module.category = "utility";
                module.name = "Cargo Scanner";
                break;
            case "128662522":
                module.@class = "0";
                module.rating = "C";
                module.category = "utility";
                module.name = "Cargo Scanner";
                break;
            case "128662523":
                module.@class = "0";
                module.rating = "B";
                module.category = "utility";
                module.name = "Cargo Scanner";
                break;
            case "128662524":
                module.@class = "0";
                module.rating = "A";
                module.category = "utility";
                module.name = "Cargo Scanner";
                break;

            //Collector Limpet Controller
            case "128671244":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671245":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671246":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671247":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671248":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671239":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671240":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671241":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671242":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671243":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671234":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671235":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671236":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671237":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671238":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671229":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671230":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671231":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671232":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;
            case "128671233":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Collector Limpet Controller";
                break;

            //Standard Docking Computer
            case "128049549":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Standard Docking Computer";
                break;

            //Frame Shift Drive Interdictor
            case "128666707":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666711":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666715":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666719":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666723":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666706":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666710":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666714":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666718":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666722":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666705":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666709":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666713":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666717":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666721":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666704":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666708":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666712":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666716":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;
            case "128666720":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Frame Shift Drive Interdictor";
                break;

            //Fuel Scoop
            case "128666651":
                module.@class = "8";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666659":
                module.@class = "8";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666667":
                module.@class = "8";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666675":
                module.@class = "8";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666683":
                module.@class = "8";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666650":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666658":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666666":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666674":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666682":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666649":
                module.@class = "6";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666657":
                module.@class = "6";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666665":
                module.@class = "6";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666673":
                module.@class = "6";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666681":
                module.@class = "6";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666648":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666656":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666664":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666672":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666680":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666647":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666655":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666663":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666671":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666679":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666646":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666654":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666662":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666670":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666678":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666645":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666653":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666661":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666669":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666677":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666644":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666652":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666660":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666668":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;
            case "128666676":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Scoop";
                break;


            //Fuel Transfer Limpet Controller
            case "128671264":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671265":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671266":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671267":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671268":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671259":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671260":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671261":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671262":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671263":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671254":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671255":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671256":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671257":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671258":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671249":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671250":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671251":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671252":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;
            case "128671253":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Fuel Transfer Limpet Controller";
                break;


            //Hatch Breaker Limpet Controller
            case "128066547":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066548":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066549":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066550":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066551":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066542":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066543":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066544":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066545":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066546":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066537":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066538":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066539":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066540":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066541":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066532":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066533":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066534":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066535":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;
            case "128066536":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Hatch Breaker Limpet Controller";
                break;


            //Hull Reinforcement Package
            case "128668545":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668546":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668543":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668544":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668541":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668542":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668539":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668540":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668537":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;
            case "128668538":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Hull Reinforcement Package";
                break;


            //Fuel Tank
            //  EDDN seems to use the "standard" category fuel tanks


            //Planetary Vehicle Hangar
            case "128672292":
                module.@class = "6";
                module.rating = "H";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;
            case "128672293":
                module.@class = "6";
                module.rating = "G";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;
            case "128672290":
                module.@class = "4";
                module.rating = "H";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;
            case "128672291":
                module.@class = "4";
                module.rating = "G";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;
            case "128672288":
                module.@class = "2";
                module.rating = "H";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;
            case "128672289":
                module.@class = "2";
                module.rating = "G";
                module.category = "internal";
                module.name = "Planetary Vehicle Hangar";
                break;

            //Prismatic Shield Generator
            case "128671323":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671324":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671325":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671326":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671327":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671328":
                module.@class = "6";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671329":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;
            case "128671330":
                module.@class = "8";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prismatic Shield Generator";
                break;


            //Prospector Limpet Controller
            case "128671284":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671285":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671286":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671287":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671288":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671279":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671280":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671281":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671282":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671283":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671274":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671275":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671276":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671277":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671278":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671269":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671270":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671271":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671272":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;
            case "128671273":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Prospector Limpet Controller";
                break;


            //Refinery
            case "128666687":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666691":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666695":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666699":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666703":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666686":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666690":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666694":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666698":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666702":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666685":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666689":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666693":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666697":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666701":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666684":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666688":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666692":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666696":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Refinery";
                break;
            case "128666700":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Refinery";
                break;


            //Scanners
            case "128663561":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Advanced Discovery Scanner";
                break;
            case "128663560":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Intermediate Discovery Scanner";
                break;
            case "128662535":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Basic Discovery Scanner";
                break;
            case "128666634":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Detailed Surface Scanner";
                break;


            //Shield Cell Bank
            case "128064333":
                module.@class = "8";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064334":
                module.@class = "8";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064335":
                module.@class = "8";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064336":
                module.@class = "8";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064337":
                module.@class = "8";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064328":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064329":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064330":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064331":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064332":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064323":
                module.@class = "6";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064324":
                module.@class = "6";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064325":
                module.@class = "6";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064326":
                module.@class = "6";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064327":
                module.@class = "6";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064318":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064319":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064320":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064321":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064322":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064313":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064314":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064315":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064316":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064317":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064308":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064309":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064310":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064311":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064312":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064303":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064304":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064305":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064306":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064307":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064298":
                module.@class = "1";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064299":
                module.@class = "1";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064300":
                module.@class = "1";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064301":
                module.@class = "1";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;
            case "128064302":
                module.@class = "1";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Cell Bank";
                break;


            //Shield Generator
            case "128064293":
                module.@class = "8";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064294":
                module.@class = "8";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064295":
                module.@class = "8";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064296":
                module.@class = "8";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064297":
                module.@class = "8";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064288":
                module.@class = "7";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064289":
                module.@class = "7";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064290":
                module.@class = "7";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064291":
                module.@class = "7";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064292":
                module.@class = "7";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064283":
                module.@class = "6";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064284":
                module.@class = "6";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064285":
                module.@class = "6";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064286":
                module.@class = "6";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064287":
                module.@class = "6";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064278":
                module.@class = "5";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064279":
                module.@class = "5";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064280":
                module.@class = "5";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064281":
                module.@class = "5";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064282":
                module.@class = "5";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064273":
                module.@class = "4";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064274":
                module.@class = "4";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064275":
                module.@class = "4";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064276":
                module.@class = "4";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064277":
                module.@class = "4";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064268":
                module.@class = "3";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064269":
                module.@class = "3";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064270":
                module.@class = "3";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064271":
                module.@class = "3";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064272":
                module.@class = "3";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064263":
                module.@class = "2";
                module.rating = "E";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064264":
                module.@class = "2";
                module.rating = "D";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064265":
                module.@class = "2";
                module.rating = "C";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064266":
                module.@class = "2";
                module.rating = "B";
                module.category = "internal";
                module.name = "Shield Generator";
                break;
            case "128064267":
                module.@class = "2";
                module.rating = "A";
                module.category = "internal";
                module.name = "Shield Generator";
                break;


            //Frame Shift Drive
            case "128064133":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064134":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064135":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064136":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064137":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064128":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064129":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064130":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064131":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064132":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064123":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064124":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064125":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064126":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064127":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064118":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064119":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064120":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064121":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064122":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064113":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064114":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064115":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064116":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064117":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064108":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064109":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064110":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064111":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064112":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064103":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064104":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064105":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064106":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;
            case "128064107":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Frame Shift Drive";
                break;

            //Fuel Tank
            case "128064346":
                module.@class = "1";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064347":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064348":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064349":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064350":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064351":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064352":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;
            case "128064353":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Fuel Tank";
                break;


            //Life Support
            case "128064173":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064174":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064175":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064176":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064177":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064168":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064169":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064170":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064171":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064172":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064163":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064164":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064165":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064166":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064167":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064158":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064159":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064160":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064161":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064162":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064153":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064154":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064155":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064156":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064157":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064148":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064149":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064150":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064151":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064152":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064143":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064144":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064145":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064146":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064147":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064138":
                module.@class = "1";
                module.rating = "E";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064139":
                module.@class = "1";
                module.rating = "D";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064140":
                module.@class = "1";
                module.rating = "C";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064141":
                module.@class = "1";
                module.rating = "B";
                module.category = "standard";
                module.name = "Life Support";
                break;
            case "128064142":
                module.@class = "1";
                module.rating = "A";
                module.category = "standard";
                module.name = "Life Support";
                break;


            //Planetary Approach Suite
            case "128672317":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Planetary Approach Suite";
                break;


            //Power Distributor
            case "128064213":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064214":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064215":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064216":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064217":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064208":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064209":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064210":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064211":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064212":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064203":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064204":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064205":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064206":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064207":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064198":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064199":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064200":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064201":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064202":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064193":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064194":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064195":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064196":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064197":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064188":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064189":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064190":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064191":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064192":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064183":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064184":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064185":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064186":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064187":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064178":
                module.@class = "1";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064179":
                module.@class = "1";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064180":
                module.@class = "1";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064181":
                module.@class = "1";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Distributor";
                break;
            case "128064182":
                module.@class = "1";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Distributor";
                break;


            //Power Plant
            case "128064063":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064064":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064065":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064066":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064067":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064058":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064059":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064060":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064061":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064062":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064053":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064054":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064055":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064056":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064057":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064048":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064049":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064050":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064051":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064052":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064043":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064044":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064045":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064046":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064047":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064038":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064039":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064040":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064041":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064042":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064033":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064034":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064035":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064036":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Power Plant";
                break;
            case "128064037":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Power Plant";
                break;


            //Sensors
            case "128064253":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064254":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064255":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064256":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064257":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064248":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064249":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064250":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064251":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064252":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064243":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064244":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064245":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064246":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064247":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064238":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064239":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064240":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064241":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064242":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064233":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064234":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064235":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064236":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064237":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064228":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064229":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064230":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064231":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064232":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064223":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064224":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064225":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064226":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064227":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064218":
                module.@class = "1";
                module.rating = "E";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064219":
                module.@class = "1";
                module.rating = "D";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064220":
                module.@class = "1";
                module.rating = "C";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064221":
                module.@class = "1";
                module.rating = "B";
                module.category = "standard";
                module.name = "Sensors";
                break;
            case "128064222":
                module.@class = "1";
                module.rating = "A";
                module.category = "standard";
                module.name = "Sensors";
                break;


            //Thrusters
            case "128064098":
                module.@class = "8";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064099":
                module.@class = "8";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064100":
                module.@class = "8";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064101":
                module.@class = "8";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064102":
                module.@class = "8";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064093":
                module.@class = "7";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064094":
                module.@class = "7";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064095":
                module.@class = "7";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064096":
                module.@class = "7";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064097":
                module.@class = "7";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064088":
                module.@class = "6";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064089":
                module.@class = "6";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064090":
                module.@class = "6";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064091":
                module.@class = "6";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064092":
                module.@class = "6";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064083":
                module.@class = "5";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064084":
                module.@class = "5";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064085":
                module.@class = "5";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064086":
                module.@class = "5";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064087":
                module.@class = "5";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064078":
                module.@class = "4";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064079":
                module.@class = "4";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064080":
                module.@class = "4";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064081":
                module.@class = "4";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064082":
                module.@class = "4";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064073":
                module.@class = "3";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064074":
                module.@class = "3";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064075":
                module.@class = "3";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064076":
                module.@class = "3";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064077":
                module.@class = "3";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064068":
                module.@class = "2";
                module.rating = "E";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064069":
                module.@class = "2";
                module.rating = "D";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064070":
                module.@class = "2";
                module.rating = "C";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064071":
                module.@class = "2";
                module.rating = "B";
                module.category = "standard";
                module.name = "Thrusters";
                break;
            case "128064072":
                module.@class = "2";
                module.rating = "A";
                module.category = "standard";
                module.name = "Thrusters";
                break;


            //Adder Bulkheads
            case "128049268":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Adder";
                break;
            case "128049269":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Adder";
                break;
            case "128049270":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Adder";
                break;
            case "128049271":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Adder";
                break;
            case "128049272":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Adder";
                break;


            //Anaconda Bulkheads
            case "128049364":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Anaconda";
                break;
            case "128049365":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Anaconda";
                break;
            case "128049366":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Anaconda";
                break;
            case "128049367":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Anaconda";
                break;
            case "128049368":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Anaconda";
                break;


            //Asp Bulkheads
            case "128049304":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Asp";
                break;
            case "128049305":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Asp";
                break;
            case "128049306":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Asp";
                break;
            case "128049307":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Asp";
                break;
            case "128049308":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Asp";
                break;


            //Asp Scout Bulkheads
            case "128672278":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Asp Scout";
                break;
            case "128672279":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Asp Scout";
                break;
            case "128672280":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Asp Scout";
                break;
            case "128672281":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Asp Scout";
                break;
            case "128672282":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Asp Scout";
                break;


            //Cobra Mk III Bulkheads
            case "128049280":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Cobra Mk III";
                break;
            case "128049281":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Cobra Mk III";
                break;
            case "128049282":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Cobra Mk III";
                break;
            case "128049283":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Cobra Mk III";
                break;
            case "128049284":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Cobra Mk III";
                break;


            //Cobra MkIV Bulkheads
            case "128672264":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Cobra MkIV";
                break;
            case "128672265":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Cobra MkIV";
                break;
            case "128672266":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Cobra MkIV";
                break;
            case "128672267":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Cobra MkIV";
                break;
            case "128672268":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Cobra MkIV";
                break;


            //DiamondBack Scout Bulkheads
            case "128671218":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Diamondback Scout";
                break;
            case "128671219":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Diamondback Scout";
                break;
            case "128671220":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Diamondback Scout";
                break;
            case "128671221":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Diamondback Scout";
                break;
            case "128671222":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Diamondback Scout";
                break;


            //Diamondback Explorer Bulkheads
            case "128671832":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Diamondback Explorer";
                break;
            case "128671833":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Diamondback Explorer";
                break;
            case "128671834":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Diamondback Explorer";
                break;
            case "128671835":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Diamondback Explorer";
                break;
            case "128671836":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Diamondback Explorer";
                break;


            //Eagle Bulkheads
            case "128049256":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Eagle";
                break;
            case "128049257":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Eagle";
                break;
            case "128049258":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Eagle";
                break;
            case "128049259":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Eagle";
                break;
            case "128049260":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Eagle";
                break;


            //Federal Assault Ship Bulkheads
            case "128672147":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Federal Assault Ship";
                break;
            case "128672148":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Federal Assault Ship";
                break;
            case "128672149":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Federal Assault Ship";
                break;
            case "128672150":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Federal Assault Ship";
                break;
            case "128672151":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Federal Assault Ship";
                break;


            //Federal Corvette Bulkheads
            case "128049370":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Federal Corvette";
                break;
            case "128049371":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Federal Corvette";
                break;
            case "128049372":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Federal Corvette";
                break;
            case "128049373":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Federal Corvette";
                break;
            case "128049374":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Federal Corvette";
                break;


            //Federal Dropship Bulkheads
            case "128049322":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Federal Dropship";
                break;
            case "128049323":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Federal Dropship";
                break;
            case "128049324":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Federal Dropship";
                break;
            case "128049325":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Federal Dropship";
                break;
            case "128049326":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Federal Dropship";
                break;


            //Federal Gunship Bulkheads
            case "128672154":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Federal Gunship";
                break;
            case "128672155":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Federal Gunship";
                break;
            case "128672156":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Federal Gunship";
                break;
            case "128672157":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Federal Gunship";
                break;
            case "128672158":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Federal Gunship";
                break;


            //Fer-de-Lance Bulkheads
            case "128049352":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Fer-de-Lance";
                break;
            case "128049353":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Fer-de-Lance";
                break;
            case "128049354":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Fer-de-Lance";
                break;
            case "128049356":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Fer-de-Lance";
                break;
            case "128049355":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Fer-de-Lance";
                break;


            //Hauler Bulkheads
            case "128049262":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Hauler";
                break;
            case "128049263":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Hauler";
                break;
            case "128049264":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Hauler";
                break;
            case "128049265":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Hauler";
                break;
            case "128049266":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Hauler";
                break;


            //Imperial Clipper Bulkheads
            case "128049316":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Imperial Clipper";
                break;
            case "128049317":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Imperial Clipper";
                break;
            case "128049318":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Imperial Clipper";
                break;
            case "128049319":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Imperial Clipper";
                break;
            case "128049320":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Imperial Clipper";
                break;


            //Imperial Courier Bulkheads
            case "128671224":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Imperial Courier";
                break;
            case "128671225":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Imperial Courier";
                break;
            case "128671226":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Imperial Courier";
                break;
            case "128671227":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Imperial Courier";
                break;
            case "128671228":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Imperial Courier";
                break;


            //Imperial Cutter Bulkheads
            case "128049376":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Imperial Cutter";
                break;
            case "128049377":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Imperial Cutter";
                break;
            case "128049378":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Imperial Cutter";
                break;
            case "128049379":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Imperial Cutter";
                break;
            case "128049380":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Imperial Cutter";
                break;


            //Imperial Eagle Bulkheads
            case "128672140":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Imperial Eagle";
                break;
            case "128672141":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Imperial Eagle";
                break;
            case "128672142":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Imperial Eagle";
                break;
            case "128672143":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Imperial Eagle";
                break;
            case "128672144":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Imperial Eagle";
                break;


            //Keelback Bulkheads
            case "128672271":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Keelback";
                break;
            case "128672272":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Keelback";
                break;
            case "128672273":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Keelback";
                break;
            case "128672274":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Keelback";
                break;
            case "128672275":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Keelback";
                break;


            //Orca Bulkheads
            case "128049328":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Orca";
                break;
            case "128049329":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Orca";
                break;
            case "128049330":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Orca";
                break;
            case "128049331":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Orca";
                break;
            case "128049332":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Orca";
                break;


            //Python Bulkheads
            case "128049340":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Python";
                break;
            case "128049341":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Python";
                break;
            case "128049342":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Python";
                break;
            case "128049343":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Python";
                break;
            case "128049344":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Python";
                break;


            //Sidewinder Bulkheads
            case "128049250":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Sidewinder";
                break;
            case "128049251":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Sidewinder";
                break;
            case "128049252":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Sidewinder";
                break;
            case "128049253":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Sidewinder";
                break;
            case "128049254":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Sidewinder";
                break;


            //Type-6 Transporter Bulkheads
            case "128049286":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Type-6 Transporter";
                break;
            case "128049287":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Type-6 Transporter";
                break;
            case "128049288":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Type-6 Transporter";
                break;
            case "128049289":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Type-6 Transporter";
                break;
            case "128049290":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Type-6 Transporter";
                break;


            //Type-7 Transporter Bulkheads
            case "128049298":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Type-7 Transporter";
                break;
            case "128049299":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Type-7 Transporter";
                break;
            case "128049300":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Type-7 Transporter";
                break;
            case "128049301":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Type-7 Transporter";
                break;
            case "128049302":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Type-7 Transporter";
                break;


            //Type-9 Heavy Bulkheads
            case "128049334":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Type-9 Heavy";
                break;
            case "128049335":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Type-9 Heavy";
                break;
            case "128049336":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Type-9 Heavy";
                break;
            case "128049337":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Type-9 Heavy";
                break;
            case "128049338":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Type-9 Heavy";
                break;


            //Viper Bulkheads
            case "128049274":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Viper";
                break;
            case "128049275":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Viper";
                break;
            case "128049276":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Viper";
                break;
            case "128049277":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Viper";
                break;
            case "128049278":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Viper";
                break;


            //Viper MkIV Bulkheads
            case "128672257":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Viper MkIV";
                break;
            case "128672258":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Viper MkIV";
                break;
            case "128672259":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Viper MkIV";
                break;
            case "128672260":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Viper MkIV";
                break;
            case "128672261":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Viper MkIV";
                break;


            //Vulture Bulkheads
            case "128049310":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Lightweight Alloy";
                module.ship = "Vulture";
                break;
            case "128049311":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reinforced Alloy";
                module.ship = "Vulture";
                break;
            case "128049312":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Military Grade Composite";
                module.ship = "Vulture";
                break;
            case "128049313":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Mirrored Surface Composite";
                module.ship = "Vulture";
                break;
            case "128049314":
                module.@class = "1";
                module.rating = "I";
                module.category = "standard";
                module.name = "Reactive Surface Composite";
                module.ship = "Vulture";
                break;

            default:
                success = false;
                break;


             
        }

        return Tuple.Create<bool, Module>(success, module);
        
    }
}