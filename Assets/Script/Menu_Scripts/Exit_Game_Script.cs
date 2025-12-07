using UnityEngine;
using System;

public class Exit_Game_Script : MonoBehaviour
{
    // Called by the UI button to close the game.
    public void Exit_Game()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Stop play mode when testing in the editor.
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
