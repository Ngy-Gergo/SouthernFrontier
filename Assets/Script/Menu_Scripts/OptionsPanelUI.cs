using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; // Whole panel to show/hide
    [SerializeField] private Button closeButton;   // Close button
    [SerializeField] private Slider masterSlider;
    private void Awake()
    {
        // Wire close once.
        if (closeButton != null) closeButton.onClick.AddListener(Hide);

        // Start hidden.
        Hide();
    }
    private void OnEnable()
    {
        // Keep UI in sync without firing OnValueChanged.
        if (AudioManager.Instance != null && masterSlider != null)
            masterSlider.SetValueWithoutNotify(AudioManager.Instance.MasterVolume);
    }

    public void OnMasterChanged(float v)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(v);
    }
    public void Show()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
