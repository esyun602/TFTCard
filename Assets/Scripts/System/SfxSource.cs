using System;
using UnityEngine;

public class SfxSource : MonoBehaviour
{
    [NonSerialized] public AudioSource src;
    [NonSerialized] public Transform followTarget;
    [NonSerialized] public Action<SfxSource> onFinished;
    [NonSerialized] public SfxInfo Info;

    float _remain;
    bool _active;

    void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
    }

    public void Play(SfxInfo c, Vector3 pos, bool follow)
    {
        Info = c;
        followTarget = follow ? followTarget : null; // 호출 측에서 먼저 세팅

        var clip = c.clip;
        if (!clip)
        {
            Release();
            return;
        }

        transform.position = pos;
        src.clip = clip;
        src.volume = c.volume;
        src.pitch = c.pitch;
        src.spatialBlend = c.spatialBlend;
        src.dopplerLevel = c.dopplerLevel;
        src.spread = c.spread;
        src.minDistance = c.minDistance;
        src.maxDistance = c.maxDistance;
        src.rolloffMode = c.rolloffMode;
        src.outputAudioMixerGroup = c.mixerGroup;
        src.priority = c.priority;

        src.Stop();
        src.Play();
        _remain = clip.length / Mathf.Max(0.001f, src.pitch);
        _active = true;
    }

    void Update()
    {
        if (!_active) return;
        if (followTarget) transform.position = followTarget.position;

        _remain -= Time.unscaledDeltaTime;
        if (_remain <= 0f || !src.isPlaying)
        {
            Release();
        }
    }

    public void StopImmediate()
    {
        src.Stop();
        Release();
    }

    void Release()
    {
        _active = false;
        followTarget = null;
        Info = null;
        onFinished?.Invoke(this);
    }
}