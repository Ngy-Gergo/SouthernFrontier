using TMPro;
using UnityEngine;

public class WaveWarningUI : MonoBehaviour
{
    [SerializeField] private EnemyWaveManager manager; // Source of wave events
    [SerializeField] private TMP_Text text;            // Warning text label
    [SerializeField] private GameObject root;          // Whole warning widget (show/hide)

    [Header("Estimate")]
    [Range(0f, 0.9f)]
    [SerializeField] private float estimateVariancePercent = 0.2f; // 0.2 = +/-20%
    private void Start()
    {
        Refresh();
    }
    private void Awake()
    {
        // Auto-fill common refs so it works even if not wired.
        if (root == null) root = gameObject;
        if (text == null) text = GetComponentInChildren<TMP_Text>(true);

        // Always visible.
        root.SetActive(true);
    }

    private void OnEnable()
    {
        // Find manager if not assigned.
        if (manager == null) manager = FindObjectOfType<EnemyWaveManager>(true);

        // Update on every turn and also when wave state changes.
        TurnManager.OnNextTurn += Refresh;
        if (manager != null)
        {
            manager.OnWaveArrived += OnWaveStateChanged;
            manager.OnWaveWon += OnWaveStateChanged;
            manager.OnGameLost += Refresh;
            manager.OnGameWon += Refresh;
        }

        // Draw once on start.
        Refresh();
    }

    private void OnDisable()
    {
        TurnManager.OnNextTurn -= Refresh;

        if (manager != null)
        {
            manager.OnWaveArrived -= OnWaveStateChanged;
            manager.OnWaveWon -= OnWaveStateChanged;
            manager.OnGameLost -= Refresh;
            manager.OnGameWon -= Refresh;
        }
    }

    private void OnWaveStateChanged(int a, int b) => Refresh();
    private void OnWaveStateChanged(int a) => Refresh();

    private void Refresh()
    {
        if (root == null || text == null)
            return;

        if (manager == null) manager = FindObjectOfType<EnemyWaveManager>(true);

        // If core refs are missing, show a safe placeholder.
        if (TurnManager.Instance == null || manager == null || manager.WavesDef == null || manager.WavesDef.waves == null)
        {
            text.text = "Next wave: -";
            return;
        }

        // End states.
        if (manager.Ended)
        {
            text.text = "Waves ended.";
            return;
        }

        int waveIndex = manager.WaveIndex;

        // No more waves.
        if (waveIndex >= manager.WavesDef.waves.Count)
        {
            text.text = "All waves defeated.";
            return;
        }

        int turn = TurnManager.Instance.turn;
        var wave = manager.WavesDef.waves[waveIndex];

        int turnsLeft = wave.arriveTurn - turn;

        int enemyPower = wave.enemySize * wave.strengthPerEnemy;

        int min = Mathf.FloorToInt(enemyPower * (1f - estimateVariancePercent));
        int max = Mathf.CeilToInt(enemyPower * (1f + estimateVariancePercent));

        // Display message.
        if (turnsLeft > 0)
        {
            text.text = $"In {turnsLeft} turn(s) enemies will arrive: {min}-{max}";
        }
        else if (turnsLeft == 0)
        {
            text.text = $"Wave {waveIndex + 1} arriving now! | size: {wave.enemySize} | est power: {min}-{max}";
        }
        else
        {
            // Should not happen unless arriveTurn is set wrong or turns were changed by load/debug.
            text.text = $"Wave {waveIndex + 1} overdue | size: {wave.enemySize} | est power: {min}-{max}";
        }
    }
}
