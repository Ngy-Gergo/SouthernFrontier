using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Recipes DB")]
public class ResourceRecipesDB : ScriptableObject
{
    public List<ResourceRecipe> recipes = new List<ResourceRecipe>(); // Inspector list of recipes

    private Dictionary<ResourceType, ResourceRecipe> _map; // Fast lookup cache

    public bool TryGet(ResourceType output, out ResourceRecipe recipe)
    {
        // Build the cache the first time we query.
        if (_map == null)
        {
            _map = new Dictionary<ResourceType, ResourceRecipe>();

            foreach (var r in recipes)
                if (r != null) _map[r.output] = r;
        }

        // Return true only if we have a valid recipe.
        return _map.TryGetValue(output, out recipe) && recipe != null;
    }
}
