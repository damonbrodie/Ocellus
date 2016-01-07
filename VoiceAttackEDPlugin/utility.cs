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
    public static string numberToString(int number)
    {
        switch (number)
        {
            case 0:
                {
                    return "Zero";
                }
            case 1:
                {
                    return "One";
                }
            case 2:
                {
                    return "Two";
                }
            case 3:
                {
                    return "Three";
                }
            case 4:
                {
                    return "Four";
                }
            case 5:
                {
                    return "Five";
                }
            case 6:
                {
                    return "Six";
                }
            case 7:
                {
                    return "Seven";
                }
            case 8:
                {
                    return "Eight";
                }
            case 9:
                {
                    return "Nine";
                }
            case 10:
                {
                    return "Ten";
                }
            default:
                {
                    return null;
                }
        }
    }
}