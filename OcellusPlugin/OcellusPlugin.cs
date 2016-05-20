using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
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
        public const string pluginVersion = "0.3";

        private static string dictateSystem(ref Dictionary<string, object> state)
        {
            if (! state.ContainsKey("VAEDrecognitionEngine"))
            {
                Debug.Write("Error:  Speech engine not initialized");
                return null;
            }

            SpeechRecognitionEngine recognitionEngine = (SpeechRecognitionEngine)state["VAEDrecognitionEngine"];
            recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            recognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1);
            RecognitionResult result = recognitionEngine.Recognize(TimeSpan.FromSeconds(5));
            if (result != null)
            {
                return result.Text;
            }
            return null; 
        }
        public static string VA_DisplayName()
        {
            return pluginName + " v. " + pluginVersion;  
        }

        public static string VA_DisplayInfo()
        {
            return "VoiceAttack Plugin\r\n\r\nFor use with Elite: Dangerous\r\n\r";  
        } 

        public static Guid VA_Id()
        {
            return new Guid("{A818A69A-D6A9-4CC8-943C-E6ABBAF4C76C}");  
        }

        public static void VA_Init1(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, bool?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            // Setup Speech engine

            if (EliteGrammar.downloadGrammar() == true)
            {
                SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
                recognitionEngine.SetInputToDefaultAudioDevice();
                Grammar grammar = new Grammar(Path.Combine(Config.Path(), "SystemsGrammar.xml"));
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
                else
                {
                }
            }
            else
            {
                
                state.Add("VAEDloggedIn", "no");
            }

            EliteBinds eliteBinds = new EliteBinds();
            state.Add("VAEDeliteBinds", eliteBinds);
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
                switch (context)
                {
                    case "check upgrade":
                        if (Upgrade.needUpgrade(ref state))
                        {
                            booleanValues["VAEDneedUpgrade"] = true;
                        }
                        else
                        {
                            booleanValues["VAEDneedUpgrade"] = false;
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
                                    booleanValues["VAEDerror"] = true;
                                    break;
                                }
                                Dictionary<string, dynamic> tempAtlas = (Dictionary<string, dynamic>)state["VAEDatlasIndex"];

                                double distance = Atlas.calcDistance(ref tempAtlas, currentSystem, textValues["VAEDtargetSystem"]);

                                decimalValues["VAEDdecimalDistance"] = (decimal)distance;
                                intValues["VAEDintDistance"] = (int)(distance + .5);
                                booleanValues["VAEDerror"] = false;
                                break;
                            }
                        }
                        booleanValues["VAEDerror"] = true;
                        break;
                    case "dictate system":
                        string system = dictateSystem(ref state);
                        textValues["VAEDdictateSystem"] = system;
                        break;
                    case "send key":
                        EliteBinds eliteBinds = (EliteBinds)state["VAEDeliteBinds"];
                        string[] parts = textValues["VAEDsendKey"].Split(new char[] { ':' }, 2);

                        List<ushort> scanCodes = eliteBinds.GetCodes(parts[1]);
                        booleanValues["VAEDsendKeyError"] = false;
                        switch (parts[0])
                        {
                            case "KEYPRESS":
                                KeyMouse.KeyPress(scanCodes);
                                break;
                            case "KEYUP":
                                KeyMouse.KeyUp(scanCodes);
                                break;
                            case "KEYDOWN":
                                KeyMouse.KeyDown(scanCodes);
                                break;
                            default:
                                booleanValues["VAEDsendKeyError"] = true;
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
                    case "export for coriolis":
                        string json = Coriolis.createCoriolisJson(ref state);
                        Clipboard.SetText(json);
                        Debug.Write("------------------ Coriolis JSON Follows ---------------------");
                        Debug.Write(json);
                        break;
                    case "edit web variables":
                        var webVarsForm = new WebVars.EditWebVars();
                        webVarsForm.ShowDialog();
                        break;
                    case "update web vars":
                        GetWebVars.readWebVars(ref state, ref textValues, ref intValues, ref booleanValues);
                        break;
                    case "update file vars":
                        FileVar.readFileVars(ref state, ref textValues, ref intValues, ref booleanValues);
                        break;
                    case "clipboard":
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                        {
                            textValues.Add("VAEDclipboard", Clipboard.GetText());
                        }
                        break;
                    case "set credentials":
                        var credentialsForm = new Credentials.Login();
                        credentialsForm.ShowDialog();
                        CookieContainer loginCookies = credentialsForm.Cookie;
                        state["VAEDcookieContainer"] = loginCookies;
                        string loginResponse = credentialsForm.LoginResponse;
                        Debug.Write("LoginResponse: " + loginResponse);
                        textValues["VAEDloggedIn"] = loginResponse;
                        break;
                    case "verification":
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
                    case "update eddn":
                        Eddn.updateEddn(ref state);
                        break;
                    case "profile":
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