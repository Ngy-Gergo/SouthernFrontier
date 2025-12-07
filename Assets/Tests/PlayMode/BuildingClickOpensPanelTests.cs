using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BuildingClickOpensPanelTests
{
    private const string GameSceneName = "MainGameScene";

    [UnityTest]
    public IEnumerator ClickingProductionBuilding_OpensPanel_ButMilitaryDoesNot()
    {
        yield return SceneManager.LoadSceneAsync(GameSceneName, LoadSceneMode.Single);
        yield return null; // let Awake/Start run

        // Find the production panel (include inactive).
        var panelUI = UnityEngine.Object.FindFirstObjectByType<BuildingPanelUI>(FindObjectsInactive.Include);
        Assert.IsNotNull(panelUI, "No BuildingPanelUI found in the Game scene.");

        // Grab panelRoot so we can check if it opened.
        var panelRootField = typeof(BuildingPanelUI)
            .GetField("panelRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(panelRootField, "BuildingPanelUI does not have a private field named 'panelRoot'.");

        var panelRoot = panelRootField.GetValue(panelUI) as GameObject;
        Assert.IsNotNull(panelRoot, "BuildingPanelUI.panelRoot is not assigned in the scene.");

        // Ensure we start closed.
        panelRoot.SetActive(false);
        Assert.IsFalse(panelRoot.activeSelf, "Panel root should start inactive.");

        // Find a clickable production building (BuildingOnClick but not troop building).
        var clickType = FindTypeByName("BuildingOnClick");
        Assert.IsNotNull(clickType, "Could not find type 'BuildingOnClick'. Rename in test or ensure script compiles.");

        var clickComponents = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(mb => mb != null && mb.GetType() == clickType)
            .ToArray();

        Assert.IsTrue(clickComponents.Length > 0, "No BuildingOnClick component found in the Game scene.");

        var productionClick = clickComponents
            .FirstOrDefault(mb => mb.GetComponent<TroopTrainingBuilding>() == null);

        Assert.IsNotNull(productionClick, "No production building found (BuildingOnClick without TroopTrainingBuilding).");

        // Click production building -> panel should open.
        productionClick.gameObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        yield return null;

        Assert.IsTrue(panelRoot.activeSelf, "Clicking production building should activate the production panel.");

        // Reset to closed.
        panelRoot.SetActive(false);
        Assert.IsFalse(panelRoot.activeSelf);

        // Click a troop building -> should NOT open the production panel.
        var military = UnityEngine.Object.FindFirstObjectByType<TroopTrainingBuilding>(FindObjectsInactive.Include);
        Assert.IsNotNull(military, "No TroopTrainingBuilding found in the Game scene.");

        military.gameObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        yield return null;

        Assert.IsFalse(panelRoot.activeSelf, "Clicking military building should NOT open the production panel.");
    }

    private static Type FindTypeByName(string typeName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetTypes().FirstOrDefault(x => x.Name == typeName);
            if (t != null) return t;
        }
        return null;
    }
}
