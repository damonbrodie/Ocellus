using System;
using Microsoft.Win32;


// ***********************************************************
// *  Functions for accessing the plugin's registry entries  *
// ***********************************************************
class PluginRegistry
{
    private const string pluginRegistryPath = @"SOFTWARE\OcellusPlugin";

    public static bool setStringValue(string attribute, string value)
    { 
        try
        {
            RegistryKey VAEDregistryKey = Registry.CurrentUser.CreateSubKey(pluginRegistryPath);
            VAEDregistryKey.SetValue(attribute, value);
            VAEDregistryKey.Close();
        }
        catch
        {
            Debug.Write("ERROR:  Unable to write value to Windows Registry.");
            return false;
        }
        return true;
    }

    public static string getStringValue(string attribute)
    {
        try
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\" + pluginRegistryPath, attribute, null).ToString();
        }
        catch
        {
            return null;
        }
    }

    public static void deleteKey(string key)
    {
        RegistryKey tempKey = Registry.CurrentUser.OpenSubKey(pluginRegistryPath, true);
        try
        {
            tempKey.DeleteValue(key);
        }
        catch (Exception ex)
        {
            Debug.Write("can't delete key: " + ex.ToString());
        }
    }

    public static int checkRegistry()
    {
        int check = 0;
        // Create the plugins registry key and an initial variable if it doesn't exist (typically first run)
        RegistryKey keyTest = Registry.CurrentUser.OpenSubKey(pluginRegistryPath);
        if (keyTest == null)
        {
            check = 1; // Registry is completely empty
            setStringValue("webVar1", "http://ocellus.io/webvars_test");
        }
        // These were added after the initial public release, so add them to the registry if they don't exist:

        if (getStringValue("updateText") == null)
        {
            if (check == 0)
            {
                check = 2; // Registry doesn't have notification entries
            }
            setStringValue("updateNotification", "tts");
            setStringValue("updateText", "An update is available on Ocellus dot i o");
            
        }
        if (getStringValue("eddnText") == null)
        {
            setStringValue("eddnNotification", "tts");
            setStringValue("eddnText", "Updating Data Network");
        }
        if (getStringValue("startupText") == null)
        {
            setStringValue("startupNotification", "tts");
            setStringValue("startupText", "Welcome back Commander!");
        }

        if (getStringValue("engineText") == null)
        {
            if (check == 0)
            {
                check = 3; //Registry doesn't have recognition engine entries
            }
            setStringValue("engineNotification", "sound");
            setStringValue("engineSound", @"c:\windows\media\Windows Balloon.wav");
        }
        return check;
    }
}