using UnityEngine;

public class BuildingOnClick : MonoBehaviour
{
    [SerializeField] private BuildingPanelUI panel;
    private Building _b;

    private void Awake() => _b = GetComponent<Building>();

    private void OnMouseDown()
    {
        if (panel != null && _b != null)
            panel.Show(_b);
    }
}
