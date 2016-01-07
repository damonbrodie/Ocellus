using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Web.Script.Serialization;


// *****************************
// *  Voice Attack functions   *
// *****************************

namespace VoiceAttackEDPlugin
{   
    public class VoiceAttackPlugin
    {
        private const string pluginVersion = "0.1";

        public static string VA_DisplayName()
        {
            return "Voice Attack EDPlugin " + pluginVersion;  
        }

        public static string VA_DisplayInfo()
        {
            return "VoiceAttackPlugin\r\n\r\nTo be used with Elite: Dangerous\r\n\r";  
        } 

        public static Guid VA_Id()
        {

            return new Guid("{A818A69A-D6A9-4CC8-943C-E6ABBAF4C76C}");  
        }

        public static void VA_Init1(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            // Setup plugin storage directory - used for cookies and debug logs
            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Voice Attack ED Plugin");
            string debugFile = Path.Combine(appPath, "debug.log");

            if (File.Exists(debugFile))
            {
                File.Delete(debugFile);
            }

            try
            {
                Directory.CreateDirectory(appPath);
            }
            catch
            {
                // XXX Catch this and con't let the Invoke code run
            }


            // Determing Elite Dangerous game directory
            string gamePath = PluginRegistry.getStringValue("GamePath");
            int isSteam = (int) PluginRegistry.getIntegerValue("isSteamGame");

            if (gamePath != null || isSteam == -1)
            {
                Tuple<string, int> tResponse = EliteRegistry.getGameDetails();
                gamePath = tResponse.Item1;
                isSteam = tResponse.Item2;
            }


            // XXX need to make this work for Season 1 of ED - both 32 and 64 bit versions
            string logPath = string.Empty;
            if (gamePath != string.Empty)
            {
                logPath = Path.Combine(gamePath, "Products", "elite-dangerous-64", "Logs");
            }

            state.Add("VAEDgamePath", gamePath);
            state.Add("VAEDlogPath", logPath);
            state.Add("VAEDnetLogFile", String.Empty);


            // verboseEnabled:  0 - not enabled, 1 - was already enabled, 2 - just enabled now
            string gameConfigPath = Path.Combine(gamePath, "Products", "elite-dangerous-64");
            int verboseEnabled = Elite.enableVerboseLogging(gameConfigPath);
            state.Add("VAEDverboseLoggingEnabled", verboseEnabled);


            // Determine if ED is currently running
            Int32 processId = Elite.getPID();
            state.Add("VAEDelitePid", processId);


            // Reset current location in netLog reading to the top
            long pos = 0;
            state.Add("VAEDcurrentLogPosition", pos);


            //Set the cooldown timer to the past so it can be run right away.
            state.Add("VAEDcooldown", DateTime.Now.AddHours(-6));


            // Get FD credentials from registry
            Boolean haveConfig = true;

            string FDemail = PluginRegistry.getStringValue("email");
            if (FDemail != "null")
            {
                state.Add("VAEDemail", FDemail);
            }
            else
            {
                haveConfig = false;
            }

            string FDpassword = PluginRegistry.getStringValue("password");
            if (FDpassword != "null")
            {
                state.Add("VAEDpassword", FDpassword);
            }
            else
            {
                haveConfig = false;
            }

            string cookieFile = Path.Combine(appPath, "cookies.txt");
            CookieContainer cookieJar = new CookieContainer();
            state.Add("VAEDcookieFile", cookieFile);

            state.Add("VAEDloggedIn", "no");

            if (haveConfig)
            {
                if (File.Exists(cookieFile))
                {
                    // If we have cookies then we are likely already logged in
                    cookieJar = Companion.ReadCookiesFromDisk(cookieFile);
                    state.Add("VAEDcookieContainer", cookieJar);
                    state["VAEDloggedIn"] = "yes";
                }
                else
                {
                    // Log into the API
                    Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI(FDemail, FDpassword);

                    string loginResponse = tAuthentication.Item2;
                    state["VAEDloggedIn"] = loginResponse;

                    if (loginResponse == "yes" || loginResponse == "verification")
                    {
                        cookieJar = tAuthentication.Item1;
                        state.Add("VAEDcookieContainer", cookieJar);
                    }

                }
            }
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
            //this function gets called when VoiceAttack is closing (normally).  You would put your cleanup code in here, but be aware that your code must be robust enough to not absolutely depend on this function being called

        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            if (textValues.ContainsKey("VAEDcommand"))
            {
                Utility.writeDebug("COMMAND:  " + textValues["VAEDcommand"]);
                switch (textValues["VAEDcommand"])
                {

                    case "clipboard":
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                        {
                            textValues.Add("VAEDclipboard", Clipboard.GetText());

                        }
                        break;

                    case "save email":
                        if(!PluginRegistry.setStringValue("email", textValues["VAEDvalue"]))
                        {
                            textValues["VAEDerror"] = "registry";
                        }                        

                        if (state.ContainsKey("VAEDemail"))
                        {
                            state["VAEDemail"] = textValues["VAEDvalue"];
                        }
                        else
                        {
                            state.Add("VAEDemail", textValues["VAEDvalue"]);
                        }
                        break;

                    case "save password":
                        if(!PluginRegistry.setStringValue("password", textValues["VAEDvalue"]))
                        {
                            textValues["VAEDerror"] = "registry";
                        }

                        if (state.ContainsKey("VAEDpassword"))
                        {
                            state["VAEDpassword"] = textValues["VAEDvalue"];
                        }
                        else
                        {
                            state.Add("VAEDpassword", textValues["VAEDvalue"]);
                        }
                        break;

                    case "send verification":
                        if (state["VAEDloggedIn"].ToString() == "verification")
                        {
                            if (textValues.ContainsKey("VAEDvalue"))
                            {
                                CookieContainer cookieContainer = (CookieContainer)state["VAEDcookieContainer"];
                                
                                Tuple<CookieContainer, string> tRespon = Companion.verifyWithAPI(cookieContainer, textValues["VAEDvalue"]);
                                state["VAEDcookieContainer"] = tRespon.Item1;

                                Companion.WriteCookiesToDisk(state["VAEDcookieFile"].ToString(), tRespon.Item1);

                                state["VAEDloggedIn"] = "yes";
                                textValues.Add("VAEDprofileStatus", "yes");
                                break;
                            }
                        }
                        else if (state["VAEDloggedIn"].ToString() == "no")
                        {
                            Utility.writeDebug("Not logged in - can't verify");
                            textValues.Add("VAEDprofileStatus", "no");
                            break;
                        }
                        Utility.writeDebug("Error:  Unknown verification problem");
                        break;

                    case "profile":
                        if (state["VAEDloggedIn"].ToString() == "verification")
                        {
                            textValues.Add("VAEDprofileStatus", "verification");
                            Utility.writeDebug("Verification needed before we can get the profile");
                            break;
                        }
                        else if ((!state.ContainsKey("VAEDcookieContainer") && state["VAEDloggedIn"].ToString() == "yes") || state["VAEDloggedIn"].ToString() == "no" || state["VAEDloggedIn"].ToString() == "error")
                        {
                            Utility.writeDebug("Login needed before we can get the profile"); 
                            // If we're not logged in, attempt to do so
                            if (state.ContainsKey("VAEDemail") && state.ContainsKey("VAEDpassword"))
                            {
                                string FDemail = state["VAEDemail"].ToString();
                                string FDpassword = state["VAEDpassword"].ToString();

                                Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI(FDemail, FDpassword);

                                string loginResponse = tAuthentication.Item2;
                                state["VAEDloggedIn"] = loginResponse;

                                if (loginResponse == "verification")
                                {
                                    textValues.Add("VAEDprofileStatus", "verification");
                                    CookieContainer cookieJar = tAuthentication.Item1;
                                    state.Add("VAEDcookieContainer", cookieJar);
                                    break;
                                }
                                if (loginResponse == "yes")
                                {
                                    CookieContainer cookieJar = tAuthentication.Item1;
                                    state.Add("VAEDcookieContainer", cookieJar);
                                    // We think we're logged in fall through and see if we can get the profile
                                }
                            }
                            else
                            {
                                // No credentials in registry
                                state["VAEDloggedIn"] = "no";
                                state.Remove("VAEDcookieContainer");
                                textValues.Add("VAEDprofileStatus", "credentials");
                                break;
                            }

                        }
                        if (state["VAEDloggedIn"].ToString() == "yes" && state.ContainsKey("VAEDcookieContainer"))
                        {
                            DateTime lastRun = (DateTime)state["VAEDcooldown"];
                            DateTime oneMinuteAgo = DateTime.Now.AddMinutes(-1);

                            if (lastRun.CompareTo(oneMinuteAgo) > 0)
                            {
                                Utility.writeDebug("COOLDOWN IN Progress");
                                break;
                            }
                            state["VAEDcooldown"] = DateTime.Now;

                            textValues.Add("VAEDprofileStatus", "yes");

                            CookieContainer cookieContainer = (CookieContainer)state["VAEDcookieContainer"];

                            Tuple<CookieContainer, string> tRespon = Companion.getProfile(cookieContainer);

                            state["VAEDcookieContainer"] = tRespon.Item1;
                            Companion.WriteCookiesToDisk(state["VAEDcookieFile"].ToString(), tRespon.Item1);

                            JavaScriptSerializer serializer = new JavaScriptSerializer();

                            Utility.writeDebug(tRespon.Item2);

                            var result = serializer.Deserialize<Dictionary<string, dynamic>>(tRespon.Item2);
                            Boolean currentlyDocked = false;
                            string lastDockedSystem = "";
                            string lastDockedStarport = "";
                            
                            try
                            {
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

                                List<string> keys = new List <string> (allShips.Keys);

                                // Null out ship locations
                                string[] listOfShips = Elite.listofShipsShortNames();
                                foreach (string ship in listOfShips)
                                {
                                    textValues["VAEDship-" + ship] = null;
                                    intValues["VAEDshipCounter-" + ship] = 0;
                                }


                                int counter = 1;
                                foreach (string key in keys)
                                {
                                    string counterString = Utility.numberToString(counter);
                                    string tempShip = result["ships"][key]["name"];
                                    string tempSystem = result["ships"][key]["starsystem"]["name"];
                                    string shipKey = "VAEDship" + counterString;
                                    string shipLocationKey = "VAEDshipLocation" + counterString;
                                    textValues[shipKey] = tempShip;
                                    textValues[shipLocationKey] = tempSystem;
                                    textValues["VAEDship-" + tempShip] = tempSystem;
                                    Utility.writeDebug("tempSHIP: " + tempShip);
                                    intValues["VAEDshipCounter-" + tempShip]++;

                                    counter++;
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
                                textValues["VAEDcurrentShip"] = currentShip;
                                intValues["VAEDcargoCapacity"] = cargoCapacity;
                                intValues["VAEDquantityInCargo"] = quantityInCargo;
                            }
                            catch (Exception ex)
                            {
                                Utility.writeDebug("Error: Unable to parse Companion API output " + ex.ToString());
                            }

                            if (currentlyDocked)
                            {
                                textValues["VAEDcurrentSystem"] = lastDockedSystem;
                                textValues["VAEDcurrentStarport"] = lastDockedStarport;
                            }
                            else
                            { 
                                // If not docked, then we get the System information from the netLog file
                                long currentPosition = (long)state["VAEDcurrentLogPosition"];
                                Int32 elitePid = (Int32)state["VAEDelitePid"];

                                string logPath = state["VAEDlogPath"].ToString();
                                string filename = state["VAEDnetLogFile"].ToString();

                                Tuple<Boolean, string, string, long, Int32> tLogReturn = Elite.tailNetLog(logPath, filename, currentPosition, elitePid);

                                Boolean success = tLogReturn.Item1;
                                string currentSystem = tLogReturn.Item2.ToString();
                                filename = tLogReturn.Item3.ToString();
                                currentPosition = tLogReturn.Item4;
                                elitePid = tLogReturn.Item5;

                                state["VAEDnetLogFile"] = filename;
                                state["VAEDcurrentLogPosition"] = currentPosition;
                                state["VAEDelitePid"] = elitePid;

                                textValues["VAEDcurrentStartport"] = null;
                                if (success)
                                {
                                    textValues["VAEDcurrentSystem"] = currentSystem;
                                }
                                else
                                {
                                    textValues["VAEDcurrentSystem"] = null;
                                }
                            }
                            if (1 == 1)
                            {
                                Utility.writeDebug("TEXT VALUES");
                                foreach(string key in textValues.Keys)
                                {
                                    if (textValues[key] != null)
                                    {
                                        Utility.writeDebug(key + ":  " + textValues[key]);
                                    }
                                    
                                }

                                Utility.writeDebug("INTEGER VALUES");
                                foreach (string key in intValues.Keys)
                                {
                                    if (intValues[key] != null)
                                    {
                                        Utility.writeDebug(key + ":  " + intValues[key].ToString());
                                    }
                                }

                                Utility.writeDebug("BOOLEAN VALUES");
                                foreach (string key in booleanValues.Keys)
                                {
                                    if (booleanValues[key] != null)
                                    {
                                        Utility.writeDebug(key + ":  " + booleanValues[key].ToString());
                                    }
                                }

                            }
                        }          
                        break;

                    default:
                        Utility.writeDebug("Error: unknown command");
                        break;
                }
            }          
        }
    }
}
