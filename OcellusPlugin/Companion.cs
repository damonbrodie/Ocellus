using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Threading.Tasks;


// **************************************************************************
// *  Functions for accessing Frontier's Companion API for Elite Dangerous  *
// **************************************************************************
class Companion
{
    private const string loginURL = "https://companion.orerve.net/user/login";
    private const string confirmURL = "https://companion.orerve.net/user/confirm";
    private const string profileURL = "https://companion.orerve.net/profile";

    public static Tuple<CookieContainer, string> loginToAPI(CookieContainer cookieContainer)
    {
        string email = PluginRegistry.getStringValue("email");
        string password = PluginRegistry.getStringValue("password");
        if (email == null || email == string.Empty || password == null || password == string.Empty)
        {
            return Tuple.Create(cookieContainer, "credentials");
        }
        string returnString;

        Tuple<bool, string, CookieContainer, string> tInitialGet = Web.sendRequest(loginURL, cookieContainer);
        // XXX handle returned errors
        cookieContainer = tInitialGet.Item3;
        string loginPageHTML = tInitialGet.Item4;

        if (loginPageHTML.Contains("Login"))
        {
            string sendData = "email=" + email + "&password=" + password;
            Tuple<bool, string, CookieContainer, string> tLoginResponse = Web.sendRequest(loginURL, cookieContainer, loginURL, sendData);
            // XXX handle returned errors
            cookieContainer = tLoginResponse.Item3;
            string postPageHTML = tLoginResponse.Item4;
            Debug.Write("LOGIN OUTPUT:  " + postPageHTML);
            if (postPageHTML.Contains("Verification"))
            {
                returnString = "verification";
            }
            else if (postPageHTML.Contains("password"))
            {
                returnString = "credentials";
            }
            else
            {
                // When verification works it doesn't return content, assume we are logged in
                returnString = "ok";
            }
        }
        else
        {
            Debug.Write("Got empty response");
            returnString = "ok";
        }
        return Tuple.Create(cookieContainer, returnString);
    }

    public static Tuple<CookieContainer, string> verifyWithAPI(CookieContainer cookieContainer, string verificationCode)
    {
        string sendData = "code=" + verificationCode;
        Tuple<bool, string, CookieContainer, string> tResponse = Web.sendRequest(confirmURL, cookieContainer, confirmURL, sendData);

        // XXX handle returned error code
        Debug.Write("return code: " + tResponse.Item1.ToString());
        Debug.Write("return message: " + tResponse.Item2.ToString());
        Debug.Write("return HTML:  " + tResponse.Item4);

        string postVerifyHTML = tResponse.Item4;
        if (postVerifyHTML.Contains("Verification Code"))
        {
            return Tuple.Create(tResponse.Item3, "verification");
        }
        else if (postVerifyHTML.Contains("Please correct"))
        {
            return Tuple.Create(tResponse.Item3, "login");
        }
        else
        {
            return Tuple.Create(tResponse.Item3, "ok");
        }
    }

    private static Tuple<CookieContainer, string> getProfile(CookieContainer cookieContainer)
    {
        Tuple<bool, string, CookieContainer, string> tRespon = Web.sendRequest(profileURL, cookieContainer);
        // XXX handle error messages
        return Tuple.Create(tRespon.Item3, tRespon.Item4);
    }

