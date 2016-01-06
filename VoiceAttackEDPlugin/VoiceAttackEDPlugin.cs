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
            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Voice Attack ED Plugin");
            string debugFile = Path.Combine(appPath, "debug.log");

            if (File.Exists(debugFile))
            {
                File.Delete(debugFile);
            }

            string gamePath = PluginRegistry.getStringValue("GamePath");
            int isSteam = (int) PluginRegistry.getIntegerValue("isSteamGame");

            if (gamePath != null || isSteam == -1)
            {
                Tuple<string, int> tResponse = EliteRegistry.getGameDetails();
                gamePath = tResponse.Item1;
                isSteam = tResponse.Item2;
            }

            string logPath = string.Empty;
            if (gamePath != string.Empty)
            {
                logPath = Path.Combine(gamePath, "Products", "elite-dangerous-64", "Logs");
            }

            state.Add("VAEDgamePath", gamePath);
            state.Add("VAEDlogPath", logPath);
            state.Add("VAEDnetLogFile", String.Empty);

            string configPath = Path.Combine(gamePath, "Products", "elite-dangerous-64");

            //verboseEnabled:  0 - not enabled, 1 - was already enabled, 2 - just enabled now
            int verboseEnabled = Elite.enableVerboseLogging(configPath);
            state.Add("VAEDverboseLoggingEnabled", verboseEnabled);

            Int32 processId = Elite.getPID();
            state.Add("VAEDelitePid", processId);

            long pos = 0;
            state.Add("VAEDcurrentLogPosition", pos);

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

            // Frontier's companion API URLs
            // XXX Make these CONST?
            state.Add("VAEDconfirmURL", "https://companion.orerve.net/user/confirm");
            state.Add("VAEDprofileURL", "https://companion.orerve.net/profile");

            try
            {
                Directory.CreateDirectory(appPath);
            }
            catch
            {
                // XXX Catch this and con't let the Invoke code run
            }

            //Set the cooldown timer to the past so it can be run right away.
            state.Add("VAEDcooldown", DateTime.Now.AddHours(-6));

            string cookieFile = Path.Combine(appPath, "cookies.txt");
            CookieContainer cookieJar = new CookieContainer();
            state.Add("VAEDcookieFile", cookieFile);

            state.Add("VAEDloggedIn", "no");

            state.Add("VAEDdebug", false);

            if (haveConfig)
            {
                if (File.Exists(cookieFile))
                {
                    cookieJar = Companion.ReadCookiesFromDisk(cookieFile);
                    state.Add("VAEDcookieContainer", cookieJar);
                    state["VAEDloggedIn"] = "yes";
                }
                else
                {
                    
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

            // Accept FD's certificate
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            if (textValues.ContainsKey("VAEDcommand"))
            {
                Utility.writeDebug("Found a VAEDcommand:  " + textValues["VAEDcommand"]);
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
                                Utility.writeDebug("in send verification code is:  " + textValues["VAEDvalue"]);

                                string sendData = "code=" + textValues["VAEDvalue"];

                                CookieContainer cookieContainer = (CookieContainer)state["VAEDcookieContainer"];
                                Tuple<CookieContainer, string> tRespon = Companion.sendRequest(state["VAEDconfirmURL"].ToString(), cookieContainer, state["VAEDconfirmURL"].ToString(), sendData);
                                state["VAEDcookieContainer"] = tRespon.Item1;

                                Companion.WriteCookiesToDisk(state["VAEDcookieFile"].ToString(), tRespon.Item1);

                                //XXX this is broken for now - verification returns a 404 error - why?
                                // Currently we just assume we're logged in.
                                state["VAEDloggedIn"] = "yes";
                                textValues.Add("VAEDprofileStatus", "yes");
                                break;
                            }
                        }
                        else if (state["VAEDloggedIn"].ToString() == "no")
                        {
                            Utility.writeDebug("in send verification - state says we are not logged in");
                            textValues.Add("VAEDprofileStatus", "no");
                            break;
                        }
                        Utility.writeDebug("in send verification - fall through");
                        break;

                    case "profile":
                        if (state["VAEDloggedIn"].ToString() == "verification")
                        {
                            textValues.Add("VAEDprofileStatus", "verification");
                            Utility.writeDebug("In profile, request verification");
                            break;
                        }
                        else if ((!state.ContainsKey("VAEDcookieContainer") && state["VAEDloggedIn"].ToString() == "yes") || state["VAEDloggedIn"].ToString() == "no" || state["VAEDloggedIn"].ToString() == "error")
                        {
                            Utility.writeDebug("Need to get logged in"); 
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
                            Utility.writeDebug("in profile - have credentials");

                            DateTime lastRun = (DateTime)state["VAEDcooldown"];

                            DateTime oneMinuteAgo = DateTime.Now.AddMinutes(-1);

                            if (lastRun.CompareTo(oneMinuteAgo) > 0)
                            {
                                Utility.writeDebug("COOLDOWN IN Progress");
                                break;
                            }

                            textValues.Add("VAEDprofileStatus", "yes");

                            CookieContainer cookieContainer = (CookieContainer)state["VAEDcookieContainer"];

                            state["VAEDcooldown"] = DateTime.Now;

                            Tuple<CookieContainer, string> tRespon = Companion.sendRequest(state["VAEDprofileURL"].ToString(), cookieContainer);
                            state["VAEDcookieContainer"] = tRespon.Item1;
                            Companion.WriteCookiesToDisk(state["VAEDcookieFile"].ToString(), tRespon.Item1);

                            Utility.writeDebug("in profile: getProfilePage returned: " + tRespon.Item2);
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            var result = serializer.Deserialize<Dictionary<string, dynamic>>(tRespon.Item2);

                            Boolean currentlyDocked = false;
                            
                            try
                            {
                                string cmdr = result["commander"]["name"];
                                int credits = result["commander"]["credits"];
                                int debt = result["commander"]["debt"];
                                int currentShipId = result["commander"]["currentShipId"];
                                currentlyDocked = result["commander"]["docked"];
                                string combatRank = RankMapping.combatRankToString(result["commander"]["rank"]["combat"]);
                                string tradeRank = RankMapping.tradeRankToString(result["commander"]["rank"]["trade"]);
                                string exploreRank = RankMapping.exploreRankToString(result["commander"]["rank"]["explore"]);
                                string cqcRank = RankMapping.cqcRankToString(result["commander"]["rank"]["cqc"]);

                                string federationRank = RankMapping.federationRankToString(result["commander"]["rank"]["federation"]);
                                string empireRank = RankMapping.empireRankToString(result["commander"]["rank"]["empire"]);

                                string powerPlayRank = RankMapping.powerPlayRankToString(result["commander"]["rank"]["power"]);

                                string lastDockedSystem = result["lastSystem"]["name"];
                                string lastDockedStarport = result["lastStarport"]["name"];


                                int cargoCapacity = result["ship"]["cargo"]["capacity"];
                                int quantityInCargo = result["ship"]["cargo"]["qty"];

                                ArrayList allShips = result["ships"];

                                int howManyShips = allShips.Count;

                                string currentShip = result["ships"][currentShipId]["name"];

                                Utility.writeDebug("CMDR: " + cmdr);
                                Utility.writeDebug("Credits: " + credits.ToString());
                                Utility.writeDebug("Loan: " + debt.ToString());
                                Utility.writeDebug("Are Currently Docked: " + currentlyDocked.ToString());
                                Utility.writeDebug("Combat Rank: " + combatRank);
                                Utility.writeDebug("Explorer Rank: " + exploreRank);
                                Utility.writeDebug("Trader Rank: " + tradeRank);
                                Utility.writeDebug("CQC Rank: " + cqcRank);
                                Utility.writeDebug("Federation Rank: " + federationRank);
                                Utility.writeDebug("Empire Rank: " + empireRank);
                                Utility.writeDebug("Power Play Rank: " + powerPlayRank);
                                Utility.writeDebug("Last Docked System: " + lastDockedSystem);
                                Utility.writeDebug("Last Docked Starport: " + lastDockedStarport);
                                Utility.writeDebug("Current Ship: " + currentShip);
                                Utility.writeDebug("How Many Ships: " + howManyShips.ToString());
                                Utility.writeDebug("Total Cargo Capacity: " + cargoCapacity.ToString());
                                Utility.writeDebug("Quantity In Cargo: " + quantityInCargo.ToString());

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


                                //Current sincee we are docked
                                textValues["VAEDcurrentSystem"] = lastDockedSystem;
                                textValues["VAEDcurrentStarport"] = lastDockedStarport;

                            }
                            catch (Exception ex)
                            {
                                Utility.writeDebug("Error - unable to parse output " + ex.ToString());
                            }

                            if (! currentlyDocked)
                            { 
                                // If not docked, then we get the System information from the log file
                                long currentPosition = (long)state["VAEDcurrentLogPosition"];
                                Int32 elitePid = (Int32)state["VAEDelitePid"];

                                string logPath = state["VAEDlogPath"].ToString();
                                string filename = state["VAEDnetLogFile"].ToString();

                                Tuple<Boolean, string, string, long, Int32> tLogReturn = Elite.tailNetLog(logPath, filename, currentPosition, elitePid);
                                //string newTimestamp = tLogReturn.Item1;

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
                            
                        }          

                        break;

                    default:
                        Utility.writeDebug("ERROR: Unexpected condition");
                        break;
                }
            }          
        }
    }
}
