using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Web.Script.Serialization;


class Atlas
{
    private const string atlasURL = "http://ocellus.io/data/atlas_index.zip";

    public static bool downloadAtlas()
    {
        string path = Config.Path();
        string zipFile = Path.Combine(path, "atlas_index.zip");
        string atlasFile = Path.Combine(Config.Path(), "atlas_index.txt");

        if (File.Exists(atlasFile))
        {
            // Download the index once a week
            DateTime fileTime = File.GetLastWriteTime(atlasFile);
            DateTime weekago = DateTime.Now.AddDays(-7);
            if (fileTime > weekago)
            {
                return true;
            }
        }


        if (Web.downloadFile(atlasURL, zipFile))
        {
            ZipFile.ExtractToDirectory(zipFile, path);
            File.Delete(zipFile);
        }
        else
        {
            Debug.Write("ERROR:  Unable to download Atlas Index from Ocellus.io");
        }
        return true;
    }

    public static void loadAtlasIndex(ref Dictionary<string, object> state)
    {
        string atlasIndexFile = Path.Combine(Config.Path(), "atlas_index.txt");
        if (!File.Exists(atlasIndexFile) && !downloadAtlas())
        {
            Debug.Write("Error getting the atlas index - returning");
            return;
        }
        try
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 50000000;
            Dictionary<string, dynamic> atlasIndex = new Dictionary<string, dynamic>();
            string[] lines = File.ReadAllLines(atlasIndexFile);
            atlasIndex = serializer.Deserialize<Dictionary<string, dynamic>>(lines[0]);

            if (state.ContainsKey("VAEDatlasIndex"))
            {
                state["VAEDatlasIndex"] = atlasIndex;
            }
            else
            {
                state.Add("VAEDatlasIndex", atlasIndex);
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }

    }

    public static double calcDistance(ref Dictionary<string, dynamic> atlas, string fromSystem, string toSystem)
    {
        double fromX = (double)atlas[fromSystem]["x"];
        double fromY = (double)atlas[fromSystem]["y"];
        double fromZ = (double)atlas[fromSystem]["z"];
        double toX = (double)atlas[toSystem]["x"];
        double toY = (double)atlas[toSystem]["y"];
        double toZ = (double)atlas[toSystem]["z"];

        double distance = Math.Sqrt(Math.Pow((fromX + toX), 2) + Math.Pow((fromY + toY), 2) + Math.Pow((fromZ + toZ), 2));

        return distance;
    }
}