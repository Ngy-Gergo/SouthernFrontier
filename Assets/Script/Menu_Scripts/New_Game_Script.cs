using UnityEngine;
using UnityEngine.SceneManagement;

public class New_Game_Script : MonoBehaviour
{
    public void New_Game()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
