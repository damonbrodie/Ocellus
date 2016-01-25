using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


// ***********************************
// *  Functions for File Variables   *
// ***********************************

class FileVar
{
    private static void removeFileVars(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, Boolean?> booleanValues)
    {

        string pattern = @"^VAEDfileVar-.*$";
        Regex regexFileVar = new Regex(pattern);
        if (state.ContainsKey("VAEDfileTextVariableNames"))
        {
            List<string> textVariables = (List<string>)state["VAEDfileTextVariableNames"];
            foreach (string textVar in textVariables)
            {
                Match matchFileVar = regexFileVar.Match(textVar);
                if (matchFileVar.Success)
                {
                    textValues[textVar] = null;
                }
            }
            state.Remove("VAEDfileTextVariableNames");
        }
        if (state.ContainsKey("VAEDfileIntVariableNames"))
        {
            List<string> intVariables = (List<string>)state["VAEDfileIntVariableNames"];
            foreach (string intVar in intVariables)
            {

                Match matchFileVar = regexFileVar.Match(intVar);
                if (matchFileVar.Success)
                {
                    intValues[intVar] = null;
                }
            }
            state.Remove("VAEDfileIntVariableNames");
        }
        if (state.ContainsKey("VAEDfileBooleanVariableNames"))
        {
            List<string> boolVariables = (List<string>)state["VAEDfileBooleanVariableNames"];
            foreach (string boolVar in boolVariables)
            {

                Match matchFileVar = regexFileVar.Match(boolVar);
                if (matchFileVar.Success)
                {
                    booleanValues[boolVar] = null;
                }
            }
            state.Remove("VAEDfileBooleanVariableNames");
        }
    }

    private static Boolean createFileConfig(string configfile)
    {
        if (!File.Exists(configfile))
        {
            string[] lines =
            {
                "# This file provides a method for external proceses to insert Voice Attack variables dynamically.",
                "# The variables must be prefixed with VAEDfileVar-",
                "# All other variable names will be ignored.  Lines that don't have variable names will also be ignored",
                "#",
                "# There are three variable types available: string, integer and boolean",
                "# -if the value is either true or false (case does not matter) then they will be interpreted as boolean",
                "# -if the value has only digits, then the value will be interpreted as integer",
                "# -all other variables will be interpreted as strings",
                "#",
                "VAEDfileVar-powerPlayControlAvailable=true",
                "VAEDfileVar-quantitySilver=75",
                "VAEDfileVar-communityGoalBookmark=Hel"
            };
            File.WriteAllLines(configfile, lines);

        }
        return true;
    }

    public static string getFileVarFilename()
    {
        string fileConfigFile = Path.Combine(Config.Path(), "FileVariables.txt");
        if (!File.Exists(fileConfigFile))
        {
            Boolean worked = createFileConfig(fileConfigFile);
            if (!worked)
            {
                return null;
            }
        }
        return fileConfigFile;
    }

    public static void readFileVars(ref Dictionary<string, object> state, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, Boolean?> booleanValues)
    {
        removeFileVars(ref state, ref textValues, ref intValues, ref booleanValues);
        string fileConfigFile = getFileVarFilename();

        string pattern = @"^(VAEDfileVar-[^=]*)\s*=\s*(.*)$";
        Regex regexFileVar = new Regex(pattern);


        string[] lines = File.ReadAllLines(fileConfigFile);

        List<string> textVariableNames = new List<string>();
        List<string> intVariableNames = new List<string>();
        List<string> boolVariableNames = new List<string>();

        int matchInt;

        foreach (string line in lines)
        {
            string trimStr = line.Replace("\n", "");
            trimStr = trimStr.Replace("\r", "");
            Match matchFileVar = regexFileVar.Match(trimStr);
            if (matchFileVar.Success)
            {
                string variableName = matchFileVar.Groups[1].Value.Trim();
                var variableValue = matchFileVar.Groups[2].Value.TrimStart();

                if (variableValue.ToLower() == "true")
                {
                    booleanValues[variableName] = true;
                    boolVariableNames.Add(variableName);
                }
                else if (variableValue.ToLower() == "false")
                {
                    booleanValues[variableName] = true;
                    boolVariableNames.Add(variableName);
                }
                else if (Int32.TryParse(variableValue, out matchInt))
                {
                    intValues[variableName] = matchInt;
                    intVariableNames.Add(variableName);
                }
                else
                {
                    textValues[variableName] = variableValue;
                    textVariableNames.Add(variableName);
                }
            }
        }
        if (textVariableNames.Count > 0)
        {
            state.Add("VAEDfileTextVariableNames", textVariableNames);
        }
        if (intVariableNames.Count > 0)
        {
            state.Add("VAEDfileIntVariableNames", intVariableNames);
        }
        if (boolVariableNames.Count > 0)
        {
            state.Add("VAEDfileBooleanVariableNames", boolVariableNames);
        }
    }
}