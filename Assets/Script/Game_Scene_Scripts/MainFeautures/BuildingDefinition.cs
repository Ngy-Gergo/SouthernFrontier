using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductionOption
{
    public ResourceType output;   // What resource this option produces
    public int basePerTurn = 5;   // Produced at level 0
    public int perLevel = 1;      // Extra per building level
}

[Serializable]
public class LevelUpCost
{
    // Cost for going from level N -> N+1 (index = N).
    public List<ResourceAmount> cost = new List<ResourceAmount>();
}

[CreateAssetMenu(menuName = "Game/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    public string displayName = "Building"; // Shown in UI
    [TextArea] public string description;   // Shown in UI

    public List<ProductionOption> productionOptions = new List<ProductionOption>(); // What it can produce

    public List<GameObject> levelVisualPrefabs = new List<GameObject>(); // Model per level (optional)

    public List<LevelUpCost> levelUpCosts = new List<LevelUpCost>(); // Upgrade costs per step
}
