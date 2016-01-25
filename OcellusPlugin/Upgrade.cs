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

    private static Tuple<double, string, string> checkServerVersion()
    {
        Tuple<Boolean, string, CookieContainer, string> tResponse = Web.sendRequest(versionCheckURL);
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

    public static Boolean needUpgrade()
    {
        Tuple<double, string, string> tResponse = checkServerVersion();
        double serverVer = tResponse.Item1;
        double myVer = double.Parse(pluginVersion);
        if (myVer < serverVer)
        {
            return true;
        }
        return false;
    }

    public static Boolean downloadUpdate()
    {
        Tuple<double, string, string> tResponse = checkServerVersion();
        // XXX download the update file
        return true;
    }
}