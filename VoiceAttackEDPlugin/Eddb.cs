using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Web.Script.Serialization;


// *************************************************************
// *  Functions for displaying System/Station on EDDB website  *
// *************************************************************

class Eddb
{

    private const string indexURL = "http://ocellus.io/data/eddb_index.zip";

    private static Boolean getEddbIndex()
    {
        string path = Config.getConfigPath();
        string zipFile = Path.Combine(path, "eddb_index.zip");
        if (Web.downloadFile(indexURL, zipFile))
        {
            ZipFile.ExtractToDirectory(zipFile, path);
            File.Delete(zipFile);
        }
        return true;
    }

    public static void loadEddbIndex(ref Dictionary<string, object> state)
    {
        string eddbIndexFile = Path.Combine(Config.getConfigPath(), "eddb_index.txt");
        if (!File.Exists(eddbIndexFile) && ! getEddbIndex())
        {
            return;
        }
        try
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 50000000;
            Dictionary<string, dynamic> eddbIndex = new Dictionary<string, dynamic>();
            string[] lines = File.ReadAllLines(eddbIndexFile);
            eddbIndex = serializer.Deserialize<Dictionary<string, dynamic>>(lines[0]);

            if (state.ContainsKey("VAEDeddbIndex"))
            {
                state["VAEDeddbIndex"] = eddbIndex;
            }
            else
            {
                state.Add("VAEDeddbIndex", eddbIndex);
            }
        }
        catch (Exception ex)
        {
            Utility.writeDebug(ex.ToString());
        }   
    }
}

