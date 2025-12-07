using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Placeholder for a future wave entry (size + arrival turn).
    // Use a simple struct/class here unless you really need records.
    public record Enemies
    {
        public int size;
        public int arriveOnTurn;
    }

    public void EnemyWave()
    {
        // Example wave creation (wire this into your wave manager later).
        Enemies enemy = new Enemies
        {
            size = 20,
            arriveOnTurn = 5
        };
    }
}
