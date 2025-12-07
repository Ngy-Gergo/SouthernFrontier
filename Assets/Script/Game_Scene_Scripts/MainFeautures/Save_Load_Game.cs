using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class BuildingSave
{
    public string id;              // Matches Building.Id
    public int level;              // Saved building level
    public int selectedOptionIndex; // Saved production option
}

[Serializable]
public class SaveData
{
    public int turn;                    // Current turn
    public List<ResourceAmount> resources; // Stockpile snapshot
    public List<BuildingSave> buildings;   // Per-building state
}

public class Save_Load_Game : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager; // Optional scene reference

    private string PathFile => Path.Combine(Application.persistentDataPath, "save.json"); // Save file path

    public void Save()
    {
        // Build save payload.
        var data = new SaveData
        {
            turn = turnManager != null ? turnManager.turn : 1,
            resources = Stockpile.Instance != null ? Stockpile.Instance.ExportAll() : new List<ResourceAmount>(),
            buildings = new List<BuildingSave>()
        };

        // Save every building that has an id.
        foreach (var b in FindObjectsOfType<Building>())
        {
            if (string.IsNullOrWhiteSpace(b.Id)) continue;

            data.buildings.Add(new BuildingSave
            {
                id = b.Id,
                level = b.Level,
                selectedOptionIndex = b.SelectedOptionIndex
            });
        }

        File.WriteAllText(PathFile, JsonUtility.ToJson(data, true));
        Debug.Log($"Saved: {PathFile}");
    }

    public void Load()
    {
        if (!File.Exists(PathFile))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        // NOTE: LoadScene is async-ish. Applying data immediately may run before objects exist.
        SceneManager.LoadScene("MainGameScene");

        var json = File.ReadAllText(PathFile);
        var data = JsonUtility.FromJson<SaveData>(json);

        if (turnManager != null) turnManager.turn = data.turn;
        if (Stockpile.Instance != null) Stockpile.Instance.ImportAll(data.resources);

        // Map buildings by id for quick lookup.
        var map = new Dictionary<string, Building>();
        foreach (var b in FindObjectsOfType<Building>())
            if (!string.IsNullOrWhiteSpace(b.Id))
                map[b.Id] = b;

        // Apply building states.
        if (data.buildings != null)
        {
            for (int i = 0; i < data.buildings.Count; i++)
            {
                var s = data.buildings[i];
                if (s != null && map.TryGetValue(s.id, out var b))
                    b.ApplySave(s.level, s.selectedOptionIndex);
            }
        }

        Debug.Log($"Loaded: {PathFile}");
    }
}
