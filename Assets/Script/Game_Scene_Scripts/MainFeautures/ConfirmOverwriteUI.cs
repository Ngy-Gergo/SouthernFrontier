using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmOverwriteUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; // Whole confirm popup root
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action _onYes;

    private void Awake()
    {
        // Start hidden.
        if (panelRoot != null) panelRoot.SetActive(false);

        if (yesButton != null)
            yesButton.onClick.AddListener(() =>
            {
                if (panelRoot != null) panelRoot.SetActive(false);
                var cb = _onYes;
                _onYes = null;
                cb?.Invoke();
            });

        if (noButton != null)
            noButton.onClick.AddListener(() =>
            {
                if (panelRoot != null) panelRoot.SetActive(false);
                _onYes = null;
            });
    }

    public void Ask(Action onYes)
    {
        _onYes = onYes;

        // Force visible.
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);

            // Force it in front of other UI in the same canvas.
            panelRoot.transform.SetAsLastSibling();

            // If you have CanvasGroup, force it visible and clickable.
            var cg = panelRoot.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
    }
}
