using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot; // The actual panel GameObject we show/hide

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;       // Building name
    [SerializeField] private TMP_Text descText;        // Building description
    [SerializeField] private TMP_Text levelText;       // Current level
    [SerializeField] private TMP_Text productionText;  // Output summary

    [Header("Controls")]
    [SerializeField] private TMP_Dropdown outputDropdown; // Pick what this building produces
    [SerializeField] private Button levelUpButton;        // Upgrade button
    [SerializeField] private Button closeButton;          // Close panel

    private Building _b; // Currently selected building

    private void Awake()
    {
        // Wire UI once.
        closeButton.onClick.AddListener(Hide);
        levelUpButton.onClick.AddListener(OnLevelUp);
        outputDropdown.onValueChanged.AddListener(OnOutputChanged);

        // Hide the visuals at start (keeps this component alive for first click).
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void Hide()
    {
        // Close and forget the selection.
        if (panelRoot != null) panelRoot.SetActive(false);
        _b = null;
    }

    public void Show(Building b)
    {
        // Open panel for the clicked building.
        _b = b;
        if (panelRoot != null) panelRoot.SetActive(true);

        RebuildDropdown();
        Refresh();
    }

    private void RebuildDropdown()
    {
        // Build dropdown from the building's production options.
        outputDropdown.ClearOptions();
        if (_b == null || _b.Definition == null) return;

        var opts = new List<string>();
        foreach (var o in _b.Definition.productionOptions)
            opts.Add(o.output.ToString());

        outputDropdown.AddOptions(opts);
        outputDropdown.value = Mathf.Clamp(_b.SelectedOptionIndex, 0, Mathf.Max(0, opts.Count - 1));
        outputDropdown.RefreshShownValue();
    }

    private void Refresh()
    {
        // Push building data into the UI.
        if (_b == null) return;

        titleText.text = _b.DisplayName;
        descText.text = _b.Description;
        levelText.text = $"Level: {_b.Level}";

        var opt = _b.GetSelectedOption();
        if (opt != null)
        {
            int perTurn = opt.basePerTurn + (opt.perLevel * _b.Level);
            productionText.text = $"Produces: {opt.output} (+{perTurn}/turn)";
        }
        else
        {
            productionText.text = "Produces: -";
        }

        // Disable upgrade if we're already maxed.
        levelUpButton.interactable = _b.CanLevelUp();
    }

    private void OnLevelUp()
    {
        // Upgrade then refresh UI.
        if (_b == null) return;
        _b.TryLevelUp();
        Refresh();
    }

    private void OnOutputChanged(int idx)
    {
        // Switch output type then refresh.
        if (_b == null) return;
        _b.SelectOption(idx);
        Refresh();
    }
}
