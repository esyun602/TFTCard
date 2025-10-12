using System;
using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraftUIUnitCard : DraftUICard
{
    public override ICard TargetCard => unitCard;
    private UnitCard unitCard;
    [SerializeField] private UISkillCardInfoHandler unitActInfoHandler;

    private float originPosX;
    
    public override void OnInitialize(ICardSpec targetCard)
    {
        originPosX = transform.position.x;
        unitCard = new UnitCard((UnitCardSpec)targetCard);
        SetInfo();
    }

    protected override void OnPointerEnterImpl()
    {
        transform.DOKill();
        //todo: fix 

        transform.DOMoveX(Screen.width / 2f - 200f, 0.2f);
        
        unitActInfoHandler.transform.parent.gameObject.SetActive(true);
    }

    protected override void OnPointerExitImpl()
    {
        transform.DOKill();
        transform.DOMoveX(originPosX, 0.2f);
        unitActInfoHandler.transform.parent.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        unitActInfoHandler.transform.parent.gameObject.SetActive(false);
    }

    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, unitCard.Stat);
        //todo: fix, 생성으로 변경 및 공용 ui 카드 오브젝트 만들고 addobject로 수정
        //일단은 하나만
        unitActInfoHandler.Initialize(unitCard.UnitSkillCard[0], unitCard.UnitSkillCard[0].Stat, null);
    }
}