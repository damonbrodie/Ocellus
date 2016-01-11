using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


// **********************************
// *  Functions for Web Variables   *
// **********************************

class WebVar
{
    private static Boolean createWebConfig(string configfile)
    {
        if (! File.Exists(configfile))
        {
            string[] lines =
            {
                "# Use the following JSON format to retrieve Voice Attack variables from the web:",
                "#    JSON=http://someurl.com/path/to/json",
                "#",
                "# The JSON must return a dictionary of attribute/value pairs, and the variables must",
                "# have the prefix VAEDwebVar-",
                "#",
                "# example JSON: {\"webVar\":{\"VAEDwebVar-bookmark\": \"Robigo\",\"VAEDwebVar-quantity\": 50,\"VAEDwebVar-communityGoalActive\": true}}",
                "#",
                "# You may have multiple JSON lines to retrieve variables from several locations.  They will be processed in order,",
                "# and duplicate variable names will be overwritten by the latest source of the variable.",
                "#",
                "# There are three variable types available: string, integer and boolean, and these will be placed in the corresponsing",
                "# type inside of Voice Attack.",
                "#",
                "JSON=http://ocellus.io/example/json"
            };
            File.WriteAllLines(configfile, lines);
            
        }
        return true;
    }

    public static string getWebVarFilename()
    {
        string webConfigFile = Path.Combine(Config.getConfigPath(), "WebVariables.txt");
        if (!File.Exists(webConfigFile))
        {
            Boolean worked = createWebConfig(webConfigFile);
            if (!worked)
            {
                return null;
            }
        }
        return webConfigFile;
    }

    public static string readWebVars()
    {
        string webConfigFile = getWebVarFilename();
        string pattern = @"^JSON\s*=\s*(.*)$";
        Regex regex = new Regex(pattern);

        string[] lines = File.ReadAllLines(webConfigFile);
        foreach (string line in lines)
        {
            
            Match match = regex.Match(line);
            if (match.Success)
            {
                string url = match.Groups[1].Value;
                Tuple<CookieContainer, string> tResponse = Web.sendRequest(url);
                string htmlData = tResponse.Item2;
                return htmlData;
            }
        }
        return null;
    }
}