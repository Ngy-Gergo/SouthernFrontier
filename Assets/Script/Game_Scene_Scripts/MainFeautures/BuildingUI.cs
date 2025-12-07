using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text productionText;

    [Header("Controls")]
    [SerializeField] private TMP_Dropdown outputDropdown;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Button closeButton;

    private Building _b;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
        levelUpButton.onClick.AddListener(OnLevelUp);
        outputDropdown.onValueChanged.AddListener(OnOutputChanged);

        Hide();
    }

    public void Show(Building b)
    {
        _b = b;
        panelRoot.SetActive(true);

        RebuildDropdown();
        Refresh();
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        _b = null;
    }

    private void RebuildDropdown()
    {
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
        else productionText.text = "Produces: -";

        levelUpButton.interactable = _b.CanLevelUp();
    }

    private void OnLevelUp()
    {
        if (_b == null) return;
        _b.TryLevelUp();
        Refresh();
    }

    private void OnOutputChanged(int idx)
    {
        if (_b == null) return;
        _b.SelectOption(idx);
        Refresh();
    }
}
