using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;


// **********************************************
// *  Functions for working with ED game files  *
// **********************************************

class Elite
{
    const string uninstallRegistryPath64bit = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
    const string uninstallRegistryPath32bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";


    private static Tuple<string, long> getSystemFromLog(string path, string logFile, long seekPos)
    {
        string fullPath = Path.Combine(path, logFile);

        long fileLength = 0;

        string currentSystem = "";

        string pattern = @"^{\d\d:\d\d:\d\d} System:\d+\(([^)]+)\)";
        Regex regex = new Regex(pattern);

        if (File.Exists(fullPath))
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
                    Match match = regex.Match(currentLine);
                    if (match.Success)
                    {
                        currentSystem = match.Groups[1].Value;
                    }
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.Write("Error reading file " + ex.ToString());
            }
        }
        return Tuple.Create(currentSystem, fileLength);
    }

    public static string getGameLogPath(string gamePath)
    {
        // XXX make this work for Season 1, 64 and 32 bit.
        if (gamePath != null)
        {
            return Path.Combine(gamePath, "Products", "elite-dangerous-64", "Logs");
        }
        return null;
    }

    public static string getGamePath()
    {
        string gamePath = PluginRegistry.getStringValue("gamePath");
        if (gamePath != string.Empty)
        {
            return gamePath;
        }
        else
        {
            RegistryKey localMachine = Registry.LocalMachine;
            RegistryKey uninstallKey = null;
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
                        string startString = "";

                        if (publisher == "Frontier Developments")
                        {
                            string gameLocation = programKey.GetValue("InstallLocation").ToString();
                            PluginRegistry.setStringValue("gamePath", gameLocation);

                            int endPos = uninstallString.IndexOf("steam.exe");
                            if (endPos > 0)
                            {
                                endPos += 10;
                                startString = uninstallString.Substring(0, endPos);
                                PluginRegistry.setStringValue("startPath", startString);
                                if (uninstallString.Contains("359320"))
                                {
                                    PluginRegistry.setStringValue("startParams", "steam://rungameid/359320");
                                }
                                else if (uninstallString.Contains("419270"))
                                {
                                    PluginRegistry.setStringValue("startParams", "steam://rungameid/419270");
                                }
                            }
                            else
                            {
                                // XXX what if it isn't steam?
                            }

                            return gameLocation;
                        }
                    }
                    catch { } // No publisher - definietely not Frontier
                }
            } //else no installer in registry?

            return string.Empty;
        }
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
        // Ship names "official" names
        string[] ships = {"Adder", "Anaconda", "Asp Explorer", "Asp Scout", "Cobra Mark 3", "Cobra Mark 4",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Dropship",
                "Federal Assault Ship", "Federal Corvette", "Federal Gunship", "Fer de Lance", "Hauler",
                "Imperial Clipper", "Imperial Courier", "Cutter", "Imperial Eagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type 6", "Type 7", "Type 9",
                "Viper Mark 3", "Viper Mark 4", "Vulture" };
        return ships;
    }

    public static string[] listOfShipsLongNames()
    {
        // Ship names "official" names
        string[] ships = {"Adder", "Anaconda", "Asp Explorer", "Asp Scout", "Cobra MkIII", "Cobra MkIV",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Dropship",
                "Federal Assault Ship", "Federal Corvette", "Federal Gunship", "Fer-de-Lance", "Hauler",
                "Imperial Clipper", "Imperial Courier", "Cutter", "Imperial Eagle", "Keelback", "Orca",
                "Python", "Sidewinder MkI", "Type-6 Transporter", "Type-7 Transporter", "Type-9 Heavy",
                "Viper MkIII", "ViperMkIV", "Vulture" };
        return ships;
    }

    public static string[] listofShipsShortNames()
    {
        // Ship names as refered to in the API
        // XXX needs to be verified.
        string[] ships = {"Adder", "Anaconda", "Asp", "Asp_Scout", "CobraMkIII", "CobraMkIV",
                "DiamondBackXL", "DiamondBack", "Eagle", "Federation_Dropship",
                "Federation_Dropship_MkII", "Federation_Corvette", "Federation_Gunship", "FerDeLance",
                "Hauler", "Empire_Trader", "Empire_Courier", "Imperial Cutter", "Empire_Eagle",
                "Independant_Trader", "Orca", "Python", "SideWinder", "Type6", "Type7", "Type9",
                "Viper", "Viper_MkIV", "Vulture" };
        return ships;
    }

    public static string[] listOfShipVariableNames()
    {
        // Ship names for use in variables
        string[] ships = {"Adder", "Anaconda", "AspExplorer", "AspScout", "CobraMkIII", "CobraMkIV",
                "DiamondbackExplorer", "DiamondbackScout", "Eagle", "FederalDropship",
                "FederalAssaultShip", "FederalCorvette", "FederalGunship", "Fer-de-Lance", "Hauler",
                "ImperialClipper", "ImperialCourier", "Cutter", "ImperialEagle", "Keelback", "Orca",
                "Python", "Sidewinder", "Type-6", "Type-7", "Type-9",
                "ViperMkIII", "ViperMkIV", "Vulture" };
        return ships;
    }

    public static string frontierShipToVariable(string frontierShip)
    {
        int counter = 0;
        string[] variableShips = listOfShipVariableNames();
        foreach (string ship in listofShipsShortNames())
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
        foreach (string ship in listofShipsShortNames())
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
        foreach (string ship in listofShipsShortNames())
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