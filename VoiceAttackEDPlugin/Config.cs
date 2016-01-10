using System;
using System.IO;


// ********************************************************
// *  Functions for handling the plugin config directory  *
// ********************************************************

class Config
{
    public static string getConfigPath()
    {
        string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Voice Attack ED Plugin");
        try
        {
            Directory.CreateDirectory(appPath);
        }
        catch
        {
            return null;
        }
        return appPath;
    }

    public static string getCookiePath()
    {
        return Path.Combine(getConfigPath(), "cookies.txt");
    }
}

