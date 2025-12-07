using UnityEngine;
using System;

public class Exit_Game_Script : MonoBehaviour
{
    public void Exit_Game()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
