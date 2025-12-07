using System;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour
{
    public static Stockpile Instance { get; private set; }

    [Header("Starting resources")]
    [SerializeField] private List<ResourceAmount> starting = new List<ResourceAmount>(); // Starting amounts

    public event Action OnChanged; // Fired whenever totals change

    private readonly Dictionary<ResourceType, int> _totals = new Dictionary<ResourceType, int>(); // Resource totals

    private void Awake()
    {
        // Singleton setup.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Initialize every resource to 0.
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            _totals[t] = 0;

        // Apply inspector starting values.
        for (int i = 0; i < starting.Count; i++)
            _totals[starting[i].type] = Mathf.Max(0, starting[i].amount);

        OnChanged?.Invoke();
    }

    public int Get(ResourceType type) => _totals.TryGetValue(type, out var value) ? value : 0; // Safe read

    public void Add(ResourceType type, int amount)
    {
        // Add and notify UI.
        if (amount == 0) return;
        _totals[type] = Get(type) + amount;
        OnChanged?.Invoke();
    }

    public bool CanAfford(List<ResourceAmount> cost)
    {
        // True if we have enough for every item in the list.
        if (cost == null) return true;

        for (int i = 0; i < cost.Count; i++)
            if (Get(cost[i].type) < cost[i].amount)
                return false;

        return true;
    }

    public bool Spend(List<ResourceAmount> cost)
    {
        // Subtract cost if possible.
        if (!CanAfford(cost)) return false;

        for (int i = 0; i < cost.Count; i++)
            _totals[cost[i].type] = Get(cost[i].type) - cost[i].amount;

        OnChanged?.Invoke();
        return true;
    }

    public int Produce(ResourceType output, int desiredAmount, ResourceRecipesDB recipesDb)
    {
        // Produce up to desiredAmount, limited by recipe inputs.
        if (desiredAmount <= 0) return 0;

        // No recipe => free output.
        if (recipesDb == null || !recipesDb.TryGet(output, out var recipe) || recipe.inputs == null || recipe.inputs.Count == 0)
        {
            Add(output, desiredAmount);
            return desiredAmount;
        }

        int producible = desiredAmount;

        // Compute how many we can afford by each input.
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            var inp = recipe.inputs[i];
            if (inp.amount <= 0) continue;

            int possibleByThisInput = Get(inp.type) / inp.amount;
            producible = Mathf.Min(producible, possibleByThisInput);
            if (producible <= 0) break;
        }

        if (producible <= 0) return 0;

        // Spend inputs.
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            var inp = recipe.inputs[i];
            if (inp.amount <= 0) continue;

            _totals[inp.type] = Get(inp.type) - (inp.amount * producible);
        }

        // Add output.
        _totals[output] = Get(output) + producible;

        OnChanged?.Invoke();
        return producible;
    }

    public List<ResourceAmount> ExportAll()
    {
        // Dump current totals for saving.
        var list = new List<ResourceAmount>();
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            list.Add(new ResourceAmount { type = t, amount = Get(t) });

        return list;
    }

    public void ImportAll(List<ResourceAmount> list)
    {
        // Reset everything then apply imported values.
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            _totals[t] = 0;

        if (list != null)
            for (int i = 0; i < list.Count; i++)
                _totals[list[i].type] = list[i].amount;

        OnChanged?.Invoke();
    }
}
