using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;       // Whole pause menu
    [SerializeField] private OptionsPanelUI optionsPanel; // Same options panel concept
    [SerializeField] private ConfirmOverwriteUI confirm; // Overwrite warning panel

    private void Awake()
    {
        // Start hidden.
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void Open()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Continue()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void SaveGame()
    {
        if (confirm == null)
        {
            Debug.LogError("PauseMenuUI: ConfirmOverwriteUI is NULL (not assigned or destroyed).");
            return;
        }

        if (Save_Load_Game.Instance == null)
        {
            Debug.LogError("PauseMenuUI: Save_Load_Game.Instance is NULL.");
            return;
        }

        confirm.Ask(() => Save_Load_Game.Instance.Save());
    }


    public void Options()
    {
        if (optionsPanel != null) optionsPanel.Show();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(GamePaths.MenuScene);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
