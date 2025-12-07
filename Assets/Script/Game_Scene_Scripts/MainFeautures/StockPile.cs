using System;
using System.Collections.Generic;
using UnityEngine;
public class Stockpile : MonoBehaviour
{
    public static Stockpile Instance { get; private set; }

    public event Action OnChanged;

    private readonly Dictionary<ResourceType, int> _totals = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            _totals[t] = 0;
    }

    public int Get(ResourceType type) => _totals.TryGetValue(type, out var value) ? value : 0;

    public void Add(ResourceType type, int amount)
    {
        if (amount == 0) return;
        _totals[type] = Get(type) + amount;
        OnChanged?.Invoke();
    }

    public bool CanAfford(List<ResourceAmount> cost)
    {
        if (cost == null) return true;
        for (int i = 0; i < cost.Count; i++)
            if (Get(cost[i].type) < cost[i].amount) return false;
        return true;
    }

    public bool Spend(List<ResourceAmount> cost)
    {
        if (!CanAfford(cost)) return false;
        for (int i = 0; i < cost.Count; i++)
            _totals[cost[i].type] = Get(cost[i].type) - cost[i].amount;

        OnChanged?.Invoke();
        return true;
    }

    // Produces up to desiredAmount; if recipe inputs are missing, produces less.
    public int Produce(ResourceType output, int desiredAmount, ResourceRecipesDB recipesDb)
    {
        if (desiredAmount <= 0) return 0;

        // No recipe => free production
        if (recipesDb == null || !recipesDb.TryGet(output, out var recipe) || recipe.inputs == null || recipe.inputs.Count == 0)
        {
            Add(output, desiredAmount);
            return desiredAmount;
        }

        int producible = desiredAmount;

        // Find max producible based on each input
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            var inp = recipe.inputs[i];
            if (inp.amount <= 0) continue;

            int possibleByThisInput = Get(inp.type) / inp.amount;
            producible = Mathf.Min(producible, possibleByThisInput);
            if (producible <= 0) break;
        }

        if (producible <= 0) return 0;

        // Consume inputs
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            var inp = recipe.inputs[i];
            if (inp.amount <= 0) continue;
            _totals[inp.type] = Get(inp.type) - (inp.amount * producible);
        }

        // Add output
        _totals[output] = Get(output) + producible;

        OnChanged?.Invoke();
        return producible;
    }

    // Save helpers
    public List<ResourceAmount> ExportAll()
    {
        var list = new List<ResourceAmount>();
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            list.Add(new ResourceAmount { type = t, amount = Get(t) });
        return list;
    }

    public void ImportAll(List<ResourceAmount> list)
    {
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            _totals[t] = 0;

        if (list != null)
            for (int i = 0; i < list.Count; i++)
                _totals[list[i].type] = list[i].amount;

        OnChanged?.Invoke();
    }
}