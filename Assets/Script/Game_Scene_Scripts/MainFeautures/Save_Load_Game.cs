using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class BuildingSave
{
    public string id;
    public int level;
    public int selectedOptionIndex;
}

[Serializable]
public class SaveData
{
    public int turn;
    public List<ResourceAmount> resources;
    public List<BuildingSave> buildings;
}

public class Save_Load_Game : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    private string PathFile => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public void Save()
    {
        var data = new SaveData
        {
            turn = turnManager != null ? turnManager.turn : 1,
            resources = Stockpile.Instance != null ? Stockpile.Instance.ExportAll() : new List<ResourceAmount>(),
            buildings = new List<BuildingSave>()
        };

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
        SceneManager.LoadScene("MainGameScene");
        var json = File.ReadAllText(PathFile);
        var data = JsonUtility.FromJson<SaveData>(json);

        if (turnManager != null) turnManager.turn = data.turn;
        if (Stockpile.Instance != null) Stockpile.Instance.ImportAll(data.resources);

        // Apply building states by id
        var map = new Dictionary<string, Building>();
        foreach (var b in FindObjectsOfType<Building>())
            if (!string.IsNullOrWhiteSpace(b.Id))
                map[b.Id] = b;

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
