using System;


// **********************************************************************************
// *  Functions for determining distance to any populated sytem in Elite Dangerous  *
// **********************************************************************************
class Atlas
{
    public static int calcDistanceFromHere(Elite.MessageBus messageBus, string toSystem)
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
        if (messageBus.haveSystemCoords)
        {
            Debug.Write("Using coords from netlog");
            fromX = messageBus.currentX;
            fromY = messageBus.currentY;
            fromZ = messageBus.currentZ;
        }
        else if (messageBus.systemIndex["systems"].ContainsKey(messageBus.currentSystem))
        {
            Debug.Write("Using coords from Index");
            fromX = Convert.ToDouble(messageBus.systemIndex["systems"][messageBus.currentSystem]["x"]);
            fromY = Convert.ToDouble(messageBus.systemIndex["systems"][messageBus.currentSystem]["y"]);
            fromZ = Convert.ToDouble(messageBus.systemIndex["systems"][messageBus.currentSystem]["z"]);
        }
        else
        {
            Debug.Write("Error:  Unable to determine your current coordinates in system '" + messageBus.currentSystem +"'");
            return -1;
        }
        if (messageBus.systemIndex["systems"].ContainsKey(toSystem))
        {
            toX = Convert.ToDouble(messageBus.systemIndex["systems"][toSystem]["x"]);
            toY = Convert.ToDouble(messageBus.systemIndex["systems"][toSystem]["y"]);
            toZ = Convert.ToDouble(messageBus.systemIndex["systems"][toSystem]["z"]);
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