using System;
using UnityEngine;

public class AttackKeywordIcon : KeywordIcon
{
    [SerializeField] private GameObject posIcon;
    [SerializeField] private GameObject negIcon;
    protected override void OnValueChange()
    {
        if (Value < 0)
        {
            posIcon.SetActive(false);
            negIcon.SetActive(true);
        }
        else
        {
            posIcon.SetActive(true);
            negIcon.SetActive(false);
        }
    }
}