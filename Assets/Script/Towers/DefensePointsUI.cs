using System.Collections;
using TMPro;
using UnityEngine;

public class DefensePointsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;                // Target TMP label
    [SerializeField] private TroopDefinitionsDB troopDefs; // Maps TroopType -> TroopDefinition (defense)

    private Coroutine _hookRoutine;

    private void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _hookRoutine = StartCoroutine(HookWhenReady());
    }

    private void OnDisable()
    {
        if (_hookRoutine != null) StopCoroutine(_hookRoutine);
        _hookRoutine = null;

        if (TroopBank.Instance != null) TroopBank.Instance.OnChanged -= Refresh;
        Tower.OnAnyTowerChanged -= Refresh; // only if you kept this event
    }

    private IEnumerator HookWhenReady()
    {
        // Wait until required singletons exist (scene load order safe).
        while (TroopBank.Instance == null || Stockpile.Instance == null || TurnManager.Instance == null)
            yield return null;

        TroopBank.Instance.OnChanged += Refresh;
        Tower.OnAnyTowerChanged += Refresh; // only if you kept this event

        Refresh();
    }

    private void Refresh()
    {
        if (text == null) return;

        int troopDefense = (TroopBank.Instance != null) ? TroopBank.Instance.GetTotalDefense(troopDefs) : 0;

        int towerDefense = 0;
        for (int i = 0; i < Tower.All.Count; i++)
            if (Tower.All[i] != null)
                towerDefense += Tower.All[i].GetDefense();

        int total = troopDefense + towerDefense;
        text.text = $"Defense: {total}";
    }
}
