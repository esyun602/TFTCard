using System;
using DG.Tweening;
using MessageSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleUICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private SimpleStateMachine stateMachine;
	private ICardInfoHandler infoHandler;
	private BattleCardObjectInHand targetCard;
	private void Awake()
	{
		infoHandler = GetComponentInChildren<ICardInfoHandler>();
		stateMachine = new SimpleStateMachine();
	}
	
	public void Initialize(BattleCardObjectInHand targetCard)
	{
		this.targetCard = targetCard;
		stateMachine.ChangeState(new BattleUICardNormalState(this));
		
		InitializeInfo();
	}

	private void InitializeInfo()
	{
		infoHandler.Initialize(targetCard.TargetCard, targetCard.Stat);
	}

	public void OnPointerEnter(PointerEventData eventData)
    {
	    if (stateMachine.CurrentState is BattleUICardNormalState normalState)
	    {
		    normalState.SetHover();
	    }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
	    if (stateMachine.CurrentState is BattleUICardNormalState { IsHovered: true } normalState)
	    {
		    normalState.RemoveHover();
	    }
    }

    private void Update()
    {
        (stateMachine as IUpdatable)?.UpdateFrame(Time.deltaTime);
    }

	protected class BattleUICardNormalState : IState, IUpdatable
	{
		private BattleUICard owner;

		private AnimationCurve returnAnimationCurve;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;

		public BattleUICardNormalState(BattleUICard owner)
		{
			this.owner = owner;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			//todo: fix
			owner.transform.parent.SetAsLastSibling();
			owner.transform.SetAsLastSibling();
			isHovered = true;
			hoverTarget = originalScale * 1.8f;
			RestartHover();
		}

		public void RemoveHover()
		{
			isHovered = false;
			hoverTarget = originalScale;
			RestartHover();
		}

		public void Enter(IState prevState)
		{
			RestartHover();
			//todo: 임시
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
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