// **************************************************
// *  ID to Text mapping for Elite Dangerous Ranks  *
// **************************************************

class RankMapping
{
    public static string combatRankToString(int rank)
    {
        string[] rankings = new string[] { "Harmless", "Mostly Harmless", "Novice",
            "Competent", "Expert", "Master", "Dangerous", "Deadly", "Elite" };

        return rankings[rank];
    }

    public static string tradeRankToString(int rank)
    {
        string[] rankings = new string[] { "Penniless", "Mostly Penniless", "Pedlar",
            "Dealer", "Merchant", "Broker", "Entrepreneur", "Tycoon", "Elite" };

        return rankings[rank];
    }

    public static string exploreRankToString(int rank)
    {
        string[] rankings = new string[] { "Aimless", "Mostly Aimless", "Scout",
            "Surveyor", "Trailblazer", "Pathfinder", "Ranger", "Pioneer", "Elite" };

        return rankings[rank];
    }

    public static string cqcRankToString(int rank)
    {
        string[] rankings = new string[] { "Helpless", "Mostly Helpless", "Amateur",
            "Semi Professional", "Professional", "Champion", "Hero", "Gladiator", "Elite" };

        return rankings[rank];
    }

    public static string federationRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Recruit", "Cadet",
            "Midshipman", "Petty Officer", "Chief Petty Officer", "Warrant Officer",
            "Ensign", "Lieutenant", "Lieutenant Commander", "Post Commander",
            "Post Captain", "Rear Admiral", "Vice Admiral", "Admiral" };

        return rankings[rank];
    }

    public static string empireRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Outsider", "Serf",
            "Master", "Squire", "Knight", "Lord",
            "Baron", "Viscount", "Count", "Earl",
            "Marquis", "Duke", "Prince", "King" };

        return rankings[rank];
    }

    public static string powerPlayRankToString(int rank)
    {
        string[] rankings = new string[] { "None", "Rating 1", "Rating 2",
            "Rating 3", "Rating 4", "Rating 5" };

        return rankings[rank];
    }
}