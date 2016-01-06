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
        Utility.writeDebug("Checking PID - last value" + checkPid.ToString());
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
            Utility.writeDebug("Elite stopped");
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
                Utility.writeDebug("Error:  Unable to modify AppConfig.xml:  " + ex.ToString());
                return 0;
            }

            File.Replace(newConfig, configFile, backupFile);
            return 2;
        }
        else
        {
            Utility.writeDebug("Error:  Can't access Elite game directory: " + path);
            return 0;
        }
    }
}