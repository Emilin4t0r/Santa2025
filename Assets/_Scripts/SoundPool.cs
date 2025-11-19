using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioSource pool.
/// - Fixed-size pool (size = poolSize)
/// - PlayOneShot usage with optional spatialBlend
/// - If pool exhausted, steals oldest playing source
/// </summary>
public class SoundPool : MonoBehaviour
{
    public static SoundPool Instance { get; private set; }

    [Header("Pool")]
    [Tooltip("Number of pooled AudioSources (fixed).")]
    public int poolSize = 10;

    [Header("Optional prefab")]
    [Tooltip("Optional prefab that contains an AudioSource. If null, created GameObjects will be used.")]
    public GameObject audioSourcePrefab;

    [Header("Defaults")]
    public bool defaultSpatial = true;
    public float releasePadding = 0.05f; // small pad to make sure clip finished

    class Pooled
    {
        public GameObject go;
        public AudioSource src;
        public float startedAt;
        public float duration;
        public bool playing => src.isPlaying;
    }

    Pooled[] pool;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitPool();
    }

    void InitPool()
    {
        pool = new Pooled[Mathf.Max(1, poolSize)];
        for (int i = 0; i < pool.Length; i++)
        {
            GameObject go;
            if (audioSourcePrefab != null)
            {
                go = Instantiate(audioSourcePrefab, transform);
            }
            else
            {
                go = new GameObject("PooledAudioSrc");
                go.transform.SetParent(transform);
                go.AddComponent<AudioSource>();
            }

            var src = go.GetComponent<AudioSource>();
            src.playOnAwake = false;
            pool[i] = new Pooled { go = go, src = src, startedAt = 0f, duration = 0f };
        }
    }

    // Find free or oldest
    Pooled GetSlot()
    {
        // first try free
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].playing) return pool[i];
        }

        // none free -> steal oldest (smallest startedAt)
        int idx = 0;
        float oldest = pool[0].startedAt;
        for (int i = 1; i < pool.Length; i++)
        {
            if (pool[i].startedAt < oldest)
            {
                oldest = pool[i].startedAt;
                idx = i;
            }
        }

        // stop it and reuse
        pool[idx].src.Stop();
        return pool[idx];
    }

    /// <summary>
    /// Play a one-shot clip at world position. Returns true if started.
    /// </summary>
    public bool PlayOneShot(AudioClip clip, Vector3 position, float volume = 0.75f, float pitch = 1f, float randomPitchScale = 0.1f, bool? spatial = null)
    {
        if (clip == null) return false;
        var slot = GetSlot();
        if (slot == null) return false;

        // configure source
        slot.src.transform.position = position;
        slot.src.transform.SetParent(transform); // not attached to moving object
        slot.src.spatialBlend = spatial ?? defaultSpatial ? 1f : 0f;
        slot.src.pitch = Random.Range(pitch - randomPitchScale, pitch + randomPitchScale);
        // use PlayOneShot so we don't clobber src.clip (safer for reuse)
        slot.src.PlayOneShot(clip, volume);

        slot.startedAt = Time.time;
        slot.duration = clip.length;
        return true;
    }

    // optional: stop all currently playing (useful for e.g. scene transitions)
    public void StopAll()
    {
        for (int i = 0; i < pool.Length; i++) pool[i].src.Stop();
    }
}
