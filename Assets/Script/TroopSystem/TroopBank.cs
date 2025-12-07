using System;
using System.Collections.Generic;
using UnityEngine;

public class TroopBank : MonoBehaviour
{
    public static TroopBank Instance { get; private set; } // Global access
    public event Action OnChanged;                          // Fired when troop counts change

    private readonly Dictionary<TroopType, int> _counts = new Dictionary<TroopType, int>(); // Troops owned

    private void Awake()
    {
        // Singleton setup.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Initialize all troop types to 0.
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
            _counts[t] = 0;
    }

    public int Get(TroopType type) => _counts.TryGetValue(type, out var v) ? v : 0; // Safe read

    public void Add(TroopType type, int amount)
    {
        // Add troops and notify UI.
        if (amount <= 0) return;

        _counts[type] = Get(type) + amount;
        OnChanged?.Invoke();
    }

    public int GetTotalDefense(TroopStatsDB db)
    {
        // Sum defense based on troop counts and stats.
        if (db == null) return 0;

        int total = 0;
        foreach (TroopType t in Enum.GetValues(typeof(TroopType)))
        {
            var s = db.Get(t);
            if (s != null) total += Get(t) * s.defense;
        }

        return total;
    }
}
