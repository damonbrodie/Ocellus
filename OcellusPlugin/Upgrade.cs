using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;


// ****************************************************
// *  Functions for upgrading the plugin and profile  *
// ****************************************************
class Upgrade
{
    private const string versionCheckURL = "http://ocellus.io/version";

    private static Tuple<string, string, string> checkServerVersion()
    {
        Debug.Write("Checking server for Ocellus plugin updates.");
        Tuple<bool, string, CookieContainer, string> tResponse = Web.sendRequest(versionCheckURL);
        string htmlData = tResponse.Item4;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        try
        {
            var result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);
            
            string serverVer = result["version"];
            string profileURL = result["profile"];
            string pluginURL = result["plugin"];

            return Tuple.Create<string, string, string>(serverVer, profileURL, pluginURL);
        }
        catch
        {
            Debug.Write("ERROR:  Unable to parse version check response");
        }
        return Tuple.Create<string, string, string>("0.1", null, null);
    }

    public static bool needUpgrade()
    {
        Tuple<string, string, string> tResponse = checkServerVersion();
        string serverVer = tResponse.Item1;
        

        var serverVersion = new Version(serverVer);
        var localVersion = new Version(OcellusPlugin.OcellusPlugin.pluginVersion);

        var result = serverVersion.CompareTo(localVersion);
        if (result > 0)
        {
            Debug.Write("Ocellus Update Available at ocellus.io");
            return true;
        }
        Debug.Write("Ocellus is up-to-date");
        return false;
    }

    public static bool needUpgradeWithCooldown(ref Dictionary<string, object> state)
    {
        int upgradeCooldown = Utilities.isCoolingDown(ref state, "VAEDupgradeCooldown", 3600);
        if (upgradeCooldown > 0)
        {
            Debug.Write("Ocellus version check is cooling down: " + upgradeCooldown.ToString() + " seconds remain.");
            if (state.ContainsKey("VAEDupgradeAvailable"))
            {
                return (bool)state["VAEDupgradeAvailable"];
            }
            return false;
            
        }
        return needUpgrade();
    }
}