using UnityEngine;

public class BuildingOnClick : MonoBehaviour
{
    [SerializeField] private BuildingPanelUI panel; // UI panel to open (auto-found if not set)
    private Building building;                      // Cached building data on this object

    private void Awake()
    {
        // Grab the Building component once.
        building = GetComponent<Building>();

        // Find the panel even if it's inactive.
        if (panel == null) panel = FindObjectOfType<BuildingPanelUI>(true);
    }

    private void OnMouseDown()
    {
        if (UIBlock.PointerIsOverUI()) return;
        // Safety: panel might not exist at Awake (scene load order).
        if (panel == null) panel = FindObjectOfType<BuildingPanelUI>(true);

        // Show this building's info.
        panel.Show(building);
    }
}
