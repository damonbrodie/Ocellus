using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Speech.Recognition;


// *****************************
// *  Voice Attack functions   *
// *****************************
namespace OcellusPlugin
{   
    public class OcellusPlugin
    {
        public const string pluginName = "Ocellus - Elite: Dangerous Assistant";
        public const string pluginVersion = "0.7";
        public const string eliteWindowTitle = "Elite - Dangerous (CLIENT)";

        public static string VA_DisplayName()
        {
            return pluginName + " v. " + pluginVersion;  
        }

        public static string VA_DisplayInfo()
        {
            return "Ocellus VoiceAttack Plugin\r\n\r\nFor use with Elite: Dangerous\r\n\r";  
        } 

        public static Guid VA_Id()
        {
            return new Guid("{A818A69A-D6A9-4CC8-943C-E6ABBAF4C76C}");  
        }

        public static void VA_Init1(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, bool?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            try
            {
                Debug.Write("---------------------- Ocellus Plugin Initializing ----------------------");
                // Setup Speech engine
                if (EliteGrammar.downloadGrammar())
                {
                    SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
                    recognitionEngine.SetInputToDefaultAudioDevice();
                    Grammar grammar = new Grammar(Path.Combine(Config.Path(), "systems_grammar.xml"));
                    Task.Run(() => recognitionEngine.LoadGrammar(grammar));
                    state.Add("VAEDrecognitionEngine", recognitionEngine);
                }

                // Setup plugin storage directory - used for cookies and debug logs
                string appPath = Config.Path();
                string cookieFile = Config.CookiePath();
                string debugFile = Config.DebugPath();
                textValues["VAEDdebugPath"] = debugFile;

                // Determine Elite Dangerous directories
                string gamePath = Elite.getGamePath();
                string gameStartString = PluginRegistry.getStringValue("startPath");
                string gameStartParams = PluginRegistry.getStringValue("startParams");
                state.Add("VAEDgamePath", gamePath);
                textValues["VAEDgameStartString"] = gameStartString;
                textValues["VAEDgameStartParams"] = gameStartParams;

                // Load EDDB Index into memory
                Eddb.loadEddbIndex(ref state);

                // Load Atlas Index into memory
                Atlas.loadAtlasIndex(ref state);

                Dictionary<string, dynamic> tempAtlas = (Dictionary<string, dynamic>)state["VAEDatlasIndex"];

                // Load Tracked Systems into memory
                TrackSystems.Load(ref state);

                CookieContainer cookieJar = new CookieContainer();
                if (File.Exists(cookieFile))
                {
                    // If we have cookies then we are likely already logged in
                    cookieJar = Web.ReadCookiesFromDisk(cookieFile);
                    Tuple<CookieContainer, string> tAuthentication = Companion.loginToAPI(cookieJar);
                    if (tAuthentication.Item2 == "ok")
                    {
                        cookieJar = tAuthentication.Item1;
                        state.Add("VAEDcookieContainer", cookieJar);
                        state["VAEDloggedIn"] = "ok";
                    }
                }
                else
                {
                    state.Add("VAEDloggedIn", "no");
                }

                EliteBinds eliteBinds = new EliteBinds();
                state.Add("VAEDeliteBinds", eliteBinds);
                string bindsFile = Elite.getBindsFilename();
                DateTime fileTime = File.GetLastWriteTime(bindsFile);
                state.Add("VAEDbindsFile", bindsFile);
                state.Add("VAEDbindsTimestamp", fileTime);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
            //this function gets called when VoiceAttack is closing (normally).  You would put your cleanup code in here, but be aware that your code must be robust enough to not absolutely depend on this function being called
        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, bool?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            try
            {
                Debug.Write("COMMAND:  " + context);
                switch (context.ToLower())
                {
                    case "check for upgrade":
                        if (Upgrade.needUpgrade(ref state))
                        {
                            booleanValues["VAEDupgradeAvailable"] = true;
                            state["VAEDupgradeAvailable"] = true;
                        }
                        else
                        {
                            booleanValues["VAEDupgradeAvailable"] = false;
                            state["VAEDupgradeAvailable"] = false;
                        }
                        break;
                    case "distance from here":
                        bool worked = Companion.updateProfile(ref state, ref shortIntValues, ref textValues, ref intValues, ref decimalValues, ref booleanValues);
                        decimalValues["VAEDdecimalDistance"] = null;
                        decimalValues["VAEDintDistance"] = null;
                        if (worked)
                        {
                            if (state.ContainsKey("VAEDcompanionDict") && textValues.ContainsKey("VAEDtargetSystem"))
                            {
                                Dictionary<string, dynamic> companion = (Dictionary<string, dynamic>)state["VAEDcompanionDict"];
                                string currentSystem = "";
                                if (companion.ContainsKey("lastSystem") && companion["lastSystem"].ContainsKey("name"))
                                {
                                    currentSystem = companion["lastSystem"]["name"];
                                }
                                else
                                {
                                    Debug.Write("Error:  Could not determine current location for command 'distance from here'");
                                    booleanValues["VAEDerrorSourceSystem"] = true;
                                    break;
                                }
                                Dictionary<string, dynamic> tempAtlas = (Dictionary<string, dynamic>)state["VAEDatlasIndex"];

                                int distance = Atlas.calcDistance(ref tempAtlas, currentSystem, textValues["VAEDtargetSystem"]);
                                if (distance < 0)
                                {
                                    //Cound not find destination system
                                    Debug.Write("Error:  Could not determine distance to target system");
                                    booleanValues["VAEDerrorTargetSystem"] = true;
                                    break;
                                }
                                // Found the system - return distance
                                intValues["VAEDintDistance"] = distance;
                                booleanValues["VAEDerrorTargetSystem"] = false;
                                booleanValues["VAEDerrorSourceSystem"] = false;
                                break;
                            }
                        }
                        //Can't find the System
                        booleanValues["VAEDerrorSourceSystem"] = true;
                        booleanValues["VAEDerrorDestinationSystem"] = false;
                        break;
                    case "dictate system":
                        if (state.ContainsKey("VAEDrecognitionEngine"))
                        {
                            SpeechRecognitionEngine recognitionEngine = (SpeechRecognitionEngine)state["VAEDrecognitionEngine"];

                            Tuple<string,string> tSystemNames = EliteGrammar.dictateSystem(recognitionEngine, (List<String>)state["VAEDtrackedSystems"]);
                            textValues["VAEDdictateSystem"] = tSystemNames.Item1;
                            textValues["VAEDdictateSystemPhonetic"] = tSystemNames.Item2;
                            break;
                        }
                        else
                        {
                            Debug.Write("Error:  Speech Engine not yet Initialized.  (Possibly still loading)");
                        }
                        textValues["VAEDdictateSystem"] = null;
                        textValues["VAEDdictateSystemPhonetic"] = null;
                        break;
                    case "press key bind":
                        // If the Binds file changes then reload the binds.
                        string bindsFile = (string)state["VAEDbindsFile"];
                        DateTime oldTimestamp = (DateTime)state["VAEDbindsTimestamp"];

                        DateTime newTimestamp = File.GetLastWriteTime(bindsFile);
                        EliteBinds eliteBinds;
                        if (oldTimestamp != newTimestamp)
                        {
                            Debug.Write("Binds file change:  reloading");
                            eliteBinds = new EliteBinds();
                            state["VAEDbindsTimestamp"] = newTimestamp;
                            state["VAEDeliteBinds"] = eliteBinds;
                        }
                        else
                        {
                            eliteBinds = (EliteBinds)state["VAEDeliteBinds"];
                        }
                        string[] parts = textValues["VAEDkeyBinding"].Split(new char[] { ':' }, 2);
                        List<uint> scanCodeExs = KeyMouse.MapVkToScanCodeExs(eliteBinds.GetCodes(parts[1]));
                        if (scanCodeExs.Count == 0)
                        {
                            Debug.Write("Warning: No key binding found for: " + textValues["VAEDkeyBinding"]);
                            booleanValues["VAEDkeyBindingError"] = true;
                            break;
                        }
                        booleanValues["VAEDkeyBindingError"] = false; 
                        switch (parts[0])
                        {
                            // For now we only "best effort" focus the game before keypressing.  Igorning the setFocus return code.
                            case "KEYPRESS":
                                User32.setFocus(eliteWindowTitle);
                                KeyMouse.KeyPress(scanCodeExs);
                                break;
                            case "KEYUP":
                                User32.setFocus(eliteWindowTitle);
                                KeyMouse.KeyUp(scanCodeExs);
                                break;
                            case "KEYDOWN":
                                User32.setFocus(eliteWindowTitle);
                                KeyMouse.KeyDown(scanCodeExs);
                                break;
                            default:
                                booleanValues["VAEDkeyBindingError"] = true;
                                break;
                        }
                        break;
                    case "clear debug":
                        Debug.Clear();
                        break;
                    case "get debug":
                        string tempDebug = Debug.Path();
                        textValues["VAEDdebugFile"] = tempDebug;
                        break;
                    case "export for ed shipyard":
                        Companion.updateProfile(ref state, ref shortIntValues, ref textValues, ref intValues, ref decimalValues, ref booleanValues);
                        
                        if (state.ContainsKey("VAEDshipObj"))
                        {
                            Ship.Components shipObj = (Ship.Components)state["VAEDshipObj"];
                            StringBuilder export = EDShipyard.export(shipObj);
                            if (export != null)
                            {
                                booleanValues["VAEDexportEDShipyardError"] = false;
                                Clipboard.SetText(export.ToString());
                                break;
                            }
                        }
                        Debug.Write("Error:  Unable to form ED Shipyard.com Export");
                        Clipboard.Clear();
                        booleanValues["VAEDexportEDShipyuardError"] = true;
                        break;
                    case "export for coriolis":
                        Companion.updateProfile(ref state, ref shortIntValues, ref textValues, ref intValues, ref decimalValues, ref booleanValues);
                        if (state.ContainsKey("VAEDshipObj"))
                        {
                            Ship.Components shipObj = (Ship.Components)state["VAEDshipObj"];
                            string json = Coriolis.export(shipObj);
                            if (json != null)
                            {
                                booleanValues["VAEDexportCoriolisError"] = false;
                                Clipboard.SetText(json);
                                break;
                            }
                        }
                        Debug.Write("Error:  Unable to form Coriolis.io JSON");
                        Clipboard.Clear();
                        booleanValues["VAEDexportCoriolisError"] = true;
                        break;
                    case "edit web variable sources":
                        bool foundWindow = false;
                        foreach (Form form in Application.OpenForms)
                            if (form.GetType().Name == "EditWebVars")
                            {
                                Debug.Write("Edit Web Variable Sources window is already open");
                                foundWindow = true;
                            }
                        if (!foundWindow)
                        {
                            var webVarsForm = new WebVars.EditWebVars();
                            webVarsForm.ShowDialog();
                        }
                        break;
                    case "get web variables":
                        GetWebVars.readWebVars(ref state, ref textValues, ref intValues, ref booleanValues);
                        break;
                    case "get file variables":
                        FileVar.readFileVars(ref state, ref textValues, ref intValues, ref booleanValues);
                        break;
                    case "get clipboard":
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                        {
                            textValues.Add("VAEDclipboard", Clipboard.GetText());
                        }
                        break;
                    case "get frontier credentials":
                        var credentialsForm = new Credentials.Login();
                        credentialsForm.ShowDialog();
                        CookieContainer loginCookies = credentialsForm.Cookie;
                        state["VAEDcookieContainer"] = loginCookies;
                        string loginResponse = credentialsForm.LoginResponse;
                        Debug.Write("LoginResponse: " + loginResponse);
                        textValues["VAEDloggedIn"] = loginResponse;
                        break;
                    case "get frontier verification":
                        CookieContainer verifyCookies = new CookieContainer();
                        if (state.ContainsKey("VAEDcookieContainer"))
                        {
                            verifyCookies = (CookieContainer)state["VAEDcookieContainer"];
                        }
                        var verificationForm = new VerificationCode.Validate();
                        verificationForm.Cookie = verifyCookies;
                        verificationForm.ShowDialog();
                        verifyCookies = verificationForm.Cookie;
                        string verifyResponse = verificationForm.VerifyResponse;
                        state["VAEDloggedIn"] = verifyResponse;
                        state["VAEDcookieContainer"] = verifyCookies;
                        textValues["VAEDloggedIn"] = verifyResponse;
                        if (verifyResponse == "ok")
                        {
                            Web.WriteCookiesToDisk(Config.CookiePath(), verifyCookies);
                        }
                        break;
                    case "update profile and eddn":
                        if (state["VAEDloggedIn"].ToString() == "ok" && state.ContainsKey("VAEDcookieContainer"))
                        {
                            Companion.updateProfile(ref state, ref shortIntValues, ref textValues, ref intValues, ref decimalValues, ref booleanValues);
                        }
                        else // Not logged in
                        {
                            textValues["VAEDprofileStatus"] = "credentials";
                        }
                        break;

                    default:
                        Debug.Write("ERROR: unknown command");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }          
        }
    }
}