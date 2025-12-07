using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static event Action OnNextTurn;

    public int turn = 1;

    public void NextTurn()
    {
        turn++;
        OnNextTurn?.Invoke();
    }
}


