using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit_To_Menu : MonoBehaviour
{
    // Called by the UI button to return to the main menu.
    public void Menu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
