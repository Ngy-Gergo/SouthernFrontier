using System;
using UnityEngine;
public class Tower : MonoBehaviour
{
    // Simple global list so we don't FindObjectsOfType every wave.
    public static readonly System.Collections.Generic.List<Tower> All = new System.Collections.Generic.List<Tower>();
    public static event System.Action OnAnyTowerChanged;

    [SerializeField] private TowerDefinition def;  // Tower levels, costs, visuals
    [SerializeField] private Transform visualRoot; // Where the model spawns
    [SerializeField] private int level = 0;        // Current tower level
    [SerializeField] private string id; // Stable id for save/load
    public string Id => id;
    private GameObject _visual; // Current spawned model

    public int Level => level;

    private void OnEnable()
    {
        // Register for fast lookup.
        if (!All.Contains(this)) All.Add(this);
        OnAnyTowerChanged?.Invoke();
    }

    private void OnDisable()
    {
        // Unregister.
        All.Remove(this);
        OnAnyTowerChanged?.Invoke();
    }
    private void OnValidate()
    {
        // Generate once in editor so it persists in scene/prefab.
        if (!Application.isPlaying && string.IsNullOrWhiteSpace(id))
            id = Guid.NewGuid().ToString();
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(0, newLevel);
        ApplyVisual();
    }

    private void Start() => ApplyVisual(); // Spawn the starting model

    public int GetDefense()
    {
        // Defense comes from the current level entry.
        if (def == null || def.levels == null || def.levels.Count == 0) return 0;

        int idx = Mathf.Clamp(level, 0, def.levels.Count - 1);
        return def.levels[idx].defense;
    }

    public bool CanUpgrade()
    {
        // True if there's a next level.
        if (def == null || def.levels == null) return false;
        return level + 1 < def.levels.Count;
    }

    public bool TryUpgrade()
    {
        // Spend resources and move to the next level.
        if (!CanUpgrade() || Stockpile.Instance == null) return false;

        int next = level + 1;
        var cost = def.levels[next].upgradeCost;

        if (!Stockpile.Instance.Spend(cost)) return false;
        
        level = next;
        ApplyVisual();
        OnAnyTowerChanged?.Invoke();
        return true;
    }

    private void ApplyVisual()
    {
        // Swap the model to match the level.
        if (_visual != null) Destroy(_visual);

        if (def == null || def.levels == null || def.levels.Count == 0) return;

        int idx = Mathf.Clamp(level, 0, def.levels.Count - 1);
        var prefab = def.levels[idx].visualPrefab;
        if (prefab == null) return;

        Transform parent = visualRoot != null ? visualRoot : transform;
        _visual = Instantiate(prefab, parent);

        // Keep it aligned under the root.
        _visual.transform.localPosition = Vector3.zero;
        _visual.transform.localRotation = Quaternion.identity;
        _visual.transform.localScale = Vector3.one;
    }
}
