using System;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public event Action<int, int, int> OnWaveWarning; // Fired one turn before: (waveIndex, arriveTurn, enemySize)
    public event Action<int, int> OnWaveArrived;      // Fired on arrival: (waveIndex, enemySize)
    public event Action<int> OnWaveWon;               // Fired after winning: (waveIndex)
    public event Action OnGameLost;                   // Fired once on defeat
    public event Action OnGameWon;                    // Fired once after last wave
    public EnemyWavesDefinition WavesDef => wavesDef; // Expose for UI

    [SerializeField] private EnemyWavesDefinition wavesDef; // Wave schedule + loot
    [SerializeField] private TroopDefinitionsDB troopDefs;  // Troop defense values (from TroopDefinition)

    private int _waveIndex = 0;  // Current wave pointer
    private bool _ended = false; // Stops further processing after game end

    public int WaveIndex => _waveIndex;
    public bool Ended => _ended;

    private void OnEnable() => TurnManager.OnNextTurn += HandleTurn; // Listen for turns
    private void OnDisable() => TurnManager.OnNextTurn -= HandleTurn;

    private void Start()
    {
        // Handle “turn 1” UI immediately (warnings/arrival).
        HandleTurn();
    }

    private void HandleTurn()
    {
        // Ignore everything after the game ends.
        if (_ended) return;

        // No data or no more waves.
        if (wavesDef == null || wavesDef.waves == null || _waveIndex >= wavesDef.waves.Count) return;

        // TurnManager must exist to read the current turn.
        if (TurnManager.Instance == null) return;

        int turn = TurnManager.Instance.turn;
        var wave = wavesDef.waves[_waveIndex];

        // “Next turn they arrive” warning.
        if (turn == wave.arriveTurn - 1)
            OnWaveWarning?.Invoke(_waveIndex, wave.arriveTurn, wave.enemySize);

        // Not the arrival turn yet.
        if (turn != wave.arriveTurn) return;

        // Wave starts now.
        OnWaveArrived?.Invoke(_waveIndex, wave.enemySize);

        int enemyPower = wave.enemySize * wave.strengthPerEnemy;

        // Player defense comes from towers + troops.
        int towerDefense = SumTowerDefense();
        int troopDefense = (TroopBank.Instance != null) ? TroopBank.Instance.GetTotalDefense(troopDefs) : 0;
        int playerDefense = towerDefense + troopDefense;

        // Lose if we can't match their power.
        if (playerDefense < enemyPower)
        {
            EndLose();
            return;
        }

        // Win: grant loot to the stockpile.
        if (Stockpile.Instance != null && wave.loot != null)
        {
            for (int i = 0; i < wave.loot.Count; i++)
                Stockpile.Instance.Add(wave.loot[i].type, wave.loot[i].amount);
        }

        OnWaveWon?.Invoke(_waveIndex);
        _waveIndex++;

        // After the last wave, show victory.
        if (_waveIndex >= wavesDef.waves.Count)
            EndWin();
    }

    private int SumTowerDefense()
    {
        // Sum defense from every tower in the scene (fast: no FindObjectsOfType).
        int sum = 0;
        for (int i = 0; i < Tower.All.Count; i++)
            if (Tower.All[i] != null)
                sum += Tower.All[i].GetDefense();
        return sum;
    }

    private void EndLose()
    {
        // End the game once.
        _ended = true;
        OnGameLost?.Invoke();
    }

    private void EndWin()
    {
        // End the game once.
        _ended = true;
        OnGameWon?.Invoke();
    }

    public void ApplySave(int waveIndex)
    {
        // Restore which wave we are on.
        _waveIndex = Mathf.Max(0, waveIndex);
    }
}
