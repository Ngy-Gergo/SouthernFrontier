using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Recipe")]
public class ResourceRecipe : ScriptableObject
{
    public ResourceType output; // What this recipe produces
    public List<ResourceAmount> inputs = new List<ResourceAmount>(); // Required inputs per 1 output
}
