using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopTrainRowUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;   // Troop icon
    [SerializeField] private TMP_Text nameText; // Troop name
    [SerializeField] private TMP_Text countText; // Owned count
    [SerializeField] private Button trainButton; // Train 1 unit

    private TroopDefinition _troop;            // Row data
    private TroopTrainingBuilding _building;   // Building that does the training

    public void Bind(TroopTrainingBuilding building, TroopDefinition troop)
    {
        // Hook this row up to a troop option.
        _building = building;
        _troop = troop;

        if (iconImage != null) iconImage.sprite = troop != null ? troop.icon : null;
        if (nameText != null) nameText.text = troop != null ? troop.displayName : "-";

        // One click = train one troop.
        trainButton.onClick.RemoveAllListeners();
        trainButton.onClick.AddListener(() =>
        {
            if (_building != null && _troop != null)
                _building.TryTrain(_troop, 1);

            Refresh();
        });

        Refresh();
    }

    public void Refresh()
    {
        // Update count and button state.
        if (_troop == null) return;

        int owned = TroopBank.Instance != null ? TroopBank.Instance.Get(_troop.type) : 0;
        if (countText != null) countText.text = owned.ToString();

        bool canTrain = (_building != null) && _building.CanTrain(_troop, 1);
        if (trainButton != null) trainButton.interactable = canTrain;
    }
}
