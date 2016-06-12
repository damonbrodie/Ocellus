using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Web.Script.Serialization;


// **********************************************************************************
// *  Functions for determining distance to any populated sytem in Elite Dangerous  *
// **********************************************************************************
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
            File.Delete(atlasFile);
            ZipFile.ExtractToDirectory(zipFile, path);
            File.Delete(zipFile);
            return true;
        }
        else
        {
            Debug.Write("ERROR:  Unable to download Atlas Index from Ocellus.io");
        }
        if (!File.Exists(atlasFile))
        {
            return false;
        }
        return true;
    }

    public static void loadAtlasIndex(ref Dictionary<string, object> state)
    {
        string atlasIndexFile = Path.Combine(Config.Path(), "atlas_index.txt");
        if (!downloadAtlas() && !File.Exists(atlasIndexFile) )
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

    public static int calcDistanceFromHere(ref Dictionary<string, dynamic> atlas, Elite.MessageBus messageBus, string toSystem)
    {
        double fromX;
        double fromY;
        double fromZ;

        double toX;
        double toY;
        double toZ;
        if (messageBus.currentSystem == "" || messageBus.currentSystem == null)
        {
            Debug.Write("Error:  calcDistance - fromSystem not specified");
            return -1;
        }
        if (toSystem == "" || toSystem == null)
        {
            Debug.Write("Error:  calcDistance - toSystem not specified");
            return -1;
        }
        if (messageBus.currentSystem == toSystem)
        {
            return 0;
        }
        if (messageBus.currentX != -9999.99)
        {
            fromX = messageBus.currentX;
            fromY = messageBus.currentY;
            fromZ = messageBus.currentZ;
        }
        else if (atlas.ContainsKey(messageBus.currentSystem))
        {
            fromX = (double)atlas[messageBus.currentSystem]["x"];
            fromY = (double)atlas[messageBus.currentSystem]["y"];
            fromZ = (double)atlas[messageBus.currentSystem]["z"];
        }
        else
        {
            Debug.Write("Error:  Unable to determine your current coordinates in system '" + messageBus.currentSystem +"'");
            return -1;
        }
        if (atlas.ContainsKey(toSystem))
        {
            toX = (double)atlas[toSystem]["x"];
            toY = (double)atlas[toSystem]["y"];
            toZ = (double)atlas[toSystem]["z"];
        }
        else
        {
            Debug.Write("Error:  Dest system '" + toSystem + "' is not in the EDDN database");
            return -1;
        }
        int distance = (int)(Math.Sqrt(Math.Pow((fromX - toX), 2) + Math.Pow((fromY - toY), 2) + Math.Pow((fromZ - toZ), 2)) + .5);

        return distance;
    }
}