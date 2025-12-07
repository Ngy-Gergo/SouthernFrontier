using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class NewGameSceneLoadTests
{
    private const string MenuSceneName = "MenuScene";       // Menu scene name (Build Settings)
    private const string GameSceneName = "MainGameScene";   // Game scene name (Build Settings)

    [UnityTest]
    public IEnumerator NewGameButton_LoadsGameScene()
    {
        // Start from the menu scene.
        yield return SceneManager.LoadSceneAsync(MenuSceneName, LoadSceneMode.Single);
        yield return null; // Let UI initialize.

        // Find the New Game button by GameObject name.
        var buttonGo = GameObject.Find("New Game");
        Assert.IsNotNull(buttonGo, "Could not find GameObject named 'New Game' in the Menu scene.");

        var button = buttonGo.GetComponent<Button>();
        Assert.IsNotNull(button, "'New Game' has no Button component.");

        // Simulate a click.
        button.onClick.Invoke();

        // Give the scene load a short window to complete.
        for (int i = 0; i < 120; i++)
        {
            if (SceneManager.GetActiveScene().name == GameSceneName)
                break;

            yield return null;
        }

        Assert.AreEqual(GameSceneName, SceneManager.GetActiveScene().name, "New Game did not load the Game scene.");
    }
}
