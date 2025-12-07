using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Set_Resolution_Script : MonoBehaviour 
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle ScreenModeToggle;
    public void Apply()
    {
        int w = 1920, h = 1080;
        if (resolutionDropdown.value == 1) { w = 2560; h = 1440; }

        var mode = ScreenModeToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(w, h, mode);
    }
}