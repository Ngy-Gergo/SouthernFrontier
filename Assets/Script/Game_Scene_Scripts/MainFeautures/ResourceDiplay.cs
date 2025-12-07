using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceType type;
    [SerializeField] private TMP_Text text;

    private Stockpile _stockpile;

    private void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        TryHook();
        Refresh();
    }

    private void Update()
    {
        // If Stockpile wasn't ready yet, hook later.
        if (_stockpile == null)
        {
            TryHook();
            if (_stockpile != null) Refresh();
        }
    }

    private void OnDisable()
    {
        if (_stockpile != null)
            _stockpile.OnChanged -= Refresh;

        _stockpile = null;
    }

    [System.Obsolete]
    private void TryHook()
    {
        if (_stockpile != null) return;

        _stockpile = Stockpile.Instance != null ? Stockpile.Instance : FindObjectOfType<Stockpile>();
        if (_stockpile != null)
            _stockpile.OnChanged += Refresh;
    }

    private void Refresh()
    {
        if (text == null) return;

        if (_stockpile == null)
        {
            text.text = "0";
            return;
        }

        text.text = _stockpile.Get(type).ToString();
    }
}
