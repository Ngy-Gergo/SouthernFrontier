using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; // The actual panel GameObject we show/hide
    [SerializeField] private TMP_Text titleText;   // "Tower"
    [SerializeField] private TMP_Text levelText;   // Level: X
    [SerializeField] private TMP_Text defenseText; // Defense: Y
    [SerializeField] private TMP_Text costText;    // Cost: ...
    [SerializeField] private Button upgradeButton; // Upgrade
    [SerializeField] private Button closeButton;   // Close

    private Tower _t;

    private void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        if (upgradeButton != null) upgradeButton.onClick.AddListener(Upgrade);

        Hide();
    }

    public void Show(Tower t)
    {
        _t = t;
        if (panelRoot != null) panelRoot.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        _t = null;
    }

    private void Upgrade()
    {
        if (_t == null) return;
        _t.TryUpgrade();
        Refresh();
    }

    private void Refresh()
    {
        if (_t == null) return;

        if (titleText != null) titleText.text = "Tower";
        if (levelText != null) levelText.text = $"Level: {_t.Level+1}";
        if (defenseText != null) defenseText.text = $"Defense: {_t.GetDefense()}";

        bool canUpgrade = _t.CanUpgrade() && Stockpile.Instance != null;

        List<ResourceAmount> cost = null;
        if (_t.CanUpgrade())
        {
            // Next level cost is stored in TowerDefinition.levels[next].upgradeCost
            // (Tower.TryUpgrade uses the same source.)
            int next = _t.Level + 1;
            var defField = typeof(Tower).GetField("def", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var def = defField != null ? defField.GetValue(_t) as TowerDefinition : null;

            if (def != null && def.levels != null && next >= 0 && next < def.levels.Count)
                cost = def.levels[next].upgradeCost;
        }

        if (costText != null)
            costText.text = (cost == null) ? "Cost: -" : $"Cost: {CostToString(cost)}";

        if (upgradeButton != null)
        {
            if (canUpgrade && cost != null)
                upgradeButton.interactable = Stockpile.Instance.CanAfford(cost);
            else
                upgradeButton.interactable = false;
        }
    }

    private string CostToString(List<ResourceAmount> cost)
    {
        if (cost == null || cost.Count == 0) return "-";

        var parts = new List<string>();
        for (int i = 0; i < cost.Count; i++)
            parts.Add($"{cost[i].type}:{cost[i].amount}");

        return string.Join("  ", parts);
    }
}
