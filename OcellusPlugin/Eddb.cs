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

    private static bool downloadIndex()
    {
        string path = Config.Path();
        string eddbIndexFile = Path.Combine(Config.Path(), "eddb_index.txt");
        string zipFile = Path.Combine(path, "eddb_index.zip");

        if (File.Exists(eddbIndexFile))
        {
            // Download the index once a week
            DateTime fileTime = File.GetLastWriteTime(eddbIndexFile);
            DateTime weekago = DateTime.Now.AddDays(-7);
            if (fileTime > weekago)
            {
                return true;
            }
        }

        if (Web.downloadFile(indexURL, zipFile))
        {
            ZipFile.ExtractToDirectory(zipFile, path);
            File.Delete(zipFile);
        }
        else
        {
            Debug.Write("ERROR:  Unable to download EDDB Index from Ocellus.io");
        }
        return true;
    }

    public static void loadEddbIndex(ref Dictionary<string, object> state)
    {
        string eddbIndexFile = Path.Combine(Config.Path(), "eddb_index.txt");
        if (!File.Exists(eddbIndexFile) && ! downloadIndex())
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
            Debug.Write(ex.ToString());
        }   
    }
}

