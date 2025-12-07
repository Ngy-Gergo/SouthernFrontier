using UnityEngine;

public class TogglePanel_Script : MonoBehaviour
{
    public GameObject panel; // Panel to show/hide

    public void Open_Close_Panel()
    {
        // Flip the panel active state.
        if (panel == null) return;
        panel.SetActive(!panel.activeSelf);
    }
}
