using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    public int arriveTurn = 5;            // Turn when the wave hits
    public int enemySize = 20;            // How many enemies show up
    public int strengthPerEnemy = 5;      // Power per enemy (enemyPower = size * strength)
    public List<ResourceAmount> loot = new List<ResourceAmount>(); // Rewards for winning
}

[CreateAssetMenu(menuName = "Game/Enemy Waves Definition")]
public class EnemyWavesDefinition : ScriptableObject
{
    public List<Wave> waves = new List<Wave>(); // Ordered list of waves
}
