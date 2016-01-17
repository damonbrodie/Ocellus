using System;
using System.Collections.Generic;
using System.Collections.Specialized;


// **********************************************************
// *  Functions for displaying ship on Coriolis.io website  *
// **********************************************************

class Coriolis
{
    public static OrderedDictionary createCoriolisJson(ref Dictionary<string, object> state)
    {
        try
        {
            if (!state.ContainsKey("VAEDcompanionDict"))
            {
                return null;
            }
            Dictionary<string, dynamic> companion = (Dictionary<string, dynamic>) state["VAEDcompanionDict"];
            OrderedDictionary coriolisDict = new OrderedDictionary();
            coriolisDict.Add("$schema", @"http://json-schema.org/draft-04/schema#");
            int shipId = companion["commander"]["currentShipId"];
            string currentShipId = shipId.ToString();
            string currentShip = companion["ships"][currentShipId]["name"];
            coriolisDict.Add("name", currentShip);
            coriolisDict.Add("title", "Ship Loadout");

            foreach (string key in coriolisDict.Keys)
            {
                Utility.writeDebug("key " + key + " | " + coriolisDict[key]);
            }
        }
        catch (Exception ex)
        {
            Utility.writeDebug(ex.ToString());
        }
        return null;
    }
    
}