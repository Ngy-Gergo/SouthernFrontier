using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit_To_Menu : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
