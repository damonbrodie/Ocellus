using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


// **********************************
// *  Functions for Web Variables   *
// **********************************

class WebVar
{
    private static void removeWebVars(ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, Boolean?> booleanValues)
    {
        string pattern = @"^VAEDwebVar-.*$";
        Regex regexWebVar = new Regex(pattern);
        foreach (string textVar in textValues.Keys)
        {
            Utility.writeDebug("textVar" + textVar);
            Match matchWebVar = regexWebVar.Match(textVar);
            if (matchWebVar.Success)
            {
                textValues[textVar] = null;
            }
        }
        foreach (string intVar in intValues.Keys)
        {
            Match matchWebVar = regexWebVar.Match(intVar);
            if (matchWebVar.Success)
            {
                intValues[intVar] = null;
            }
        }
        foreach (string boolVar in booleanValues.Keys)
        {
            Match matchWebVar = regexWebVar.Match(boolVar);
            if (matchWebVar.Success)
            {
                booleanValues[boolVar] = null;
            }
        }
    }
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

    public static string readWebVars(ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, Boolean?> booleanValues)
    {
        try
        {

            // XXX this doesn't work yet - requires feature request in VA
            removeWebVars(ref textValues, ref intValues, ref booleanValues);
            string webConfigFile = getWebVarFilename();
            string pattern = @"^JSON\s*=\s*(.*)$";
            Regex regexJsonVar = new Regex(pattern);

            pattern = @"^VAEDwebVar-.*$";
            Regex regexWebVar = new Regex(pattern);

            string[] lines = File.ReadAllLines(webConfigFile);
            foreach (string line in lines)
            {

                Match matchJsonVar = regexJsonVar.Match(line);
                if (matchJsonVar.Success)
                {
                    string url = matchJsonVar.Groups[1].Value;
                    Tuple<CookieContainer, string> tResponse = Web.sendRequest(url);
                    string htmlData = tResponse.Item2;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    var result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);

                    if (result.ContainsKey("webVar"))
                    {
                        foreach (string variableName in result["webVar"].Keys)
                        {
                            Match matchWebVar = regexWebVar.Match(variableName);
                            if (matchWebVar.Success)
                            {
                                var variableValue = result["webVar"][variableName];
                                if (variableValue.GetType() == typeof(Boolean))
                                {
                                    booleanValues[variableName] = variableValue;
                                }
                                else if (variableValue.GetType() == typeof(int))
                                {
                                    intValues[variableName] = variableValue;
                                }
                                else if (variableValue.GetType() == typeof(string))
                                {
                                    textValues[variableName] = variableValue;
                                }
                            }
                            else
                            {
                                Utility.writeDebug("Web Vars Error:  Variable does not have VAEDwebVar- prefix.  Ignoring this variable");
                            }
                            
                        }
                    }
                    else
                    {
                        Utility.writeDebug("Web Vars Error:  Response does not contain top level key \"webVar\"");
                    }
                    return htmlData;
                }
            }
            
        }
        catch (Exception ex)
        {
            Utility.writeDebug("error:  " + ex.ToString());
        }
        return null;
    }
}