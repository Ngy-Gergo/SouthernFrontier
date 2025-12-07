using NUnit.Framework;
using UnityEngine;
public class TurnManagerTests
{
    [Test]
    public void NextTurn_IncrementsTurn_AndInvokesEvent()
    {
        // Arrange
        var go = new GameObject("TurnManager");
        var tm = go.AddComponent<TurnManager>();
        tm.turn = 1;

        int callCount = 0;
        System.Action handler = () => callCount++;

        TurnManager.OnNextTurn += handler;

        try
        {
            // Act
            tm.NextTurn();

            // Assert
            Assert.AreEqual(2, tm.turn, "Turn should increment by 1.");
            Assert.AreEqual(1, callCount, "OnNextTurn should fire exactly once.");
        }
        finally
        {
            TurnManager.OnNextTurn -= handler;
            Object.DestroyImmediate(go);
        }
    }
}
