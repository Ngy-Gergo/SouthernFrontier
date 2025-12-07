using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductionOption
{
    public ResourceType output;
    public int basePerTurn = 5;
    public int perLevel = 1;
}

[Serializable]
public class LevelUpCost
{
    // Cost to go from level N -> N+1 (index = N)
    public List<ResourceAmount> cost = new();
}

[CreateAssetMenu(menuName = "Game/Building Definition")]
public class BuildingDefinition : ScriptableObject
{
    public string displayName = "Building";
    [TextArea] public string description;

    public List<ProductionOption> productionOptions = new();

    // Optional visuals per level (index = level, clamped)
    public List<GameObject> levelVisualPrefabs = new();

    // Cost list: costs[0] = level 0->1, costs[1] = 1->2, ...
    public List<LevelUpCost> levelUpCosts = new();
}
