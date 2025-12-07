using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Set_Resolution_Script : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Picks the resolution preset
    [SerializeField] private Toggle screenModeToggle;         // Fullscreen window vs windowed

    public void Apply()
    {
        // Map dropdown index to a resolution.
        int w = 1920, h = 1080;
        if (resolutionDropdown.value == 1) { w = 2560; h = 1440; }

        // Toggle decides the screen mode.
        var mode = screenModeToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(w, h, mode);
    }
}
