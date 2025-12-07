using TMPro;
using UnityEngine;

public class WaveWarningUI : MonoBehaviour
{
    [SerializeField] private EnemyWaveManager manager; // Source of wave events
    [SerializeField] private TMP_Text text;            // Warning text label
    [SerializeField] private GameObject root;          // Whole warning widget (show/hide)

    private void Awake()
    {
        // Start hidden.
        if (root != null) root.SetActive(false);
    }

    private void OnEnable()
    {
        // Listen for the "one turn left" warning.
        if (manager != null) manager.OnWaveWarning += Show;
    }

    private void OnDisable()
    {
        if (manager != null) manager.OnWaveWarning -= Show;
    }

    private void Show(int waveIndex, int arriveTurn, int enemySize)
    {
        // Pop the warning and update the message.
        if (root != null) root.SetActive(true);
        if (text != null) text.text = $"Wave {waveIndex + 1} arrives on turn {arriveTurn} (size: {enemySize})";
    }
}
