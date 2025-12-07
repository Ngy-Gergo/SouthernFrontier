using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private BuildingDefinition definition;
    [SerializeField] private ResourceRecipesDB recipesDb;
    [SerializeField] private Transform visualRoot;

    [SerializeField, Min(0)] private int level = 0;
    [SerializeField] private int selectedOptionIndex = 0;

    [SerializeField] private string id; // for save

    private GameObject _currentVisual;

    public string Id => id;
    public int Level => level;
    public int SelectedOptionIndex => selectedOptionIndex;
    public BuildingDefinition Definition => definition;

    private void OnEnable()
    {
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
        id = Guid.NewGuid().ToString();
    }

    public string DisplayName => definition != null ? definition.displayName : gameObject.name;
    public string Description => definition != null ? definition.description : "";

    public int OptionCount => definition != null ? definition.productionOptions.Count : 0;

    public ProductionOption GetSelectedOption()
    {
        if (definition == null || definition.productionOptions.Count == 0) return null;
        selectedOptionIndex = Mathf.Clamp(selectedOptionIndex, 0, definition.productionOptions.Count - 1);
        return definition.productionOptions[selectedOptionIndex];
    }

    public void SelectOption(int index)
    {
        if (definition == null || definition.productionOptions.Count == 0) return;
        selectedOptionIndex = Mathf.Clamp(index, 0, definition.productionOptions.Count - 1);
    }

    private void OnTurn()
    {
        var opt = GetSelectedOption();
        if (opt == null) return;

        int amount = opt.basePerTurn + (opt.perLevel * level);
        Stockpile.Instance?.Produce(opt.output, amount, recipesDb);
    }

    public bool CanLevelUp()
    {
        if (definition == null) return false;
        return level < definition.levelUpCosts.Count;
    }

    public bool TryLevelUp()
    {
        if (!CanLevelUp()) return false;

        var cost = definition.levelUpCosts[level].cost; // cost to go level -> level+1
        if (Stockpile.Instance == null) return false;

        if (!Stockpile.Instance.Spend(cost)) return false;

        level++;
        ApplyVisual();
        return true;
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(0, newLevel);
        ApplyVisual();
    }

    public void ApplySave(int savedLevel, int savedOptionIndex)
    {
        level = Mathf.Max(0, savedLevel);
        SelectOption(savedOptionIndex);
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (_currentVisual != null) Destroy(_currentVisual);

        if (definition == null || definition.levelVisualPrefabs == null || definition.levelVisualPrefabs.Count == 0)
            return;

        int idx = Mathf.Clamp(level, 0, definition.levelVisualPrefabs.Count - 1);
        var prefab = definition.levelVisualPrefabs[idx];
        if (prefab == null) return;

        var parent = visualRoot != null ? visualRoot : transform;
        _currentVisual = Instantiate(prefab, parent);
        _currentVisual.transform.localPosition = Vector3.zero;
        _currentVisual.transform.localRotation = Quaternion.identity;
        _currentVisual.transform.localScale = Vector3.one;
    }
}