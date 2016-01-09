using Microsoft.Win32;


// ***********************************************************
// *  Functions for accessing the plugin's registry entries  *
// ***********************************************************

class PluginRegistry
{
    private const string pluginRegistryPath = @"SOFTWARE\VoiceAttackEliteDangerousPlugin";

    public static bool setStringValue(string attribute, string value)
    { 
        try
        {
            RegistryKey VAEDregistryKey = Registry.CurrentUser.OpenSubKey(pluginRegistryPath, true);
            VAEDregistryKey.SetValue(attribute, value);
            VAEDregistryKey.Close();
        }
        catch
        {
            Utility.writeDebug("ERROR:  Unable to write value to Windows Registry.");
            return false;
        }
        return true;
    }

    public static string getStringValue(string attribute)
    {
        try
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\" + pluginRegistryPath, attribute, string.Empty).ToString();
        }
        catch
        {
            Utility.writeDebug("Error:  Unable to read from Windows Registry.");
            return null;
        }
    }
}

