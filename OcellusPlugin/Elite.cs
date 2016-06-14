using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Speech.Recognition;


// **********************************************
// *  Functions for working with ED game files  *
// **********************************************
class Elite
{
    const string uninstallRegistryPath64bit = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
    const string uninstallRegistryPath32bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    public class Speech
    {
        public string text = null;
        public string voice = null;
    }
    public class MessageBus
    {
        public string currentSystem = null;
        public double currentX;
        public double currentY;
        public double currentZ;
        public int isDocked = -1;
        public string loggedinState;
        public CookieContainer cookies;
        public DateTime profileLastUpdate;
        public DateTime eddnLastUpdate;
        public string eddnLastStation;
        public Dictionary<string, dynamic> companion;
        public string ttsVoice = null;
        public string upgradeTTS = null;
        public string updateEddnTTS = null;
        public string startupTTS = null;
        public bool grammarLoaded = false;
        public SpeechRecognitionEngine recognitionEngine;
        public List<Speech> spokenAnnouncements = new List<Speech>();
    }

    public static Tuple<string, string> getBindsFilename()
    {
        var bindsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Frontier Developments\Elite Dangerous\Options\Bindings");

        string bindPreset = Path.Combine(bindsFolder, "StartPreset.start");
        if (File.Exists(bindPreset))
        {
            string bindNamePrefix = File.ReadAllText(bindPreset);
            string bindName = bindNamePrefix + ".1.8.binds";
            string bindsFile = Path.Combine(bindsFolder, bindName);
            if (File.Exists(bindsFile))
            {
                Debug.Write("Found Binds file:  " + bindsFile);
                return Tuple.Create<string, string>(bindPreset, bindsFile);
            }
            else
            {
                string controlSchemePath = getGameControlSchemesPath();
                Debug.Write("control scheme path is " + controlSchemePath);
                bindsFile = Path.Combine(controlSchemePath, bindNamePrefix + ".binds");
                if (File.Exists(bindsFile))
                {
                    return Tuple.Create<string, string>(bindPreset, bindsFile);
                }
                else
                {
                    Debug.Write("No Binds file Found for: " + bindNamePrefix);
                    return Tuple.Create<string, string>(bindPreset, null);
                }
            }
        }
        Debug.Write("Error:  No StartPreset.start file in Elite Binds folder");
        return Tuple.Create<string, string>(null, null);
    }

