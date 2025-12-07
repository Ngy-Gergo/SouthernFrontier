using UnityEngine;

public class TroopBuildingClickOpenUI : MonoBehaviour
{
    [SerializeField] private TroopTrainingBuildingUI panel; // Troop UI panel to open
    private TroopTrainingBuilding _b;                       // Cached troop-building component

    private void Awake()
    {
        // Grab the building logic once.
        _b = GetComponent<TroopTrainingBuilding>();
    }

    private void OnMouseDown()
    {
        // Open the troop panel for this building.
        if (panel != null && _b != null)
            panel.Show(_b);
    }
}
