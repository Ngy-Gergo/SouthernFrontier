using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; } // Easy access for UI/logic
    public static event Action OnNextTurn;                   // Fired after turn increments

    public int turn = 1; // Current turn number

    private void Awake() => Instance = this; // Set singleton ref

    public void NextTurn()
    {
        // Called by the Next Turn button.
        turn++;
        OnNextTurn?.Invoke();
    }
}
