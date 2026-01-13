using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class BuildingSave
{
    public string id;               // Matches Building.Id
    public int level;               // Saved building level
    public int selectedOptionIndex; // Saved production option
    public bool productionEnabled;  // Saved on/off
}
[Serializable]
public class TowerSave
{
    public string id;
    public int level;
}

[Serializable]
public class SaveData
{
    public List<TowerSave> towers;
    public List<TroopAmount> troops; // TroopBank snapshot
    public int turn;                       // Current turn number
    public int waveIndex;                  // Current wave pointer
    public List<ResourceAmount> resources; // Stockpile snapshot
    public List<BuildingSave> buildings;   // Per-building state
}

public class Save_Load_Game : MonoBehaviour
{
    public static Save_Load_Game Instance { get; private set; }

    private SaveData _pendingLoad; // Stored until the scene is actually loaded

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        var go = new GameObject("Save_Load_Game");
        go.AddComponent<Save_Load_Game>();
        DontDestroyOnLoad(go);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this); // NOT Destroy(gameObject)
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public bool HasSave() => GamePaths.HasSave();

    public void Save()
    {
        Debug.Log("Hello");
        // Find what we need from the current scene.
        var turnManager = TurnManager.Instance;
        var waveManager = FindObjectOfType<EnemyWaveManager>();

        // Build save payload.
        var data = new SaveData
        {
            troops = TroopBank.Instance != null ? TroopBank.Instance.ExportAll() : new List<TroopAmount>(),
            turn = turnManager != null ? turnManager.turn : 1,
            waveIndex = waveManager != null ? waveManager.WaveIndex : 0,
            resources = Stockpile.Instance != null ? Stockpile.Instance.ExportAll() : new List<ResourceAmount>(),
            buildings = new List<BuildingSave>()
        };

        data.towers = new List<TowerSave>();

        var towers = FindObjectsByType<Tower>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < towers.Length; i++)
        {
            var t = towers[i];
            if (t == null || string.IsNullOrWhiteSpace(t.Id)) continue;

            data.towers.Add(new TowerSave
            {
                id = t.Id,
                level = t.Level
            });
        }

        // Save every building that has an id.
        foreach (var b in FindObjectsOfType<Building>())
        {
            if (string.IsNullOrWhiteSpace(b.Id)) continue;

            data.buildings.Add(new BuildingSave
            {
                id = b.Id,
                level = b.Level,
                selectedOptionIndex = b.SelectedOptionIndex,
                productionEnabled = b.ProductionEnabled
            });
        }

        File.WriteAllText(GamePaths.SavePath, JsonUtility.ToJson(data, true));
        Debug.Log($"Saved: {GamePaths.SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(GamePaths.SavePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        // Read first, apply after scene load.
        var json = File.ReadAllText(GamePaths.SavePath);
        _pendingLoad = JsonUtility.FromJson<SaveData>(json);

        SceneManager.LoadScene(GamePaths.MainGameScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only apply if we have pending data.
        if (_pendingLoad == null) return;

        if (TroopBank.Instance != null) TroopBank.Instance.ImportAll(_pendingLoad.troops);

        // Apply turn/resources.
        if (TurnManager.Instance != null) TurnManager.Instance.turn = _pendingLoad.turn;
        if (Stockpile.Instance != null) Stockpile.Instance.ImportAll(_pendingLoad.resources);

        // Apply wave index.
        var waveManager = FindObjectOfType<EnemyWaveManager>();
        if (waveManager != null) waveManager.ApplySave(_pendingLoad.waveIndex);

        // Map buildings by id for quick lookup.
        var map = new Dictionary<string, Building>();
        foreach (var b in FindObjectsOfType<Building>())
            if (!string.IsNullOrWhiteSpace(b.Id))
                map[b.Id] = b;

        // Apply building states.
        if (_pendingLoad.buildings != null)
        {
            for (int i = 0; i < _pendingLoad.buildings.Count; i++)
            {
                var s = _pendingLoad.buildings[i];
                if (s == null) continue;

                if (map.TryGetValue(s.id, out var b))
                {
                    b.ApplySave(s.level, s.selectedOptionIndex);
                    b.SetProductionEnabled(s.productionEnabled);
                }
            }
        }
        var towers = FindObjectsByType<Tower>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var towerMap = new Dictionary<string, Tower>();

        for (int i = 0; i < towers.Length; i++)
        {
            var t = towers[i];
            if (t == null || string.IsNullOrWhiteSpace(t.Id)) continue;
            towerMap[t.Id] = t;
        }

        if (_pendingLoad.towers != null)
        {
            for (int i = 0; i < _pendingLoad.towers.Count; i++)
            {
                var s = _pendingLoad.towers[i];
                if (s == null) continue;

                if (towerMap.TryGetValue(s.id, out var t))
                    t.SetLevel(s.level);
            }
        }

        Debug.Log($"Loaded: {GamePaths.SavePath}");

        // Clear so it doesn't re-apply on future scene loads.
        _pendingLoad = null;
    }
}
