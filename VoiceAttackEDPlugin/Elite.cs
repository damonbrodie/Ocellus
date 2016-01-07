using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;


// **********************************************
// *  Functions for working with ED game files  *
// **********************************************

class Elite
{
    private static string getLogFile(string path)
    {
        Utility.writeDebug("game log path " + path);
        try
        {
            string file = Directory.GetFiles(path, "netLog*.log").OrderByDescending(f => f).First();
            Utility.writeDebug("Current netLog is  " + file);
            return file;
        }
        catch
        {
            Utility.writeDebug("No NetLogs found");
            return string.Empty;
        }
    }

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
                Utility.writeDebug("Error reading file " + ex.ToString());

            }
        }
        return Tuple.Create(currentSystem, fileLength);
    }

    public static Tuple<Boolean, string, string, long, Int32> tailNetLog(string path, string logFile, long seekPos, Int32 elitePid)
    {

        Int32 checkPid = Elite.getPID(elitePid);

        string currentSystem = "";
        Utility.writeDebug("start");
        if (logFile == string.Empty || (checkPid != elitePid && checkPid >= 0))
        {
            // If we don't have a logFile already being tracked, or if Elite is restarted
            //   then go find the latest netLog file and start seatch that.
            logFile = getLogFile(path);
            seekPos = 0;  // Reset to start looking from the top of this new file.
            if (logFile == string.Empty)
            {
                return Tuple.Create(false, string.Empty, string.Empty, seekPos, checkPid);
            }
            Utility.writeDebug("found logFile " + logFile);
        }

        if (elitePid == checkPid)
        {
            // Elite running with same PID - safe to examine the same log file
            Tuple<string, long> tResponse = getSystemFromLog(path, logFile, seekPos);

            currentSystem = tResponse.Item1;
            seekPos = tResponse.Item2;
            Boolean flag = false;
            if (currentSystem != string.Empty)
            {
                flag = true;
            }

            Utility.writeDebug("Current System from netLog: " + currentSystem);
            return Tuple.Create(flag, currentSystem, logFile, seekPos, checkPid);
        }

        // Elite is not running, no point in reading logs
        seekPos = 0;
        elitePid = -1;
        return Tuple.Create(false, string.Empty, string.Empty, seekPos, elitePid);

    }

    public static Int32 getPID(Int32 checkPid = -1)
    {
        if (checkPid >= 0)
        {
            try
            {
                Process localById = Process.GetProcessById(checkPid);
                return checkPid;
            }
            catch //The process is not running, fall through to below
            { }
        }

        Process[] processByName = Process.GetProcessesByName("EliteDangerous64");
        if (processByName.Length == 0)
        {
            return -1; // Elite isn't running
        }
        else
        {
            checkPid = processByName[0].Id;
            return checkPid;
        }
    }

    public static int enableVerboseLogging (string path)
    {
        // Examine AppConfig.xml and see if VerboseLogging is already enabled.
        if (Directory.Exists(path))
        {
            string configFile = Path.Combine(path, "AppConfig.xml");
            string newConfig = Path.Combine(path, "AppConfig_Ocellus.xml");
            string backupFile = Path.Combine(path, "AppConfig_before_Ocellus.xml");

            string[] configLines = File.ReadAllLines(configFile);
            
            Boolean hasVerboseVar = false;

            foreach (string configLine in configLines)
            {
                if (configLine.Contains("VerboseLogging=\"0\""))
                {
                    hasVerboseVar = true;
                }
                else if (configLine.Contains("VerboseLogging=\"1\""))
                {
                    return 1;
                }
            }

            try
            {
                using (StreamWriter writeSR = new StreamWriter(newConfig))
                {
                    foreach (string currentLine in configLines)
                    {
                        Utility.writeDebug(currentLine);
                        if (currentLine.Contains("<Network") && hasVerboseVar == false)
                        {
                            // No verbose logging line, insert it right after
                            writeSR.WriteLine(currentLine);
                            writeSR.WriteLine("\t  VerboseLogging=\"1\"");

                        }
                        else if (currentLine.Contains("VerboseLogging"))
                        {
                            writeSR.WriteLine("\t  VerboseLogging=\"1\"");
                        }
                        else
                        {
                            writeSR.WriteLine(currentLine);
                        }
                    }
                }   
            }
            catch (Exception ex)
            {
                Utility.writeDebug("Error:  Unable to create temporary AppConfig.xml:  " + ex.ToString());
                return 0;
            }

            try
            {
                File.Replace(newConfig, configFile, backupFile);
                return 2;
            }
            catch (Exception ex)
            {
                Utility.writeDebug("Error:  Unable to replace AppConfig.xml:  " + ex.ToString());
                return 0;
            }
        }
        else
        {
            Utility.writeDebug("Error:  Can't access Elite game directory: " + path);
            return 0;
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

    public static string[] listOfShipsLongNames()
    {
        // Ship names "official" names
        string[] ships = {"Adder", "Anaconda", "Asp Explorer", "Asp Scout", "Cobra MkIII", "Cobra MkIV",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Assault Ship", "Federal Corvette",
                "Federal Gunship", "Fer-de-Lance", "Hauler", "Imperial Clipper", "Imperial Courier",
                "Imperial Cutter", "Imperial Eagle", "Keelback", "Orca", "Python", "Sidewinder MkI",
                "Type-6 Transporter", "Type-7 Transporter", "Type-9 Heavy", "Viper MkIII", "ViperMkIV" };
        return ships;
    }

    public static string[] listofShipsShortNames()
    {
        // Ship names as refered to in the API
        // XXX needs to be verified.
        string[] ships = {"Adder", "Anaconda", "Asp", "Asp_Scout", "Cobra", "Cobra MkIV",
                "Diamondback Explorer", "Diamondback Scout", "Eagle", "Federal Assault Ship", "Federal Corvette",
                "Federal Gunship", "Fer-de-Lance", "Hauler", "Imperial Clipper", "Imperial Courier",
                "Imperial Cutter", "Imperial Eagle", "Keelback", "Orca", "Python", "SideWinder",
                "Type6", "Type7", "Type9", "Viper", "ViperMkIV" };
        return ships;
    }
}