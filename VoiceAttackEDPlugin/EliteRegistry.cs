using System;
using Microsoft.Win32;


// ************************************************
// * Functions for reading Game registry entries  *
// ************************************************

class EliteRegistry
{
    const string uninstallRegistryPath64bit = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

    public static string getInstallPath ()
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
            Utility.writeDebug("KEY " + child);

            RegistryKey programKey = uninstallKey.OpenSubKey(child);
            try
            {
                string publisher = programKey.GetValue("Publisher").ToString();
                Utility.writeDebug("Publisher " + publisher);

                if (publisher == "Frontier Developments")
                {

                    string gameLocation = programKey.GetValue("InstallLocation").ToString();
                    PluginRegistry.setValue("GamePath", gameLocation);
                    return gameLocation;
                }
            }
            catch { } // No publisher - definietely not Frontier
        }
        return null;
    }

}