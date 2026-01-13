using UnityEngine;

public class TowerOnClick : MonoBehaviour
{
    [SerializeField] private TowerPanelUI panel; // UI panel to open (auto-found if not set)
    private Tower _tower;                        // Cached tower component

    private void Awake()
    {
        _tower = GetComponent<Tower>();
        if (panel == null) panel = FindObjectOfType<TowerPanelUI>(true);
    }

    private void OnMouseDown()
    {
        if (UIBlock.PointerIsOverUI()) return;
        if (panel == null) panel = FindObjectOfType<TowerPanelUI>(true);
        if (panel != null && _tower != null)
            panel.Show(_tower);
    }
}
