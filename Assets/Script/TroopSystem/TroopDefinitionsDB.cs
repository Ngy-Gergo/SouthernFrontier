using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Troop Definitions DB")]
public class TroopDefinitionsDB : ScriptableObject
{
    public List<TroopDefinition> troops = new List<TroopDefinition>();

    private Dictionary<TroopType, TroopDefinition> _map;

    public TroopDefinition Get(TroopType type)
    {
        // Build cache on first use.
        if (_map == null)
        {
            _map = new Dictionary<TroopType, TroopDefinition>();
            for (int i = 0; i < troops.Count; i++)
                if (troops[i] != null)
                    _map[troops[i].type] = troops[i];
        }

        return _map.TryGetValue(type, out var def) ? def : null;
    }
}
