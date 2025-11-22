using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> src;
    [SerializeField] private List<AudioClip> clips;
    private Dictionary<string, AudioClip> clipDict = new();

    private AudioSource currentSrc;
    private AudioSource otherSrc;
    
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
        otherSrc.clip = clipDict[name];
        otherSrc.Play();
        var seq = DOTween.Sequence();
        seq.Append(otherSrc.DOFade(0, 1f));
        seq.AppendCallback(otherSrc.Stop);
        seq.Play();
        currentSrc.DOFade(0, 1f);
        (currentSrc, otherSrc) = (otherSrc, currentSrc);
    }
}