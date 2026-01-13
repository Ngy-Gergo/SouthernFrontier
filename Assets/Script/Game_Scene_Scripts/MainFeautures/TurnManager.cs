using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; } // Easy access for UI/logic
    public static event Action OnNextTurn;                   // Fired after turn increments

    public int turn = 1; // Current turn number

    private void Awake()
    {
        // Singleton setup (prevents duplicates overriding Instance).
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void NextTurn()
    {
        // Called by the Next Turn button.
        turn++;
        OnNextTurn?.Invoke();
    }
}
