using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// 카테고리(버스)
public enum SfxBus
{
    Default,
    UI,
    Combat,
    Environment,
    Voice
}

public class SfxInfo
{
    public AudioClip clip;
    public float volume;
    public float pitch;

    public float spatialBlend; // 0=2D, 1=3D
    public float dopplerLevel;
    public float spread;
    public float minDistance;
    public float maxDistance;
    public AudioRolloffMode rolloffMode;
    public AudioMixerGroup mixerGroup;
    public SfxBus bus;

    public int maxInstances;
    public float cooldown;
    public int priority; // 낮을수록 우선순위 높음

    public SfxInfo(AudioClip clip)
    {
        this.clip = clip;
        pitch = 1f;
        spatialBlend = 0f;
        dopplerLevel = 0f;
        spread = 0f;
        minDistance = 1f;
        maxDistance = 50f;
        rolloffMode = AudioRolloffMode.Logarithmic;
        bus = SfxBus.Default;
        maxInstances = 0;
        cooldown = 0f;
        priority = 128;
        volume = 1f;
    }
}