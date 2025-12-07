using System.Collections.Generic;
using UnityEngine;

public class TroopTrainingBuilding : MonoBehaviour
{
    [SerializeField] private TroopTrainingBuildingDefinition definition; // What this building can train + UI info

    public TroopTrainingBuildingDefinition Definition => definition;

    public bool CanTrain(TroopDefinition troop, int amount)
    {
        // Basic checks first.
        if (troop == null || amount <= 0) return false;
        if (definition == null || definition.trainableTroops == null) return false;
        if (Stockpile.Instance == null || TroopBank.Instance == null) return false;

        // Only allow troops listed on this building.
        if (!definition.trainableTroops.Contains(troop)) return false;

        // Can we pay the total cost?
        return Stockpile.Instance.CanAfford(MultiplyCost(troop.trainCost, amount));
    }

    public bool TryTrain(TroopDefinition troop, int amount)
    {
        // Train and spend resources in one action.
        if (!CanTrain(troop, amount)) return false;

        var totalCost = MultiplyCost(troop.trainCost, amount);
        if (!Stockpile.Instance.Spend(totalCost)) return false;

        TroopBank.Instance.Add(troop.type, amount);
        return true;
    }

    private List<ResourceAmount> MultiplyCost(List<ResourceAmount> baseCost, int amount)
    {
        // Scale per-unit cost by the requested amount.
        var list = new List<ResourceAmount>();
        if (baseCost == null) return list;

        for (int i = 0; i < baseCost.Count; i++)
        {
            list.Add(new ResourceAmount
            {
                type = baseCost[i].type,
                amount = baseCost[i].amount * amount
            });
        }

        return list;
    }
}
