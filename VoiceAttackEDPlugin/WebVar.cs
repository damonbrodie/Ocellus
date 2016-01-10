using System;
using System.IO;


// **********************************
// *  Functions for Web Variables   *
// **********************************

class WebVar
{
    public static string getWebVarFilename(string path)
    {
    string webConfigFile = Path.Combine(path, "WebVariables.txt");
    if (! File.Exists(webConfigFile))
    {
        Boolean worked = createWebConfig(webConfigFile);
        if (!worked)
        {
            return null;
        }
    }
    return webConfigFile;
    }
    private static Boolean createWebConfig(string configfile)
    {
        if (! File.Exists(configfile))
        {
            string[] lines =
            {
                "# There are two methods provided to retrieve Voice Attack variables from the web:",
                "# 1. JSON=http://someurl.com/path/to/json",
                "# 2. VAEDwebVar-myVariable=http://someurl.com/page/that/returns/a/single/variable",
                "#",
                "# The JSON must return a dictionary of attribute/value pairs, and the variables must",
                "# have the prefix VAEDwebVar-",
                "#",
                "# example JSON: {\"webVar\":{\"VAEDwebVar-bookmark\": \"Robigo\",\"VAEDwebVar-quantity\": 50,\"VAEDwebVar-myBoolean\": true}}",
                "#",
                "# You may have multiple lines for JSON and/or individual variables.  They will be processed in order,",
                "# and duplicate variable names will be overwritten by the latest source of the variable.",
                "#",
                "# There are three variable types available: string, integer and boolean",
                "# -if the value is either true or false (case does not matter) then they will be interpreted as boolean",
                "# -if the value has only digits, then the value will be interpreted as integer",
                "# -all other variables will be interpreted as strings.",
                "#",
                "# All lines in this file that don't start with JSON or VAEDwebVar- will be ignored",
                "JSON=http://ocellus.io/json-example",
                "VAEDwebVar-powerPlayExpansionAvailable=true",
                "VAEDwebVar-quantityGold=100",
                "VAEDwebVar-communityGoalBookmark=Hip 8996"
            };
            File.WriteAllLines(configfile, lines);
            
        }
        return true;
    }
}