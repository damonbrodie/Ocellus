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

    private static Tuple<double, string, string> checkServerVersion()
    {
        Tuple<bool, string, CookieContainer, string> tResponse = Web.sendRequest(versionCheckURL);
        string htmlData = tResponse.Item4;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        try
        {
            var result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);
            
            double serverVer = double.Parse(result["version"]);
            string profileURL = result["profile"];
            string pluginURL = result["plugin"];

            return Tuple.Create<double, string, string>(serverVer, profileURL, pluginURL);
        }
        catch
        {
            Debug.Write("ERROR:  Unable to parse version check resposne");
        }
        return Tuple.Create<double, string, string>(-1.0, null, null);
    }

    public static bool needUpgrade(ref Dictionary<string, object> state)
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
        Tuple<double, string, string> tResponse = checkServerVersion();
        double serverVer = tResponse.Item1;
        double myVer = double.Parse(OcellusPlugin.OcellusPlugin.pluginVersion);
        if (myVer < serverVer)
        {
            return true;
        }
        return false;
    }

    public static bool downloadUpdate()
    {
        Tuple<double, string, string> tResponse = checkServerVersion();
        // XXX download the update file
        return true;
    }
}