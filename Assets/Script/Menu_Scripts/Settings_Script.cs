using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings_Script : MonoBehaviour
{
    // Called by the Settings button.
    public void GoTo_Setting()
    {
        SceneManager.LoadScene("AsaSA");
    }
}