    private static Tuple<string, double, double, double, int, long> getDataFromNetlog(string logFile, long seekPos)
    {
        long fileLength = 0;

        string currentSystem = null;
        double currentX = new double();
        double currentY = new double();
        double currentZ = new double();
        int currentlyDocked = -1; //Unknown state

 
        //{01:27:13} GetSafeUniversalAddress Station Count 1 moved 0 Docked Not Landed
        string dockedPattern = "^{\\d\\d:\\d\\d:\\d\\d} GetSafeUniversalAddress Station Count \\d+ moved \\d+ Docked";
        Regex dockedRegex = new Regex(dockedPattern);

        //{01:38:58} GetSafeUniversalAddress Station Count 1 moved 1 Undocked Not Landed
        string undockedPattern = "^{\\d\\d:\\d\\d:\\d\\d} GetSafeUniversalAddress Station Count \\d+ moved \\d+ Undocked";
        Regex undockedRegex = new Regex(undockedPattern);

        //{01:25:24} System:"HIP 19758" StarPos:(97.719,-105.469,-31.438)ly  Supercruise
        string systemPattern = "^{\\d\\d:\\d\\d:\\d\\d} System:\"([^\"]+)\" StarPos:\\(([^,]+),([^,]+),([^)]+)";
        Regex systemRegex = new Regex(systemPattern);

        if (File.Exists(logFile))
        {
            FileInfo fileInfo = new FileInfo(logFile);
            fileLength = fileInfo.Length;

            try
            {
                FileStream fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                fs.Seek(seekPos, SeekOrigin.Begin);
                StreamReader ts = new StreamReader(fs);
                string currentLine;
                while ((currentLine = ts.ReadLine()) != null)
                {
                    Match match = systemRegex.Match(currentLine);
                    if (match.Success)
                    {
                        currentSystem = match.Groups[1].Value;
                        currentX = Double.Parse(match.Groups[2].Value);
                        currentY = Double.Parse(match.Groups[3].Value);
                        currentZ = Double.Parse(match.Groups[4].Value);
                    }
                    match = dockedRegex.Match(currentLine);
                    if (match.Success)
                    {
                        currentlyDocked = 1;
                    }
                    match = undockedRegex.Match(currentLine);
                    if (match.Success)
                    {
                        currentlyDocked = 0;
                    }
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.Write("Error reading file " + ex.ToString());
            }
        }

        return Tuple.Create(currentSystem, currentX, currentY, currentZ, currentlyDocked, fileLength);
    }

    public static void tailNetLog(MessageBus messageBus)
    {
        //string path, string logFile, long seekPos, Int32 elitePid
        Int32 lastElitePid = -1;     

        string lastSystem = null;
        int lastDockedStatus = -1; //Unknown State
        long seekPos = 0;

        string logFile = null;

        while (true)
        {
            Thread.Sleep(5000);
            Int32 checkPid = Elite.getPID();

            if (logFile == null || (checkPid != lastElitePid && checkPid >= 0))
            {
                lastElitePid = checkPid;
                logFile = getLatestNetlog();
                seekPos = 0;
                Debug.Write("Game state change detected.  Watching new netlog:  " + logFile);
            }

            if (logFile != null)
            {
                Tuple<string, double, double, double, int, long> tResponse = getDataFromNetlog(logFile, seekPos);
                if (tResponse.Item1 != null)
                {
                    messageBus.currentSystem = tResponse.Item1;
                    messageBus.currentX = tResponse.Item2;
                    messageBus.currentY = tResponse.Item3;
                    messageBus.currentZ = tResponse.Item4;
                }
                if (tResponse.Item5 != -1)
                {
                    messageBus.isDocked = tResponse.Item5;
                }
                long newseekPos = tResponse.Item6;
                
                if (newseekPos != seekPos)
                {
                    seekPos = newseekPos;
                }
              
                if (messageBus.currentSystem != null && messageBus.currentSystem != lastSystem)
                {
                    Debug.Write("Current System:  " + messageBus.currentSystem + "(" + messageBus.currentX.ToString() + "," + messageBus.currentY.ToString() + "," + messageBus.currentZ.ToString() + ")");
                    lastSystem = messageBus.currentSystem;
                }
                if (lastDockedStatus != messageBus.isDocked)
                {   
                    Debug.Write("Docked Status changed to:  " + messageBus.isDocked.ToString());
                    if (messageBus.isDocked == 1)
                    {
                        Thread.Sleep(7000);

                        if (messageBus.loggedinState == "ok")
                        {
                            Companion.getProfile(messageBus);
                            Announcements.eddnAnnouncement(messageBus);
                            Eddn.updateEddn(messageBus);
                        }
                    }
                    lastDockedStatus = messageBus.isDocked;
                }
            }
        }
    }

    private static Int32 getPID()
    {
        Process[] processByName = Process.GetProcessesByName("EliteDangerous64");
        if (processByName.Length != 0)
        {
            return processByName[0].Id;

        }
        processByName = Process.GetProcessesByName("EliteDangerous32");
        if (processByName.Length != 0)
        {
            return processByName[0].Id;
        }
        return -1;
    }

    public static bool isEliteRunning()
    {
        Int32 gamePid = getPID();
        if (gamePid > 0)
        {
            return true;
        }
        return false;
    }

    private static string getLatestNetlog()
    {
        string netlogFile = null;
        string netlogPath = null; 
        List<string> gamePaths = getGamePaths();
        foreach (string gamePath in gamePaths)
        {
            string logPath = Path.Combine(gamePath, "Logs");
            if (Directory.Exists(logPath))
            {
                Debug.Write("Candidate netlog directory: " + logPath);
                string newLogFileAndPath = getLogFile(logPath);
                string newLogFile = Path.GetFileName(newLogFileAndPath);
                if (netlogFile == null)
                {
                    netlogFile = newLogFile;
                    netlogPath = logPath;
                }
                else
                {
                    int cmp = String.Compare(newLogFile, netlogFile);
                    if (cmp > 0)
                    {
                        netlogFile = newLogFile;
                        netlogPath = logPath;
                    }
                }
            }
        }
        if (netlogFile == null)
        {
            Debug.Write("Error:  Unable to find Elite: Dangerous netlog file.");
        }
        return Path.Combine(netlogPath, netlogFile);
    }

    private static string getLogFile(string path)
    {
        try
        {
            string file = Directory.GetFiles(path, "netLog*.log").OrderByDescending(f => f).First();
            return file;
        }
        catch
        {
            return null;
        }
    }

    public static string getGameControlSchemesPath()
    {
        List<string> gamePaths = getGamePaths();
        foreach (string gamePath in gamePaths)
        {
            string controlSchemePath = Path.Combine(gamePath, "ControlSchemes");
            if (Directory.Exists(controlSchemePath))
            {
                // Why does Frontier install everything in so many locations?
                // Return the first hit.
                return controlSchemePath;
            }
        }
        Debug.Write("Error:  Unable to find Elite: Dangerous ControlSchemes path");
        return null;
    }

    public static List<string> getGamePaths()
    {
        // https://support.frontier.co.uk/kb/faq.php?id=108
        RegistryKey localMachine = Registry.LocalMachine;
        RegistryKey uninstallKey = null;
        List <string> locations = new List<string>();
        if (Environment.Is64BitOperatingSystem)
        {
            try
            {
                uninstallKey = localMachine.OpenSubKey(uninstallRegistryPath64bit);
            }
            catch { } 
        }
        else
        {
            try
            {
                uninstallKey = localMachine.OpenSubKey(uninstallRegistryPath32bit);
            }
            catch { }
        }

        string gamePath = "";
        string productsPath = "";
        // Look to where the game's uninstaller says it is located.
        if (uninstallKey != null)
        {
            foreach (string child in uninstallKey.GetSubKeyNames())
            {
                RegistryKey programKey = uninstallKey.OpenSubKey(child);
                try
                {
                    string publisher = programKey.GetValue("Publisher", string.Empty).ToString();
                    string displayName = programKey.GetValue("Display Name", string.Empty).ToString();
                    string uninstallString = programKey.GetValue("UninstallString", string.Empty).ToString();

                    if (publisher == "Frontier Developments")
                    {
                        string gameLocation = programKey.GetValue("InstallLocation").ToString();

                        productsPath = Path.Combine(gameLocation, "Products");
                        if (Directory.Exists(productsPath))
                        {
                            gamePath = Path.Combine(productsPath, "elite-dangerous-64");
                            if (Directory.Exists(gamePath))
                            {
                                locations.Add(gamePath);
                            }
                            gamePath = Path.Combine(productsPath, "FORC-FDEV-D-1010");
                            if (Directory.Exists(gamePath))
                            {
                                locations.Add(gamePath);
                            }
                        }
                    }
                }
                catch { } // No publisher - definitely not Frontier
            }
        }
        // Now add candidate locatiojns based on the ED launcher putting the game into AppData
        
        gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Frontier_Developments\Products\FORC-FDEV-D-1010");
        if (Directory.Exists(gamePath))
        {
            locations.Add(gamePath);
        }
        gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Frontier_Developments\Products\elite-dangerous-64");
        if (Directory.Exists(gamePath))
        {
            locations.Add(gamePath);
        }
        gamePath = @"C:\Program Files (x86)\Oculus\Software\frontier-developments-plc-elite-dangerous\Products\elite-dangerous-64";
        if (Directory.Exists(gamePath))
        {
            locations.Add(gamePath);
        }
        gamePath = @"C:\Program Files (x86)\Oculus\Software\frontier-developments-plc-elite-dangerous\Products\FORC-FDEV-D-1010";
        if (Directory.Exists(gamePath))
        {
            locations.Add(gamePath);
        }

        return locations;
    }

    public static string combatRankToString(int rank)
    {
        string[] rankings = new string[] { "Harmless", "Mostly Harmless", "Novice",
            "Competent", "Expert", "Master", "Dangerous", "Deadly", "Elite" };
        return rankings[rank];
    }

    public static string tradeRankToString(int rank)
    {
        string[] rankings = new string[] { "Penniless", "Mostly Penniless", "Pedlar",
            "Dealer", "Merchant", "Broker", "Entrepreneur", "Tycoon", "Elite" };
        return rankings[rank];
    }

    public static string exploreRankToString(int rank)
    {
        string[] rankings = new string[] { "Aimless", "Mostly Aimless", "Scout",
            "Surveyor", "Trailblazer", "Pathfinder", "Ranger", "Pioneer", "Elite" };
        return rankings[rank];
    }

    public static string cqcRankToString(int rank)
    {
        string[] rankings = new string[] { "Helpless", "Mostly Helpless", "Amateur",
            "Semi Professional", "Professional", "Champion", "Hero", "Gladiator", "Elite" };
        return rankings[rank];
    }

    public static string federationRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Recruit", "Cadet",
            "Midshipman", "Petty Officer", "Chief Petty Officer", "Warrant Officer",
            "Ensign", "Lieutenant", "Lieutenant Commander", "Post Commander",
            "Post Captain", "Rear Admiral", "Vice Admiral", "Admiral" };
        return rankings[rank];
    }

    public static string empireRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Outsider", "Serf",
            "Master", "Squire", "Knight", "Lord",
            "Baron", "Viscount", "Count", "Earl",
            "Marquis", "Duke", "Prince", "King" };
        return rankings[rank];
    }

    public static string powerPlayRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Rating 1", "Rating 2",
            "Rating 3", "Rating 4", "Rating 5" };
        return rankings[rank];
    }
    
    public static string[] listOfShipsPhoneticNames()
    {
        // Ship names "phonetic"
        string[] ships = {"Adder", "Anaconda", "Asp Explorer", "Asp Scout", "Cobra Mark 3", "Cobra Mark 4",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Dropship",
                "Federal Assault Ship", "Federal Corvette", "Federal Gunship", "Fer de Lance", "Hauler",
                "Imperial Clipper", "Imperial Courier", "Imperial Cutter", "Imperial Eagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type 6", "Type 7", "Type 9",
                "Viper Mark 3", "Viper Mark 4", "Vulture" };
        return ships;
    }

    public static string[] listOfShipsLongNames()
    {
        // Ship names "official", also for ED Shipyard
        string[] ships = {"Adder", "Anaconda", "Asp Explorer", "Asp Scout", "Cobra MkIII", "Cobra MkIV",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Dropship",
                "Federal Assault Ship", "Federal Corvette", "Federal Gunship", "Fer-de-Lance", "Hauler",
                "Imperial Clipper", "Imperial Courier", "Imperial Cutter", "Imperial Eagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type-6 Transporter", "Type-7 Transporter", "Type-9 Heavy",
                "Viper MkIII", "ViperMkIV", "Vulture" };
        return ships;
    }

    public static string[] listofShipsFrontierNames()
    {
        // Ship names as refered to in the Frontier Companion API
        string[] ships = {"Adder", "Anaconda", "Asp", "Asp_Scout", "CobraMkIII", "CobraMkIV",
                "DiamondBackXL", "DiamondBack", "Eagle", "Federation_Dropship",
                "Federation_Dropship_MkII", "Federation_Corvette", "Federation_Gunship", "FerDeLance",
                "Hauler", "Empire_Trader", "Empire_Courier", "Cutter", "Empire_Eagle",
                "Independant_Trader", "Orca", "Python", "SideWinder", "Type6", "Type7", "Type9",
                "Viper", "Viper_MkIV", "Vulture" };
        return ships;
    }

    public static string[] listOfShipVariableNames()
    {
        // Ship names for use in voiceattack variables
        string[] ships = {"Adder", "Anaconda", "AspExplorer", "AspScout", "CobraMkIII", "CobraMkIV",
                "DiamondbackExplorer", "DiamondbackScout", "Eagle", "FederalDropship",
                "FederalAssaultShip", "FederalCorvette", "FederalGunship", "Fer-de-Lance", "Hauler",
                "ImperialClipper", "ImperialCourier", "ImperialCutter", "ImperialEagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type-6", "Type-7", "Type-9",
                "ViperMkIII", "ViperMkIV", "Vulture" };
        return ships;
    }

    public static string[] listOfCoriolisNames()
    {
        //Ship names for use with Coriolis
        string[] ships = { "Adder" , "Anaconda" , "Asp Explorer" , "Asp Scout" , "Cobra Mk III" , "Cobra Mk IV",
                "Diamondback Explorer", "Diamondback Scout", "Eagle",  "Federal Dropship",
                "Federal Assault Ship", "Federal Corvette", "Federal Gunship", "Fer-de-Lance", "Hauler",
                "Imperial Clipper", "Imperial Courier", "Imperial Cutter", "Imperial Eagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type-6 Transporter", "Type-7 Transporter", "Type-9 Heavy",
                "Viper", "Viper Mk IV", "Vulture" };
        return ships;
    }

    public static string frontierShipToVariable(string frontierShip)
    {
        int counter = 0;
        string[] variableShips = listOfShipVariableNames();
        foreach (string ship in listofShipsFrontierNames())
        {
            if (ship == frontierShip)
            {
                return variableShips[counter];
            }
            counter++;
        }
        return null;
    }

    public static string frontierShipToPhonetic(string frontierShip)
    {
        int counter = 0;
        string[] phoneticShips = listOfShipsPhoneticNames();
        foreach (string ship in listofShipsFrontierNames())
        {
            if (ship == frontierShip)
            {
                return phoneticShips[counter];
            }
            counter++;
        }
        return null;
    }

    public static string frontierShipToPretty(string frontierShip)
    {
        int counter = 0;
        string[] prettyShips = listOfShipsLongNames();
        foreach (string ship in listofShipsFrontierNames())
        {
            if (ship == frontierShip)
            {
                return prettyShips[counter];
            }
            counter++;
        }
        return null;
    }

    public static string frontierShipToCoriolis(string frontierShip)
    {
        int counter = 0;
        string[] prettyShips = listOfShipsLongNames();
        foreach (string ship in listofShipsFrontierNames())
        {
            if (ship == frontierShip)
            {
                return prettyShips[counter];
            }
            counter++;
        }
        return null;
    }
}