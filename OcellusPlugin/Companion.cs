using System;
using System.Net;
using System.Collections.Generic;
using System.Web.Script.Serialization;


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


        int profileCooldown = Utilities.isCoolingDown(ref state, "VAEDprofileCooldown");
        if (profileCooldown > 0)
        {
            Debug.Write("Get Profile is cooling down: " + profileCooldown.ToString() + " seconds remain.");
            return true;
        }

        textValues["VAEDprofileStatus"] = "ok";

        CookieContainer profileCookies = (CookieContainer)state["VAEDcookieContainer"];

        Tuple<CookieContainer, string> tRespon = Companion.getProfile(profileCookies);

        state["VAEDcookieContainer"] = tRespon.Item1;
        Web.WriteCookiesToDisk(Config.CookiePath(), tRespon.Item1);
        string htmlData = tRespon.Item2;

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        string currentSystem = "";
        bool currentlyDocked = false;
        var result = new Dictionary<string, dynamic>();
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

            Dictionary<string, object> allShips = result["ships"];
            int howManyShips = allShips.Count;
            int cargoCapacity = result["ship"]["cargo"]["capacity"];
            int quantityInCargo = result["ship"]["cargo"]["qty"];
            string currentShip = result["ships"][currentShipId]["name"];
            result["commander"]["currentShip"] = currentShip;

            List<string> keys = new List<string>(allShips.Keys);

            //Set current System
            textValues["VAEDcurrentSystem"] = null;
            if (result.ContainsKey("lastSystem") && result["lastSystem"].ContainsKey("name"))
            {
                textValues["VAEDcurrentSystem"] = result["lastSystem"]["name"];
            }

            // Null out ship locations
            string[] listOfShips = Elite.listOfShipVariableNames();
            foreach (string ship in listOfShips)
            {
                textValues["VAEDship-" + ship + "-1"] = null;
                intValues["VAEDshipCounter-" + ship] = 0;
            }

            Dictionary<string, dynamic> theShips = new Dictionary<string, dynamic>();
            foreach (string key in keys)
            {
                string tempShip = result["ships"][key]["name"];

                string tempSystem = null;
                if (result["ships"][key].ContainsKey("starsystem"))
                {
                    tempSystem = result["ships"][key]["starsystem"]["name"];
                    if (key == currentShipId)
                    {
                        currentSystem = tempSystem;
                    }
                }
                int currDistance = -1;
                if (textValues.ContainsKey("VAEDcurrentSystem") && textValues["VAEDcurrentSystem"] != null && state.ContainsKey("VAEDatlasIndex"))
                {
                    Dictionary<string, dynamic> tempAtlas = (Dictionary<string, dynamic>)state["VAEDatlasIndex"];
                    currDistance = Atlas.calcDistance(ref tempAtlas, textValues["VAEDcurrentSystem"], currentSystem);
                }
                //theShips.Add(   
            }

            foreach (string key in keys)
            {
                string tempShip = result["ships"][key]["name"];

                string tempSystem = null;
                if (result["ships"][key].ContainsKey("starsystem"))
                {
                    tempSystem = result["ships"][key]["starsystem"]["name"];
                }
                string variableShipName = Elite.frontierShipToVariable(tempShip);
                string shipCounterString = "VAEDshipCounter-" + variableShipName;
                Debug.Write(tempShip);
                Debug.Write(variableShipName);
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
                    textValues["VAEDambiguousDiamondback"] = textValues["VAEDship-DiamondBackScout-1"];
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
            textValues["VAEDphoneticShip"] = Elite.frontierShipToPhonetic(currentShip);
            intValues["VAEDcargoCapacity"] = cargoCapacity;
            intValues["VAEDquantityInCargo"] = quantityInCargo;
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
        DateTime timestamp = DateTime.Now;
        state["VAEDcompanionTime"] = timestamp.ToString("yyyy-MM-dd") + "T" + timestamp.ToString("H:m:szzz");
        Debug.Write("----------------FRONTIER COMPANION DATA--------------------");
        Debug.Write(htmlData);
        return true;
    }
}