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
    public const string pluginVersion = "0.1";

    public static Boolean needUpgrade()
    {
        Tuple<CookieContainer, string> tResponse = Web.sendRequest(versionCheckURL);
        string htmlData = tResponse.Item2;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        try
        {
            var result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);
            float myVer = float.Parse(pluginVersion);
            float serverVer = float.Parse(result["version"]);
            if (myVer < serverVer)
            {
                return true;
            }
        }
        catch
        {
            Utilities.writeDebug("Error:  Unable to parse version check resposne");
        }
        return false;
    }
}