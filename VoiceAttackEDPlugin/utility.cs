using System;
using System.IO;
using System.Collections.Generic;


// ***********************
// *  Utility functions  *
// ***********************

class Utilities
{
    public static void writeDebug(string line)
    {
        
        string file = Path.Combine(Config.getConfigPath(), "debug.log");
        using (StreamWriter writer = File.AppendText(file))
        {
            try
            {
                writer.Write("{0} {1} | ", DateTime.Now.ToShortDateString(), DateTime.Now.ToString("HH:mm"));
                writer.WriteLine(line);
                writer.Close();
            }
            catch
            { }
        }
    }

    public static int isCoolingDown(ref Dictionary<string, object> state, string cooldownName, int minutes = 1)
    {
        if (! state.ContainsKey(cooldownName))
        {
            state.Add(cooldownName, DateTime.Now);
            return 0;
        }

        int minutesAgo = -1;
        if (minutes > 1)
        {
            minutesAgo = minutes * -1;
        }
        
        DateTime lastRun = (DateTime)state[cooldownName];
        DateTime compareTime = DateTime.Now.AddMinutes(minutesAgo);

        double diffSeconds = (lastRun - compareTime).TotalSeconds;

        if (diffSeconds > 0)
        {
            return (int) diffSeconds;
        }
        state[cooldownName] = DateTime.Now;
        return 0;
            
    }
}