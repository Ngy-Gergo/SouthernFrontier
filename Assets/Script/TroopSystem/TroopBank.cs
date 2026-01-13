using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TroopAmount
{
    public TroopType type; // Which troop
    public int amount;     // How many
}

public class TroopBank : MonoBehaviour
{
    public static TroopBank Instance { get; private set; } // Global access
    public event Action OnChanged;                          // Fired when troop counts change

    [Header("Starting troops")]
    [SerializeField] private List<TroopAmount> starting = new List<TroopAmount>(); // Starting counts

    private readonly Dictionary<TroopType, int> _counts = new Dictionary<TroopType, int>(); // Troops owned

    private void Awake()
    {
        // Singleton setup.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Initialize all troop types to 0.
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
            _counts[t] = 0;

        for (int i = 0; i < starting.Count; i++)
            _counts[starting[i].type] = Mathf.Max(0, starting[i].amount);

        OnChanged?.Invoke();
    }

    public int Get(TroopType type) => _counts.TryGetValue(type, out var v) ? v : 0; // Safe read

    public void Add(TroopType type, int amount)
    {
        // Add troops and notify UI.
        if (amount <= 0) return;

        _counts[type] = Get(type) + amount;
        OnChanged?.Invoke();
    }

    public int GetTotalDefense(TroopDefinitionsDB db)
    {
        // Sum defense based on troop counts and troop definitions.
        if (db == null) return 0;

        int total = 0;
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
        {
            var def = db.Get(t);
            if (def != null) total += Get(t) * def.defense;
        }

        return total;
    }

    public List<TroopAmount> ExportAll()
    {
        // Dump troop counts for saving.
        var list = new List<TroopAmount>();
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
            list.Add(new TroopAmount { type = t, amount = Get(t) });

        return list;
    }

    public void ImportAll(List<TroopAmount> list)
    {
        // Reset everything then apply imported values.
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
            _counts[t] = 0;

        if (list != null)
            for (int i = 0; i < list.Count; i++)
                _counts[list[i].type] = list[i].amount;

        OnChanged?.Invoke();
    }
}
