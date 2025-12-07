using UnityEngine;
using UnityEngine.SceneManagement;

public class New_Game_Script : MonoBehaviour
{
    // Called by the New Game button.
    public void New_Game()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
