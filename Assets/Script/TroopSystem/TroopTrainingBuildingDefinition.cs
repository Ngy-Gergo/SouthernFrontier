using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Troop Training Building Definition")]
public class TroopTrainingBuildingDefinition : ScriptableObject
{
    public string displayName = "Barracks"; // Shown in the building UI
    public Sprite icon;                     // Building icon in UI

    public List<TroopDefinition> trainableTroops = new List<TroopDefinition>(); // Troops this building can train
}
