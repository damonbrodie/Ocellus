using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Web.Script.Serialization;


// ********************************************************************************
// *  Functions for downloading and updating the star/station index from Ocellus  *
// ********************************************************************************
class SystemIndex
{
    private const string IndexURL = "http://ocellus.io/data/ocellus_index.zip";

    public static bool downloadIndex()
    {
        string zipFile = Path.Combine(Config.Path(), "ocellus_index.zip");
        string indexFile = Path.Combine(Config.Path(), "ocellus_index.json");
        if (File.Exists(indexFile))
        {
            // Download the index once a week
            DateTime fileTime = File.GetLastWriteTime(indexFile);
            DateTime weekago = DateTime.Now.AddDays(-7);
            if (fileTime > weekago)
            {
                return true;
            }
        }
        if (File.Exists(zipFile))
        {
            File.Delete(zipFile);
        }
        Debug.Write("Downloading System Index file from ocellus.io");
        if (Web.downloadFile(IndexURL, zipFile))
        {
            if (File.Exists(zipFile))
            {
                if (File.Exists(indexFile))
                {
                    File.Delete(indexFile);
                }
                try
                {
                    ZipFile.ExtractToDirectory(zipFile, Config.Path());
                }
                catch
                {
                    Debug.Write("Error:  Unable to extract System Index file from - " + zipFile);
                    return false;
                }
                Debug.Write("Extracted star system index");
                File.Delete(zipFile);
                return true;
            }
            else
            {
                Debug.Write("Error:  Systems Grammar download failed - " + IndexURL);
                return false;
            }
        }
        else
        {
            Debug.Write("Error:  Unable to download Systems Index from Ocellus.io");
        }

        if (!File.Exists(indexFile))
        {
            return false;
        }
        return true;
    }

    public static void loadSystemIndex(ref Elite.MessageBus messageBus)
    {
        string systemIndexFile = Path.Combine(Config.Path(), "ocellus_index.json");
        if (!downloadIndex() && !File.Exists(systemIndexFile))
        {
            Debug.Write("Error getting the system index - returning");
            return;
        }
        try
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 50000000;
            Dictionary<string, dynamic> systemIndex = new Dictionary<string, dynamic>();
            string[] lines = File.ReadAllLines(systemIndexFile);
            systemIndex = serializer.Deserialize<Dictionary<string, dynamic>>(lines[0]);
            messageBus.systemIndex = systemIndex;
            messageBus.systemIndexLoaded = true;
            Debug.Write("Loaded " + systemIndex["systems"].Count.ToString() + " populated star systems from Index");
            
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }

    }
}

