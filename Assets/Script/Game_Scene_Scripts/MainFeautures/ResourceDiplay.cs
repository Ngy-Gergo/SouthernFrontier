using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceType type; // Which resource this label shows
    [SerializeField] private TMP_Text text;     // Target TMP text (auto-filled if empty)

    private Stockpile _stockpile; // Cached reference for updates

    private void Awake()
    {
        // Grab the TMP text on this object if not assigned.
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        // Hook up to the stockpile and draw once.
        TryHook();
        Refresh();
    }

    private void Update()
    {
        // Scene load order can be weird: keep trying until stockpile exists.
        if (_stockpile == null)
        {
            TryHook();
            if (_stockpile != null) Refresh();
        }
    }

    private void OnDisable()
    {
        // Clean unsubscribe.
        if (_stockpile != null) _stockpile.OnChanged -= Refresh;
        _stockpile = null;
    }

    private void TryHook()
    {
        // Find the stockpile (even if it was spawned later).
        if (_stockpile != null) return;

        _stockpile = Stockpile.Instance != null ? Stockpile.Instance : FindObjectOfType<Stockpile>();
        if (_stockpile != null) _stockpile.OnChanged += Refresh;
    }

    private void Refresh()
    {
        // Update the label value.
        if (text == null) return;
        text.text = (_stockpile != null) ? _stockpile.Get(type).ToString() : "0";
    }
}
