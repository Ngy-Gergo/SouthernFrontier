using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Recipe")]
public class ResourceRecipe : ScriptableObject
{
    public ResourceType output;
    public List<ResourceAmount> inputs = new();
}
