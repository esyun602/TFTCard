using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }
    [SerializeField] private List<AudioSource> src;
    [SerializeField] private List<AudioClip> clips;
    private Dictionary<string, AudioClip> clipDict = new();

    private AudioSource currentSrc;
    private AudioSource otherSrc;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void Start()
    {
        foreach (var clip in clips)
        {
            clipDict[clip.name] = clip;
        }

        currentSrc = src[0];
        otherSrc = src[1];
        
        currentSrc.Play();
        otherSrc.Stop();
    }

    public void ChangeBgm(string name)
    {
        otherSrc.clip = clipDict.GetValueOrDefault(name);
        otherSrc.Play();
        var seq = DOTween.Sequence();
        seq.Append(currentSrc.DOFade(0, 1f));
        seq.AppendCallback(currentSrc.Stop);
        seq.Play();
        otherSrc.DOFade(1, 1f);
        (currentSrc, otherSrc) = (otherSrc, currentSrc);
    }
}