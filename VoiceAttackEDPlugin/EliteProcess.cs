using System;
using System.Diagnostics;


// *******************************************
// *  Process functions for Elite Dangerous  *
// *******************************************

class EliteProcess
{
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
            Utility.writeDebug("Checking PID - current value" + checkPid.ToString());
            return checkPid;
        }
    }
}