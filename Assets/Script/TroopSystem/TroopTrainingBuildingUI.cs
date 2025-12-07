using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopTrainingBuildingUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; // Whole panel to show/hide
    [SerializeField] private Image buildingIcon;   // Header icon
    [SerializeField] private TMP_Text buildingName; // Header name

    [SerializeField] private Transform rowsParent;      // Where rows get spawned
    [SerializeField] private TroopTrainRowUI rowPrefab; // Row template

    [SerializeField] private Button closeButton; // Close panel button

    private TroopTrainingBuilding _building; // Currently selected troop building
    private readonly List<TroopTrainRowUI> _rows = new List<TroopTrainRowUI>(); // Spawned rows

    private void Awake()
    {
        // Wire close once.
        closeButton.onClick.AddListener(Hide);
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

        RebuildRows();
        Refresh();
    }

    public void Hide()
    {
        // Close panel and clean up rows.
        if (panelRoot != null) panelRoot.SetActive(false);
        _building = null;
        ClearRows();
    }

    private void RebuildRows()
    {
        // Recreate rows from the building definition.
        ClearRows();
        if (_building == null || _building.Definition == null) return;

        var troops = _building.Definition.trainableTroops;
        for (int i = 0; i < troops.Count; i++)
        {
            var t = troops[i];
            if (t == null) continue;

            var row = Instantiate(rowPrefab, rowsParent);
            row.Bind(_building, t);
            _rows.Add(row);
        }
    }

    private void Refresh()
    {
        // Let every row update its count + button state.
        for (int i = 0; i < _rows.Count; i++)
            _rows[i].Refresh();
    }

    private void ClearRows()
    {
        // Destroy previously spawned rows.
        for (int i = 0; i < _rows.Count; i++)
            if (_rows[i] != null) Destroy(_rows[i].gameObject);

        _rows.Clear();
    }
}
