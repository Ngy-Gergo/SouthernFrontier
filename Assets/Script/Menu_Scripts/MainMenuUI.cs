using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Panels")]
    [SerializeField] private OptionsPanelUI optionsPanel; // The settings/options panel (in the menu scene)

    private void Start()
    {
        // Load is only possible if a save exists.
        if (loadGameButton != null)
            loadGameButton.interactable = Save_Load_Game.Instance != null && Save_Load_Game.Instance.HasSave();
    }

    public void NewGame()
    {
        SceneManager.LoadScene(GamePaths.MainGameScene);
    }

    public void LoadGame()
    {
        if (Save_Load_Game.Instance == null) return;
        Save_Load_Game.Instance.Load();
    }

    public void OpenSettings()
    {
        if (optionsPanel != null) optionsPanel.Show();
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
