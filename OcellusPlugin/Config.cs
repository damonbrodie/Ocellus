using System;
using System.IO;


// ********************************************************
// *  Functions for handling the plugin config directory  *
// ********************************************************

class Config
{
    public static string Path()
    {
        string appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ocellus Plugin");
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

    public static string CookiePath()
    {
        return System.IO.Path.Combine(Config.Path(), "cookies.txt");
    }
}