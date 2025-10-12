using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public class EnemyGauge : UIInstance
{
    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform end;
    [SerializeField]
    private RectTransform fill;

    private float StartX => start.position.x;
    private float EndX => end.position.x;

    private int currentDivideCount;
    private int currentFill;

    private List<PooledUnityObject> poList;
    private List<EnemyCardIcon> iconList;
    
    public override UIType UIType => UIType.SceneUI;

    private List<EnemySkillCardObject> cardList;
    
    protected override void Init(object param)
    {
        poList = new();
        iconList = new();
        var divider = UnityObjectPool.GetOrCreateUIPool("EnemyGaugeDivider");
        divider.transform.SetParent(transform);
        var cardIconPool = UnityObjectPool.GetOrCreateUIPool("CardIcon");
        cardIconPool.transform.SetParent(transform);
        divider.transform.SetAsLastSibling();
        cardIconPool.transform.SetAsLastSibling();
    }

    public void InitializeBar(int divideCount, List<int> idxArray, List<EnemySkillCardObject> cardObjectInHands)
    {
        DisposeBar();
        currentDivideCount = divideCount;
        for (var i = 1; i < divideCount; i++)
        {
            poList.Add(UnityObjectPool.GetOrCreateUIPool("EnemyGaugeDivider").Instantiate(new Vector3(GetPosX(i), transform.position.y, 0)));
        }

        for(var i = 0; i < cardObjectInHands.Count; i++)
        {
            var idx = idxArray[i];
            
            var po = UnityObjectPool.GetOrCreateUIPool("CardIcon")
                .Instantiate(new Vector3(GetPosX(idx), transform.position.y, 0));
            poList.Add(po);
            var icon = po.GetComponent<EnemyCardIcon>();
            iconList.Add(icon);
            icon.CardObject = cardObjectInHands[i];
            icon.SetUse(idx <= currentFill, true);
            if (cardObjectInHands[i].IsDead)
            {
                icon.gameObject.SetActive(false);
            }
        }
        
        
        fill.offsetMax = GetFillOffsetMax(currentFill);
        cardList = cardObjectInHands;
    }

    public void SetFill(int idx)
    {
        currentFill = idx;
        DOTween.To(() => fill.offsetMax, x => fill.offsetMax = x, endValue: GetFillOffsetMax(idx), 0.25f)
            .SetTarget(fill);
        
        //todo: fix?
        if (idx == 0 && idx < currentFill)
        {
            foreach (var icon in iconList)
            {
                icon.gameObject.SetActive(false);
            }
        }
    }

    public void SetCardDisable(IBattleObject bo)
    {
        var cards = cardList.Select((x, idx) => (x, idx)).Where(x => x.x.Stat.Owner == bo);
        foreach (var (_, idx) in cards)
        {
            iconList[idx].gameObject.SetActive(false);
        }
    }

    public void SetCardUse(IAction action)
    {
        var card = cardList.FindLast(x => x.TargetCard.Action == action);
        if (card == null) return;
        iconList[cardList.IndexOf(card)].SetUse(true);
    }

    public void DisposeBar()
    {
        foreach (var po in poList)
        {
            po.Dispose();
        }
        poList.Clear();
        iconList.Clear();
    }

    private float GetPosX(int idx)
    {
       var t = (((float)idx) / currentDivideCount);
       return Mathf.Lerp(StartX, EndX, t);
    }

    private Vector2 GetFillOffsetMax(int idx)
    {
        return new Vector2(GetPosX(idx) - EndX, fill.offsetMax.y);
    }
}