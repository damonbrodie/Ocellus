using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// *************************************************************
// *  Functions for displaying ship on EDShipyard.com website  *
// *************************************************************
class EDShipyard
{
    public static StringBuilder export(Ship.Components shipObj)
    {
        string shipType = Elite.frontierShipToPretty(shipObj.attributes.shiptype);
        StringBuilder EDShipyardExport = new StringBuilder();
        EDShipyardExport.AppendLine("[" + shipType + "]");
        List<Ship.Hardpoint> sortedHardpoints = shipObj.hardpoints.OrderByDescending(o => o.slotSize).ToList();
        foreach (Ship.Hardpoint hardpoint in sortedHardpoints)
        {
            string name = "";
            if (hardpoint.name != null)
            {
                name = hardpoint.name + " ";
            }
            EDShipyardExport.AppendLine(
                hardpoint.slotSize.Substring(0,1) + ": " +
                hardpoint.@class.ToString() +
                hardpoint.rating + "/" +
                hardpoint.mount.Substring(0,1) + " " +
                name +
                hardpoint.group
           );
        }
        List<Ship.Utility> sortedUtilities = shipObj.utility.OrderByDescending(o => o.slot).ToList();
        foreach (Ship.Utility utility in sortedUtilities)
        {
            string name = "";
            if (utility.name == null)
            {
                name = utility.group;
            }
            else
            {
                name = utility.name;
            }
            EDShipyardExport.AppendLine(
                "U: " +
                utility.@class.ToString() +
                utility.rating + " " +
                name
           );
        }
        EDShipyardExport.AppendLine("");
        EDShipyardExport.AppendLine("BH: 1I " + shipObj.standard.bulkheads);
        EDShipyardExport.AppendLine("RB: " + shipObj.standard.powerPlant.@class.ToString() + shipObj.standard.powerPlant.rating + " Power Plant");
        EDShipyardExport.AppendLine("TM: " + shipObj.standard.thrusters.@class.ToString() + shipObj.standard.thrusters.rating + " Thrusters");
        EDShipyardExport.AppendLine("FH: " + shipObj.standard.frameShiftDrive.@class.ToString() + shipObj.standard.frameShiftDrive.rating + " Frame Shift Drive");
        EDShipyardExport.AppendLine("EC: " + shipObj.standard.lifeSupport.@class.ToString() + shipObj.standard.lifeSupport.rating + " Life Support");
        EDShipyardExport.AppendLine("PC: " + shipObj.standard.powerDistributor.@class.ToString() + shipObj.standard.powerDistributor.rating + " Power Distributor");
        EDShipyardExport.AppendLine("SS: " + shipObj.standard.sensors.@class.ToString() + shipObj.standard.sensors.rating + " Sensors");
        EDShipyardExport.AppendLine("FS: " + shipObj.standard.fuelTank.@class.ToString() + shipObj.standard.fuelTank.rating + " Fuel Tank (Capacity: " + shipObj.standard.fuelTank.capacity.ToString() + ")");
        EDShipyardExport.AppendLine("");
        List<Ship.Internal> sortedInternals = shipObj.@internal.OrderByDescending(o => o.slotSize).ToList();
        foreach (Ship.Internal @internal in sortedInternals)
        {
            string name = "";
            if (@internal.name == null)
            {
                name = @internal.group;
                if (@internal.capacity != 0)
                {
                    name += " (Capacity: " + @internal.capacity.ToString() + ")";
                }
            }
            else
            {
                name = @internal.name;

            }
            EDShipyardExport.AppendLine(
                @internal.slotSize.ToString() + ": " +
                @internal.@class.ToString() +
                @internal.rating + " " +
                name

           );
        }
        return EDShipyardExport;
    }
}