    public static bool updateProfile(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, bool?> booleanValues)
    {


        int profileCooldown = Utilities.isCoolingDown(ref state, "VAEDprofileCooldown", 30);
        if (profileCooldown > 0)
        {
            Debug.Write("Get Profile is cooling down: " + profileCooldown.ToString() + " seconds remain.");
            return true;
        }

        textValues["VAEDprofileStatus"] = "ok";
        string htmlData = "";

        // Load debug companion JSON if it is present.
        string debugJson = Path.Combine(Config.Path(), "debug_companion.json");
        if (File.Exists(debugJson))
        {
            htmlData = File.ReadAllText(debugJson);
        }
        else
        {
            CookieContainer profileCookies = (CookieContainer)state["VAEDcookieContainer"];
            Tuple<CookieContainer, string> tRespon = Companion.getProfile(profileCookies);

            state["VAEDcookieContainer"] = tRespon.Item1;
            Web.WriteCookiesToDisk(Config.CookiePath(), tRespon.Item1);
            htmlData = tRespon.Item2;
            if (htmlData.Contains("Please correct the following") || htmlData == "")
            {
                textValues["VAEDprofileStatus"] = "error";
                return false;
            }
        }
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var result = new Dictionary<string, dynamic>();

        bool currentlyDocked = false;
        try
        {
            result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);
            state["VAEDcompanionDict"] = result;
            string cmdr = result["commander"]["name"];
            int credits = result["commander"]["credits"];
            int debt = result["commander"]["debt"];
            int shipId = result["commander"]["currentShipId"];
            string currentShipId = shipId.ToString();
            currentlyDocked = result["commander"]["docked"];
            string combatRank = Elite.combatRankToString(result["commander"]["rank"]["combat"]);
            string tradeRank = Elite.tradeRankToString(result["commander"]["rank"]["trade"]);
            string exploreRank = Elite.exploreRankToString(result["commander"]["rank"]["explore"]);
            string cqcRank = Elite.cqcRankToString(result["commander"]["rank"]["cqc"]);

            string federationRank = Elite.federationRankToString(result["commander"]["rank"]["federation"]);
            string empireRank = Elite.empireRankToString(result["commander"]["rank"]["empire"]);

            string powerPlayRank = Elite.powerPlayRankToString(result["commander"]["rank"]["power"]);

            string currentShip = "";
            Dictionary<string, dynamic> allShips = new Dictionary<string, dynamic>();
            if (result["ships"].GetType() == typeof(System.Collections.ArrayList))
            {
                for (int counter = 0; counter < result["ships"].Count; counter += 1)
                {
                    string id = result["ships"][counter]["id"].ToString();
                    allShips.Add(id, result["ships"][counter]);
                    if (id == currentShipId)
                    {
                        currentShip = result["ships"][counter]["name"];
                    }
                }
            }
            else
            {
                allShips = result["ships"];
                currentShip = result["ships"][currentShipId]["name"];
            }
            int howManyShips = allShips.Count;
            int cargoCapacity = result["ship"]["cargo"]["capacity"];
            int quantityInCargo = result["ship"]["cargo"]["qty"];

            //Set current System
            textValues["VAEDcurrentSystem"] = null;
            if (result.ContainsKey("lastSystem") && result["lastSystem"].ContainsKey("name"))
            {
                textValues["VAEDcurrentSystem"] = result["lastSystem"]["name"];
            }
            else
            {
                Debug.Write("ERROR: Companion API doesn't have current location ");
                Debug.Write("----------------FRONTIER COMPANION DATA--------------------");
                Debug.Write(htmlData);
                textValues["VAEDprofileStatus"] = "error";
                return false;
            }
            // Null out ship locations
            string[] listOfShips = Elite.listOfShipVariableNames();
            foreach (string ship in listOfShips)
            {
                textValues["VAEDship-" + ship + "-1"] = null;
                intValues["VAEDshipCounter-" + ship] = 0;
            }

            List <string> keys = new List<string>(allShips.Keys);
            Dictionary<string, dynamic> theShips = new Dictionary<string, dynamic>();
            foreach (string key in keys)
            {
                string tempShip = allShips[key]["name"];
                string tempSystem = null;
                if (allShips[key].ContainsKey("starsystem"))
                {
                    tempSystem = allShips[key]["starsystem"]["name"];
                }
                int currDistance = -1;
                if ( tempSystem != null)
                {
                    Dictionary<string, dynamic> tempAtlas = (Dictionary<string, dynamic>)state["VAEDatlasIndex"];
                    currDistance = Atlas.calcDistance(ref tempAtlas, textValues["VAEDcurrentSystem"], tempSystem);
                }
            }

            foreach (string key in keys)
            {
                string tempShip =allShips[key]["name"];

                string tempSystem = null;
                if (allShips[key].ContainsKey("starsystem"))
                {
                    tempSystem = allShips[key]["starsystem"]["name"];
                }
                string variableShipName = Elite.frontierShipToVariable(tempShip);
                string shipCounterString = "VAEDshipCounter-" + variableShipName;
                intValues[shipCounterString]++;
                int counterInt = (int)intValues[shipCounterString];
                string counterStr = counterInt.ToString();
                textValues["VAEDship-" + variableShipName + "-" + counterStr] = tempSystem;
            }

            //Setup ambiguous ship variables
            textValues["VAEDambiguousViper"] = null;
            textValues["VAEDambiguousCobra"] = null;
            textValues["VAEDambiguousDiamondback"] = null;
            textValues["VAEDambiguousAsp"] = null;
            textValues["VAEDambiguousEagle"] = null;

            if ((intValues["VAEDshipCounter-ViperMkIII"] + intValues["VAEDshipCounter-ViperMkIV"]) == 1)
            {
                if (textValues["VAEDship-ViperMkIII-1"] != null)
                {
                    textValues["VAEDambiguousViper"] = textValues["VAEDship-ViperMkIII-1"];
                }
                else
                {
                    textValues["VAEDambiguousViper"] = textValues["VAEDship-ViperMkIV-1"];
                }

            }
            if ((intValues["VAEDshipCounter-CobraMkIII"] + intValues["VAEDshipCounter-CobraMkIV"]) == 1)
            {
                if (textValues["VAEDship-CobraMkIII-1"] != null)
                {
                    textValues["VAEDambiguousCobra"] = textValues["VAEDship-CobraMkIII-1"];
                }
                else
                {
                    textValues["VAEDambiguousCobra"] = textValues["VAEDship-CobraMkIV-1"];
                }
            }
            if ((intValues["VAEDshipCounter-DiamondbackExplorer"] + intValues["VAEDshipCounter-DiamondbackScout"]) == 1)
            {
                if (textValues["VAEDship-DiamondbackScout-1"] != null)
                {
                    textValues["VAEDambiguousDiamondback"] = textValues["VAEDship-DiamondbackScout-1"];
                }
                else
                {
                    textValues["VAEDambiguousDiamondback"] = textValues["VAEDship-DiamondBackExplorer-1"];
                }
            }
            if ((intValues["VAEDshipCounter-AspExplorer"] + intValues["VAEDshipCounter-AspScout"]) == 1)
            {
                if (textValues["VAEDship-AspExplorer-1"] != null)
                {
                    textValues["VAEDambiguousAsp"] = textValues["VAEDship-AspExplorer-1"];
                }
                else
                {
                    textValues["VAEDambiguousAsp"] = textValues["VAEDship-AspScout-1"];
                }
            }
            if ((intValues["VAEDshipCounter-Eagle"] + intValues["VAEDshipCounter-ImperialEagle"]) == 1)
            {
                if (textValues["VAEDship-Eagle-1"] != null)
                {
                    textValues["VAEDambiguousEagle"] = textValues["VAEDship-Eagle-1"];
                }
                else
                {
                    textValues["VAEDambiguousEagle"] = textValues["VAEDship-ImperialEagle-1"];
                }
            }

            intValues["VAEDnumberOfShips"] = howManyShips;
            textValues["VAEDcmdr"] = cmdr;
            intValues["VAEDcredits"] = credits;
            intValues["VAEDloan"] = debt;
            booleanValues["VAEDcurrentlyDocked"] = currentlyDocked;
            textValues["VAEDcombatRank"] = combatRank;
            textValues["VAEDexploreRank"] = exploreRank;
            textValues["VAEDtradeRank"] = tradeRank;
            textValues["VAEDcqcRank"] = cqcRank;
            textValues["VAEDfederationRank"] = federationRank;
            textValues["VAEDempireRank"] = empireRank;
            textValues["VAEDcurrentShip"] = Elite.frontierShipToPretty(currentShip);
            textValues["VAEDphoneticShip"] = Elite.frontierShipToPhonetic(currentShip).ToLower();
            intValues["VAEDcargoCapacity"] = cargoCapacity;
            intValues["VAEDquantityInCargo"] = quantityInCargo;
            Ship.Components shipObj = Ship.decode(result);
            state["VAEDshipObj"] = shipObj;
            booleanValues["VAEDshipHasCargoScanner"] = shipObj.attributes.hasCargoScanner;
            booleanValues["VAEDshipHasFrameShiftWakeScanner"] = shipObj.attributes.hasFrameShiftWakeScanner;
            booleanValues["VAEDshipHasKillWarrantScanner"] = shipObj.attributes.hasKillWarrantScanner;
            booleanValues["VAEDshipHasShieldBooster"] = shipObj.attributes.hasShieldBooster;
            booleanValues["VAEDshipHasChaffLauncher"] = shipObj.attributes.hasChaffLauncher;
            booleanValues["VAEDshipHasElectronicCountermeasures"] = shipObj.attributes.hasElectronicCountermeasures;
            booleanValues["VAEDshipHasHeatSinkLauncher"] = shipObj.attributes.hasHeatSinkLauncher;
            booleanValues["VAEDshipHasPointDefence"] = shipObj.attributes.hasPointDefence;
            textValues["VAEDshipBulkheads"] = shipObj.standard.bulkheads;
            intValues["VAEDshipPowerPlantClass"] = shipObj.standard.powerPlant.@class;
            textValues["VAEDshipPowerPlantRating"] = shipObj.standard.powerPlant.rating;

            textValues["VAEDshipThrustersRating"] = shipObj.standard.thrusters.rating;
            intValues["VAEDshipThrustersClass"] = shipObj.standard.thrusters.@class;
        
            textValues["VAEDshipFrameShiftDriveRating"] = shipObj.standard.frameShiftDrive.rating;
            intValues["VAEDshipFrameShiftDriveClass"] = shipObj.standard.frameShiftDrive.@class;

            textValues["VAEDshipLifeSupportRating"] = shipObj.standard.lifeSupport.rating;
            intValues["VAEDshipLifeSupportClass"] = shipObj.standard.lifeSupport.@class;

            textValues["VAEDshipPowerDistributorRating"] = shipObj.standard.powerDistributor.rating;
            intValues["VAEDshipPowerDistributorClass"] = shipObj.standard.powerDistributor.@class;

            textValues["VAEDshipSensorsRating"] = shipObj.standard.sensors.rating;
            intValues["VAEDshipSensorsClass"] = shipObj.standard.sensors.@class;

            textValues["VAEDshipFuelTankRating"] = shipObj.standard.fuelTank.rating;
            intValues["VAEDshipFuelTankClass"] = shipObj.standard.fuelTank.@class;

        }
        catch (Exception ex)
        {
            Debug.Write("ERROR: Unable to parse Companion API output " + ex.ToString());
            Debug.Write("----------------FRONTIER COMPANION DATA--------------------");
            Debug.Write(htmlData);
            textValues["VAEDprofileStatus"] = "error";
            return false;
        }
        try
        {
            textValues["VAEDeddbStarportId"] = null;
            textValues["VAEDcurrentStarport"] = null;
            textValues["VAEDeddbSystemId"] = null;

            if (currentlyDocked)
            {
                TrackSystems.Add(ref state, result["lastSystem"]["name"]);
                if (result.ContainsKey("lastStarport") && result["lastStarport"].ContainsKey("name"))
                {
                    textValues["VAEDcurrentStarport"] = result["lastStarport"]["name"];
                }

                //Set Station Services
                booleanValues["VAEDstarportCommodities"] = false;
                booleanValues["VAEDstarportShipyard"] = false;
                booleanValues["VAEDstarportOutfitting"] = false;
                if (result["lastStarport"].ContainsKey("commodities"))
                {
                    booleanValues["VAEDstarportCommodities"] = true;
                }
                if (result["lastStarport"].ContainsKey("ships") && result["lastStarport"]["ships"].ContainsKey("shipyard_list"))
                {
                    booleanValues["VAEDstarportShipyard"] = true;
                }
                if (result["lastStarport"].ContainsKey("ships") && result["lastStarport"].ContainsKey("modules"))
                {
                    booleanValues["VAEDstarportOutfitting"] = true;
                }

                // If we're docked, update the EDDN network.
                
                int eddnCooldown = Utilities.isCoolingDown(ref state, "VAEDeddnCooldown", 60 * 10); // Cool down 10 minutes
                if (state.ContainsKey("VAEDlastStation") && (string)state["VAEDlastStation"] == textValues["VAEDcurrentStarport"] && eddnCooldown > 0)
                {
                    Debug.Write("EDDN update is cooling down: " + eddnCooldown.ToString() + " seconds remain.");
                }
                else
                {
                    state["VAEDlastStation"] = textValues["VAEDcurrentStarport"];
                    Task.Run(() => Eddn.updateEddn(result));
                }
            }

            if (state.ContainsKey("VAEDeddbIndex"))
            {
                Dictionary<string, dynamic> tempEddb = (Dictionary<string, dynamic>)state["VAEDeddbIndex"];

                if (textValues["VAEDcurrentSystem"] != null && tempEddb.ContainsKey(textValues["VAEDcurrentSystem"]))
                {
                    textValues["VAEDeddbSystemId"] = tempEddb[textValues["VAEDcurrentSystem"]]["id"].ToString();
                    if (textValues["VAEDcurrentStarport"] != null && tempEddb[textValues["VAEDcurrentSystem"]]["stations"].ContainsKey(textValues["VAEDcurrentStarport"]))
                    {
                        textValues["VAEDeddbStarportId"] = tempEddb[textValues["VAEDcurrentSystem"]]["stations"][textValues["VAEDcurrentStarport"]].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Write("----------------FRONTIER COMPANION DATA--------------------");
            Debug.Write(htmlData);
            Debug.Write(ex.ToString());
            textValues["VAEDprofileStatus"] = "error";
            return false;
        }
        string companionFile = Path.Combine(Config.Path(), "companion.json");
        using (StreamWriter outputFile = new StreamWriter(companionFile))
        {
            outputFile.Write(htmlData);
        }
        return true;
    }
}