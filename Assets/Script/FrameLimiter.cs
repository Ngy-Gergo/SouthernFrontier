using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    [SerializeField] private int targetFps = 60;

    private void Awake()
    {
        // VSync caps to monitor refresh (usually 60/120/144).
        QualitySettings.vSyncCount = 1;

        // If you prefer manual cap, set vSyncCount = 0 and use targetFrameRate.
        Application.targetFrameRate = targetFps;
    }
}
