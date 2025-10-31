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

    protected DraftSelectPanel selectPanel;
    private float originPosX;

    public void SetSelectPanel(DraftSelectPanel selectPanel)
    {
	    this.selectPanel = selectPanel;
    }
    
    public override void OnInitialize(ICardSpec targetCard)
    {
        originPosX = transform.position.x;
        unitCard = new UnitCard((UnitCardSpec)targetCard);
        SetInfo();
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
	    }
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
			hoverTarget = originalScale * 1.3f;
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
			//todo: fix 

			owner.selectPanel.Activate(owner);
			owner.transform.DOMoveX(Screen.width / 2f - 200f, 0.2f);
			owner.unitActInfoHandler.transform.parent.gameObject.SetActive(true);
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
			owner.transform.DOKill();
			owner.transform.DOMoveX(owner.originPosX, 0.2f);
			owner.unitActInfoHandler.transform.parent.gameObject.SetActive(false);
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