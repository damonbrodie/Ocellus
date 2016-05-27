using System;
using System.Collections.Generic;
using System.IO;


// *******************************************
// *  Functions for tracking recent systems  *
// *******************************************
class TrackSystems
{

    public static void Add(ref Dictionary<string, object> state, String system)
    {
        List<String> trackedSystems = (List<String>)state["VAEDtrackedSystems"];
        trackedSystems.Capacity = 200;
        if (trackedSystems.Count == 0 || trackedSystems[0] != system)
        {
            trackedSystems.Insert(0, system);
            state["VAEDtrackedSystems"] = trackedSystems;
            string recentSystemsFile = Path.Combine(Config.Path(), "RecentSystems.txt");
            File.WriteAllLines(recentSystemsFile, trackedSystems);
        }
    }
    public static void Load(ref Dictionary<string, object> state)
    {
        string recentSystemsFile = Path.Combine(Config.Path(), "RecentSystems.txt");
        List<string> trackedSystems = new List<string>();
        if (File.Exists(recentSystemsFile))
        {
            string[] lines = File.ReadAllLines(recentSystemsFile);
            trackedSystems = new List<String>(lines);
        }
        state["VAEDtrackedSystems"] = trackedSystems;

    }
}

