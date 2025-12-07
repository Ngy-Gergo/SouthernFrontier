using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Troop Definition")]
public class TroopDefinition : ScriptableObject
{
    public string displayName = "Troop"; // Name shown in UI
    public TroopType type;               // Archer / Swordsman / etc.
    public Sprite icon;                  // UI icon

    [Header("Combat")]
    public int defense = 5; // Defense value for 1 unit

    [Header("Training Cost (for 1 troop)")]
    public List<ResourceAmount> trainCost = new List<ResourceAmount>(); // Cost per unit
}
