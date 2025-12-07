using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TroopStat
{
    public TroopType type; // Which troop this entry belongs to
    public int defense = 5; // Defense for 1 unit
    public List<ResourceAmount> trainCost = new List<ResourceAmount>(); // Cost for 1 unit
}

[CreateAssetMenu(menuName = "Game/Troop Stats DB")]
public class TroopStatsDB : ScriptableObject
{
    public List<TroopStat> stats = new List<TroopStat>(); // All troop entries

    public TroopStat Get(TroopType type)
    {
        // Simple lookup by troop type.
        for (int i = 0; i < stats.Count; i++)
            if (stats[i].type == type) return stats[i];

        return null;
    }
}
