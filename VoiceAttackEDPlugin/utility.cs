using System;
using System.IO;


// ***********************
// *  Utility functions  *
// ***********************

class Utility
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
}