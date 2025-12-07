using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TowerLevel
{
    public int defense = 20; // Defense value at this level
    public List<ResourceAmount> upgradeCost = new List<ResourceAmount>(); // Cost to reach this level
    public GameObject visualPrefab; // Model for this level (optional)
}

[CreateAssetMenu(menuName = "Game/Tower Definition")]
public class TowerDefinition : ScriptableObject
{
    public List<TowerLevel> levels = new List<TowerLevel>(); // levels[0] = level 0, levels[1] = level 1, ...
}
