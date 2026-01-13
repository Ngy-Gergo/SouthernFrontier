using UnityEngine;
using UnityEngine.EventSystems;

public static class UIBlock
{
    public static bool PointerIsOverUI()
    {
        if (EventSystem.current == null) return false;

        // Touch support.
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        // Mouse support.
        return EventSystem.current.IsPointerOverGameObject();
    }
}
