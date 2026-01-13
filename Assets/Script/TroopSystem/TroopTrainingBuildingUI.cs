using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopTrainingBuildingUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;   // Whole panel to show/hide
    [SerializeField] private Image buildingIcon;     // Header icon
    [SerializeField] private TMP_Text buildingName;  // Header name

    [Header("Single troop row")]
    [SerializeField] private TroopTrainRowUI row;    // One static row in the UI

    [SerializeField] private Button closeButton;     // Close panel button

    private TroopTrainingBuilding _building;         // Currently selected troop building
    private TroopDefinition _troop;                  // The only troop this building can train

    private void Awake()
    {
        // Wire close once.
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        Hide();
    }

    private void OnEnable()
    {
        // Refresh when resources or troops change.
        if (Stockpile.Instance != null) Stockpile.Instance.OnChanged += Refresh;
        if (TroopBank.Instance != null) TroopBank.Instance.OnChanged += Refresh;
    }

    private void OnDisable()
    {
        if (Stockpile.Instance != null) Stockpile.Instance.OnChanged -= Refresh;
        if (TroopBank.Instance != null) TroopBank.Instance.OnChanged -= Refresh;
    }

    public void Show(TroopTrainingBuilding building)
    {
        // Open panel for this building.
        _building = building;
        if (panelRoot != null) panelRoot.SetActive(true);

        var def = _building != null ? _building.Definition : null;
        if (buildingName != null) buildingName.text = def != null ? def.displayName : "Troops";
        if (buildingIcon != null) buildingIcon.sprite = def != null ? def.icon : null;

        // One troop per building: use the first entry in the list.
        _troop = (def != null && def.trainableTroops != null && def.trainableTroops.Count > 0)
            ? def.trainableTroops[0]
            : null;

        // Bind the single row once.
        if (row != null && _building != null && _troop != null)
            row.Bind(_building, _troop);

        Refresh();
    }

    public void Hide()
    {
        // Close and forget the selection.
        if (panelRoot != null) panelRoot.SetActive(false);
        _building = null;
        _troop = null;
    }

    private void Refresh()
    {
        // Update the single row.
        if (row != null) row.Refresh();

        // Optional: auto-close if config is missing.
        // if (_building == null || _troop == null) Hide();
    }
}
