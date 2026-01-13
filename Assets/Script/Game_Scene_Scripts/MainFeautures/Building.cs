using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingDefinition definition;   // Static data (name, options, costs, visuals)
    [SerializeField] private ResourceRecipesDB recipesDb;     // Optional input->output rules for crafted resources
    [SerializeField] private Transform visualRoot;            // Where the level model gets spawned

    [SerializeField, Min(0)] private int level = 0;           // Current building level
    [SerializeField] private int selectedOptionIndex = 0;     // Which production option is active

    [SerializeField] private string id;                       // Stable id for save/load
    [SerializeField] private bool productionEnabled = true; // Turn building on/off
    public bool ProductionEnabled => productionEnabled;
    public void SetProductionEnabled(bool enabled) => productionEnabled = enabled;

    private GameObject _currentVisual;                        // Spawned model for the current level

    public string Id => id;
    public int Level => level;
    public int SelectedOptionIndex => selectedOptionIndex;
    public BuildingDefinition Definition => definition;

    private void OnEnable()
    {
        // Produce once per turn.
        TurnManager.OnNextTurn += OnTurn;
        ApplyVisual();
    }

    private void OnDisable()
    {
        TurnManager.OnNextTurn -= OnTurn;
    }

    [ContextMenu("Generate Id")]
    private void GenerateId()
    {
        // One-time id for saving.
        id = Guid.NewGuid().ToString();
    }

    public string DisplayName => definition != null ? definition.displayName : gameObject.name;
    public string Description => definition != null ? definition.description : "";

    public int OptionCount => definition != null ? definition.productionOptions.Count : 0;

    public ProductionOption GetSelectedOption()
    {
        // Clamp to valid range and return the active option.
        if (definition == null || definition.productionOptions.Count == 0) return null;
        selectedOptionIndex = Mathf.Clamp(selectedOptionIndex, 0, definition.productionOptions.Count - 1);
        return definition.productionOptions[selectedOptionIndex];
    }

    public void SelectOption(int index)
    {
        // Called by UI when the player switches output type.
        if (definition == null || definition.productionOptions.Count == 0) return;
        selectedOptionIndex = Mathf.Clamp(index, 0, definition.productionOptions.Count - 1);
    }

    private void OnTurn()
    {
        // If the player turned this building off, it produces nothing and consumes nothing.
        if (!productionEnabled) return;

        // Add resources to the stockpile when a turn ends.
        var opt = GetSelectedOption();
        if (opt == null) return;

        int amount = opt.basePerTurn + (opt.perLevel * level);
        Stockpile.Instance?.Produce(opt.output, amount, recipesDb);
    }


    public bool CanLevelUp()
    {
        // Level-up costs are stored per step (level -> level+1).
        if (definition == null) return false;
        return level < definition.levelUpCosts.Count;
    }

    public bool TryLevelUp()
    {
        // Spend resources and upgrade if possible.
        if (!CanLevelUp()) return false;

        var cost = definition.levelUpCosts[level].cost;
        if (Stockpile.Instance == null) return false;

        if (!Stockpile.Instance.Spend(cost)) return false;

        level++;
        ApplyVisual();
        return true;
    }

    public void SetLevel(int newLevel)
    {
        // Used by save/load or debug.
        level = Mathf.Max(0, newLevel);
        ApplyVisual();
    }

    public void ApplySave(int savedLevel, int savedOptionIndex)
    {
        // Restore state from save.
        level = Mathf.Max(0, savedLevel);
        SelectOption(savedOptionIndex);
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        // Swap the level model.
        if (_currentVisual != null) Destroy(_currentVisual);

        if (definition == null || definition.levelVisualPrefabs == null || definition.levelVisualPrefabs.Count == 0)
            return;

        int idx = Mathf.Clamp(level, 0, definition.levelVisualPrefabs.Count - 1);
        var prefab = definition.levelVisualPrefabs[idx];
        if (prefab == null) return;

        var parent = visualRoot != null ? visualRoot : transform;
        _currentVisual = Instantiate(prefab, parent);

        // Keep the prefab aligned under the root.
        _currentVisual.transform.localPosition = Vector3.zero;
        _currentVisual.transform.localRotation = Quaternion.identity;
        _currentVisual.transform.localScale = Vector3.one;
    }
}
