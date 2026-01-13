using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;

    [Header("Clips")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip ambienceClip;

    [Header("Master Volume (0..1)")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 0.6f;

    public float MasterVolume => masterVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Auto-grab sources if not assigned.
        if (musicSource == null || ambienceSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (musicSource == null && sources.Length > 0) musicSource = sources[0];
            if (ambienceSource == null && sources.Length > 1) ambienceSource = sources[1];
        }

        SetupSource(musicSource, musicClip);
        SetupSource(ambienceSource, ambienceClip);

        // Apply current master volume
        SetMasterVolume(masterVolume);
    }

    private void SetupSource(AudioSource src, AudioClip clip)
    {
        if (src == null) return;

        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;          // 2D
        src.ignoreListenerPause = true; // keep playing if AudioListener.pause is used
    }

    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);

        ApplyVolume(musicSource);
        ApplyVolume(ambienceSource);
    }

    private void ApplyVolume(AudioSource src)
    {
        if (src == null) return;

        src.volume = masterVolume;

        // Don't "stop" on slider change; resume if needed (unless muted).
        if (src.clip != null && masterVolume > 0f && !src.isPlaying)
            src.Play();
    }
}
