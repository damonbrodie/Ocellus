using System;
using Microsoft.Win32;


// ************************************************
// * Functions for reading Game registry entries  *
// ************************************************

class EliteRegistry
{
    const string uninstallRegistryPath64bit = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

    public static Tuple <string, int> getGameDetails ()
    {
        RegistryKey localMachine = Registry.LocalMachine;
        RegistryKey uninstallKey = null;
        if (Environment.Is64BitOperatingSystem)
        {
            uninstallKey = localMachine.OpenSubKey(uninstallRegistryPath64bit);
        }
        else
        {
            // XXX need the location for 32 bit Windows
        }
        foreach (string child in uninstallKey.GetSubKeyNames())
        {
            RegistryKey programKey = uninstallKey.OpenSubKey(child);
            try
            {
                string publisher = programKey.GetValue("Publisher", string.Empty).ToString();
                string displayName = programKey.GetValue("Display Name", string.Empty).ToString();
                string uninstallString = programKey.GetValue("UninstallString", string.Empty).ToString();

                if (publisher == "Frontier Developments")
                {
                    string gameLocation = programKey.GetValue("InstallLocation").ToString();
                    int isSteam = 0;
                    if (uninstallString.Contains("steam.exe"))
                    {
                        isSteam = 1;
                    }

                    PluginRegistry.setStringValue("GamePath", gameLocation);
                    PluginRegistry.setIntegerValue("isSteamGame", isSteam);
                    return Tuple.Create(gameLocation, isSteam);
                }
            }
            catch {} // No publisher - definietely not Frontier
        }
        return Tuple.Create(string.Empty, -1);
    }

}