using System;
using UnityEngine;

public enum ResourceType
{
    Gold, Wood, Stone, Iron, Wheat, Bread, Beer, Weapons, Villagers
}

[Serializable]
public struct ResourceAmount
{
    public ResourceType type; // Which resource
    public int amount;        // How much
}
