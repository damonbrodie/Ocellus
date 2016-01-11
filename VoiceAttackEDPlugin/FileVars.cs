using System;
using System.IO;


// **********************************
// *  Functions for Web Variables   *
// **********************************

class FileVar
{
    private static Boolean createWebConfig(string configfile)
    {
        if (!File.Exists(configfile))
        {
            string[] lines =
            {
                "# This file provides a method for external proceses to insert Voice Attack variables dynamically.",
                "# The variables must be prefixed with VAEDfileVar-",
                "# All other variable names will be ignored.  Lines that don't have variable names will also be ignored",
                "# ",
                "# 2. VAEDwebVar-myVariable=http://someurl.com/page/that/returns/a/single/variable",
                "#",
                "# There are three variable types available: string, integer and boolean",
                "# -if the value is either true or false (case does not matter) then they will be interpreted as boolean",
                "# -if the value has only digits, then the value will be interpreted as integer",
                "# -all other variables will be interpreted as strings.",
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
        string fileConfigFile = Path.Combine(Config.getConfigPath(), "FileVariables.txt");
        if (!File.Exists(fileConfigFile))
        {
            Boolean worked = createWebConfig(fileConfigFile);
            if (!worked)
            {
                return null;
            }
        }
        return fileConfigFile;
    }

}