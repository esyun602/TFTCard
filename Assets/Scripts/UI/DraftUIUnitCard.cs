using System;
using System.Collections.Generic;
using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DraftUIUnitCard : DraftUICard
{
    public override ICard TargetCard => unitCard;
    private UnitCard unitCard;
    [SerializeField] private UISkillCardInfoHandler unitActInfoHandler;
    [SerializeField] private Transform synergyDescUIPoolTr;
    private SynergyDescUI[] synergyDescUIPool;
    [SerializeField] private Transform usingSynergyDescUITr;
    private List<SynergyDescUI> usingSynergyDescUIList = new();
	
    protected DraftSelectPanel selectPanel;
    private float originPosX;
    private float originPosY;

    public void SetSelectPanel(DraftSelectPanel selectPanel)
    {
	    this.selectPanel = selectPanel;
    }
    
    public override void OnInitialize(ICardSpec targetCard)
    {
	    foreach (var ui in usingSynergyDescUIList)
	    {
		    ui.transform.SetParent(synergyDescUIPoolTr);
	    }
	    usingSynergyDescUIList.Clear();
	    synergyDescUIPool = synergyDescUIPoolTr.GetComponentsInChildren<SynergyDescUI>(true);
	    
        originPosX = transform.position.x;
        originPosY = transform.position.y;
        unitCard = new UnitCard((UnitCardSpec)targetCard);
        transform.position += Vector3.right * (Screen.width / 2 + 400f);

        var seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(originPosX, 0.8f));
        seq.Insert((originPosX / Screen.width) * 0.2f, transform.DOMoveY(originPosY + 50f, 0.08f).SetLoops(8, LoopType.Yoyo));
        seq.SetTarget(transform);
        seq.Play();
        SetInfo();
    }

    private void SetHighlight(bool active)
    {
	    foreach (var ui in usingSynergyDescUIList)
	    {
		    ui.gameObject.SetActive(active);
	    }
	    
	    unitActInfoHandler.transform.parent.gameObject.SetActive(active);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
	    if (CurrentState is not DraftUIUnitCardHighlightState)
	    {
		    ChangeState(new DraftUIUnitCardHighlightState(this));
	    }
    }
	
    public override void OnPointerEnter(PointerEventData eventData)
    {
	    if (CurrentState is not DraftUIUnitCardHighlightState)
	    {
		    base.OnPointerEnter(eventData);
	    }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
	    if (CurrentState is not DraftUIUnitCardHighlightState)
	    {
		    base.OnPointerExit(eventData);
		    SetHighlight(false);
	    }
    }

    private void OnDisable()
    {
	    SetHighlight(false);
    }

    private void SetInfo()
    {
        infoHandler.Initialize(TargetCard, unitCard.Stat);
        //todo: fix, 생성으로 변경 및 공용 ui 카드 오브젝트 만들고 addobject로 수정
        //일단은 하나만
        unitActInfoHandler.Initialize(unitCard.UnitSkillCard[0], unitCard.UnitSkillCard[0].Stat, null);
        
        for (var i = 0; i < unitCard.Stat.synergyList.Count; i++)
        {
	        var synergy = unitCard.Stat.synergyList[i];
	        SetUse(synergyDescUIPool[i], synergy);
        }
    }

    private void SetUse(SynergyDescUI ui, SynergyCategory synergy)
    {
	    usingSynergyDescUIList.Add(ui);
	    ui.transform.SetParent(usingSynergyDescUITr);
	    ui.Initialize(synergy);
    }

    private class DraftUIUnitCardHighlightState : IState, IUpdatable
    {
		private DraftUIUnitCard owner;
		private AnimationCurve returnAnimationCurve;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;

		public DraftUIUnitCardHighlightState(DraftUIUnitCard owner)
		{
			this.owner = owner;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			isHovered = true;
			hoverTarget = originalScale * 1f;
			owner.tint.DOKill();
			owner.tint.DOFade(0, 0.2f);
			RestartHover();
		}

		public void RemoveHover()
		{
			isHovered = false;
			hoverTarget = originalScale;
			owner.tint.DOKill();
			owner.tint.DOFade(0.5f, 0.2f);
			RestartHover();
		}

		public void Enter(IState prevState)
		{
			SetHover();
			RestartHover();
			owner.transform.DOKill();
			owner.transform.position = owner.transform.position.GetX0z(owner.originPosY);
			//todo: fix 

			owner.selectPanel.Activate(owner);
			owner.transform.DOMoveX(Screen.width / 2f - 200f, 0.2f);
			owner.SetHighlight(true);
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
			owner.transform.DOKill();
			owner.transform.DOMoveX(owner.originPosX, 0.2f);
			owner.SetHighlight(false);
		}

		public void UpdateFrame(float dt)
		{
			UpdateScale(dt);
		}

		private void UpdateScale(float dt)
		{
			hoverTimePassed += dt;
			var progress = returnAnimationCurve.Evaluate(hoverTimePassed / hoverTime);
			owner.transform.localScale = Vector3.Lerp(startScale, hoverTarget, progress);
		}

		private void RestartHover()
		{
			hoverTimePassed = 0f;
			startScale = owner.transform.localScale;
		}
    }
}