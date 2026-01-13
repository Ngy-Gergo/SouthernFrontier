using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private EnemyWaveManager manager; // Source of win/lose events

    [Header("Panels")]
    [SerializeField] private GameObject defeatPanel;   // Shown on loss
    [SerializeField] private GameObject victoryPanel;  // Shown on final win


    private void Awake()
    {
        // Start hidden.
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (manager == null) manager = FindAnyObjectByType<EnemyWaveManager>();

        if (manager != null)
        {
            manager.OnGameLost += ShowDefeat;
            manager.OnGameWon += ShowVictory;
        }
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.OnGameLost -= ShowDefeat;
            manager.OnGameWon -= ShowVictory;
        }
    }

    private void ShowDefeat()
    {
        // Show defeat once and lock gameplay.
        if (defeatPanel != null) defeatPanel.SetActive(true);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    private void ShowVictory()
    {
        // Show victory once and lock gameplay.
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    // Wire these to buttons on the victory/defeat panels.
    public void ExitToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
