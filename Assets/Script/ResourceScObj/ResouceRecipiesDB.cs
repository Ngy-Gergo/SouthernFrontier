using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Recipes DB")]
public class ResourceRecipesDB : ScriptableObject
{
    public List<ResourceRecipe> recipes = new();

    private Dictionary<ResourceType, ResourceRecipe> _map;

    public bool TryGet(ResourceType output, out ResourceRecipe recipe)
    {
        if (_map == null)
        {
            _map = new Dictionary<ResourceType, ResourceRecipe>();
            foreach (var r in recipes)
                if (r != null) _map[r.output] = r;
        }
        return _map.TryGetValue(output, out recipe) && recipe != null;
    }
}
