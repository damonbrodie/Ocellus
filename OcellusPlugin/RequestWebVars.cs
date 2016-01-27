using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


// ****************************************
// *  Methods for processing the WebVars  *
// ****************************************

class RequestWebVars
{
    private static void removeWebVars(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, bool?> booleanValues)
    {
        string pattern = @"^VAEDwebVar-.*$";
        Regex regexWebVar = new Regex(pattern);
        if (state.ContainsKey("VAEDwebTextVariableNames"))
        {
            List<string> textVariables = (List<string>)state["VAEDwebTextVariableNames"];
            foreach (string textVar in textVariables)
            {
                Match matchWebVar = regexWebVar.Match(textVar);
                if (matchWebVar.Success)
                {
                    textValues[textVar] = null;
                }
            }
            state.Remove("VAEDwebTextVariableNames");
        }
        if (state.ContainsKey("VAEDwebIntVariableNames"))
        {
            List<string> intVariables = (List<string>)state["VAEDwebIntVariableNames"];
            foreach (string intVar in intVariables)
            {

                Match matchWebVar = regexWebVar.Match(intVar);
                if (matchWebVar.Success)
                {
                    intValues[intVar] = null;
                }
            }
            state.Remove("VAEDwebIntVariableNames");
        }
        if (state.ContainsKey("VAEDwebBooleanVariableNames"))
        {
            List<string> boolVariables = (List<string>)state["VAEDwebBooleanVariableNames"];
            foreach (string boolVar in boolVariables)
            {

                Match matchWebVar = regexWebVar.Match(boolVar);
                if (matchWebVar.Success)
                {
                    booleanValues[boolVar] = null;
                }
            }
            state.Remove("VAEDwebBooleanVariableNames");
        }
    }

    private static List<string> getWebVarHosts()
    {
        List<string> jsonHosts = new List<string>();
        int counter = 1;
        while (counter <= 5)
        {
            string currentHost = PluginRegistry.getStringValue("webVar" + counter.ToString());
            if (currentHost != null)
            {
                jsonHosts.Add(currentHost);
            }
            counter++;
        }
        return jsonHosts;
    }

    public static Tuple<bool, string, Dictionary<string,string>, Dictionary<string, int>, Dictionary<string, bool>, string> requestWebVars(string url)
    {
        bool error = false;
        string errorMessage = "";
        string htmlData = null;
        Dictionary<string, string> stringVars = new Dictionary<string, string>();
        Dictionary<string, int> intVars = new Dictionary<string, int>();
        Dictionary<string, bool> boolVars = new Dictionary<string, bool>();

        Tuple<bool, string, CookieContainer, string> tResponse = Web.sendRequest(url);
        if (tResponse.Item1 == false)
        {
            htmlData = tResponse.Item4;

            Debug.Write("htmlData: " + htmlData);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string pattern = @"^VAEDwebVar-.*$";
            Regex regexWebVar = new Regex(pattern);

            int maxVariables = 50;  //Limit in case the JSON provider goes crazy.

            try
            {
                var result = serializer.Deserialize<Dictionary<string, dynamic>>(htmlData);

                if (result.ContainsKey("webVar"))
                {
                    foreach (string variableName in result["webVar"].Keys)
                    {
                        Match matchWebVar = regexWebVar.Match(variableName);
                        if (matchWebVar.Success && maxVariables > 0)
                        {
                            var variableValue = result["webVar"][variableName];
                            if (variableValue.GetType() == typeof(bool))
                            {
                                boolVars[variableName] = variableValue;
                                maxVariables--;
                            }
                            else if (variableValue.GetType() == typeof(int))
                            {
                                intVars[variableName] = variableValue;
                                maxVariables--;
                            }
                            else if (variableValue.GetType() == typeof(string))
                            {
                                stringVars[variableName] = variableValue;
                                maxVariables--;
                            }
                        }
                        else
                        {
                            errorMessage = "Improperly named variables found and ignored";
                            Debug.Write("Web Vars Error:  Variable does not have VAEDwebVar- prefix.  Ignoring this variable");
                        }
                    }
                    if (maxVariables == 0)
                    {
                        errorMessage = "Maximum variable limit reached (50)";
                        Debug.Write("Maximum variable limit (50) reached for this JSON source"); 
                    }
                }
                else
                {
                    error = true;
                    errorMessage = "Response missing top level \"webVar\" key";
                    Debug.Write("Web Vars Error:  Response does not contain top level key \"webVar\"");
                }
            }
            catch
            {
                error = true;
                errorMessage = "Unable to parse JSON";
                Debug.Write("ERROR:  unable to read JSON:  " + htmlData);
            }
        }
        else
        {
            error = true;
            errorMessage = "Error retrieving URL";
        }
        return Tuple.Create(error, errorMessage, stringVars, intVars, boolVars, htmlData);
    }

    public static void readWebVars(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, bool?> booleanValues)
    {
        int webVarCooldown = Utilities.isCoolingDown(ref state, "VAEDwebVarCooldown");
        if (webVarCooldown > 0)
        {
            Debug.Write("Web Variable request is cooling down: " + webVarCooldown.ToString() + " seconds remain.");
            return;
        }
        removeWebVars(ref state, ref textValues, ref intValues, ref booleanValues);
        List<string> jsonHosts = getWebVarHosts();

        List<string> textVariableNames = new List<string>();
        List<string> intVariableNames = new List<string>();
        List<string> boolVariableNames = new List<string>();

        foreach (string host in jsonHosts)
        {
            Tuple<bool, string, Dictionary<string, string>, Dictionary<string, int>, Dictionary<string, bool>, string> tResponse = requestWebVars(host);
            // XXX handle error return codes
            foreach (string key in tResponse.Item3.Keys)
            {
                textValues.Add(key, tResponse.Item3[key]);
                textVariableNames.Add(key);
            }
            foreach (string key in tResponse.Item4.Keys)
            {

                intValues.Add(key, tResponse.Item4[key]);
                intVariableNames.Add(key);

            }
            foreach (string key in tResponse.Item5.Keys)
            {

                booleanValues.Add(key, tResponse.Item5[key]);
                boolVariableNames.Add(key);
            }
        }

        if (textVariableNames.Count > 0)
        {
            state.Add("VAEDwebTextVariableNames", textVariableNames);
        }
        if (intVariableNames.Count > 0)
        {
            state.Add("VAEDwebIntVariableNames", intVariableNames);
        }
        if (boolVariableNames.Count > 0)
        {
            state.Add("VAEDwebBooleanVariableNames", boolVariableNames);
        }
    }
}