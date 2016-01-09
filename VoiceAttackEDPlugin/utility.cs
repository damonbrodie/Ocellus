using System;
using System.IO;


// ***********************
// *  Utility functions  *
// ***********************

class Utility
{
    public static void writeDebug(string line)
    {
        string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Voice Attack ED Plugin");
        string file = Path.Combine(appPath, "debug.log");
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