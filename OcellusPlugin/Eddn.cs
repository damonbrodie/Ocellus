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
        public string schemaRef = "http://schemas.elite-markets.net/eddn/commodity/2";
        [DataMember(Order = 2)]
        public Header header = new Header();
        [DataMember(Order = 3)]
        public MessageOutfitting message = new MessageOutfitting();
    }

    [DataContract]
    public class RootObjectShips
    {
        [DataMember(Name = "$schemaRef", Order = 1)]
        public string schemaRef = "http://schemas.elite-markets.net/eddn/commodity/2";
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
        string demandLevel = mapBracket((int)currCommodity["demandBracket"]);
        if (demandLevel != null)
        {
            newCommodity.demandLevel = demandLevel;
        }
        string supplyLevel = mapBracket((int)currCommodity["stockBracket"]);
        if (supplyLevel != null)
        {
            newCommodity.supplyLevel = supplyLevel;
        }
        return newCommodity;
    }

    private static Tuple<string, string, string> createEddnJson(ref Dictionary<string, object> state)
    {
        if (!state.ContainsKey("VAEDcompanionDict"))
        {
            Debug.Write("Error:  No Companion API data");
            return Tuple.Create<string, string, string>(null, null, null);
        }

        Dictionary<string, dynamic> companion = (Dictionary<string, dynamic>)state["VAEDcompanionDict"];

        bool isDocked = companion["commander"]["docked"];
        if (!isDocked)
        {
            Debug.Write("Not Docked - won't submit stale data to EDDN");
            return Tuple.Create<string, string, string>(null, null, null);
        }

        string system = companion["lastSystem"]["name"];
        string starport = companion["lastStarport"]["name"];

        RootObjectCommodities eddnCommodities = new RootObjectCommodities();
        RootObjectOutfitting eddnOutfitting = new RootObjectOutfitting();
        RootObjectShips eddnShips = new RootObjectShips();

        eddnCommodities.header.uploaderID = companion["commander"]["name"];
        eddnCommodities.message.systemName = system;
        eddnCommodities.message.stationName = starport;
        eddnCommodities.message.timestamp = state["VAEDcompanionTime"].ToString();

        string commodityJson = null;
        string outfittinJson = null;
        string shipsJson = null;

        if (companion.ContainsKey("lastStarport") && companion["lastStartport"].ContainsKey("commodities"))
        {
            ArrayList commodities = companion["lastStarport"]["commodities"];

            // We don't submit if there are no commodities here
            if (commodities.Count > 0)
            {
                foreach (Dictionary<string, object> currCommodity in commodities)
                {
                    Commodity toAdd = extractCommodity(currCommodity);
                    eddnCommodities.message.commodities.Add(toAdd);
                }
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObjectCommodities));
                MemoryStream stream = new MemoryStream();
                serializer.WriteObject(stream, eddnCommodities);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                commodityJson = sr.ReadToEnd();
            }
        }

        if (companion.ContainsKey("lastStarport") && companion["lastStartport"].ContainsKey("modules"))
        {
            ArrayList modules = companion["lastStarport"]["modules"];

        }


            return Tuple.Create<string, string, string>(commodityJson, outfittinJson, shipsJson);
    }

    public static void updateEddn(ref Dictionary<string, object> state)
    {
        Tuple<string, string, string> tResponse = createEddnJson(ref state);
        if (tResponse.Item1 != null)
        {
            Web.sendRequest(uploadURL, null, null, tResponse.Item1);
            Debug.Write(tResponse.Item1);
        }
    }
}