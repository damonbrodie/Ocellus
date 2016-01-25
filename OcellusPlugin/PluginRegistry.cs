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
            // Create the plugins registry key and an initial variable if it doesn't exist (typically first run)
            RegistryKey keyTest = Registry.CurrentUser.OpenSubKey(pluginRegistryPath);
            if (keyTest == null)
            {
                setStringValue("webVar1", "http://ocellus/webvars/communitygoals.json");
            }
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
}