using System;
using System.IO;
using System.Collections.Generic;


// *********************************
// *  Utility and Debug functions  *
// *********************************

class Debug
{
    public static string Path()
    {
        string debugPath = System.IO.Path.Combine(Config.Path(), "debug.log");
        if (!File.Exists(debugPath))
        {
            var handle = File.Create(debugPath);
            handle.Close();
        }
        return debugPath;
    }

    public static void Write(string line)
    {
        string file = Debug.Path();
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

    public static void Clear()
    {
        string file = Debug.Path();
        if (File.Exists(file))
        {
            try
            {
                File.Delete(file);
            }
            catch { }
        }
    }
}

class Utilities
{
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