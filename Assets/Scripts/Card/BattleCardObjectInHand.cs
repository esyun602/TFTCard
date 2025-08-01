using System;
using System.Collections.Generic;
using MessageSystem;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Exception = System.Exception;

[Flags]
public enum InputBlockFlag
{
	None = 0,
	Hover = 1 << 0,
	Select = 1 << 1,
	All = Hover | Select
}

//todo: transform cache?
public abstract class BattleCardObjectInHand : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IMessageReceiver
{
	//todo: fix how?
	protected abstract ICard TargetCard { get; }
	protected SimpleStateMachine cardObjectStateMachine = new();
	private new BoxCollider collider;

	private Vector3 handTargetPos;
	private Quaternion handTargetRotation;
	protected Vector3 hoverTargetPos;

	protected InputBlockFlag blockInput;

	//todo: 스탯이 없는 카드
	public abstract IStat Stat { get; }

	protected virtual bool CanSelect()
	{
		//todo: access fix?
		if (Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy < Stat.GetValuesByValueType(BattleValueType.Cost)[0])
		{
			return false;
		}
		
		return true;
	}

	protected abstract bool CanUse(ITile tile = null);

	//todo: 풀링으로 수정
	public void Activate()
	{
		if (collider == null)
		{
			collider = GetComponentInChildren<BoxCollider>();
		}

		gameObject.SetActive(true);
		transform.forward = Camera.main.transform.forward;
		ChangeState(new CardObjectNormalInHandState(this));
		GetComponentInChildren<ICardInfoHandler>().Initialize(TargetCard.CardStaticSpec, Stat);
		OnActivate();
	}

	protected virtual void OnActivate()
	{
		
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);

		ChangeState(null);
		OnDeactivate();
	}

	protected virtual void OnDeactivate()
	{
		
	}

	public void UpdateBlockInput(InputBlockFlag flag)
	{
		blockInput = flag;
		if ((flag & InputBlockFlag.Hover) != InputBlockFlag.None &&
		    cardObjectStateMachine.CurrentState is CardObjectNormalInHandState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}

	protected void ChangeState(IState nextState)
	{
		cardObjectStateMachine.ChangeState(nextState);
	}

	private void Update()
	{
		cardObjectStateMachine.UpdateFrame(Time.deltaTime);
	}

	//todo: field와 같은 리소스 쓰는게 확정되면 리소스 재활용 추가

	public void Dispose()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnPointerClickImpl(eventData);
	}

	protected abstract void OnPointerClickImpl(PointerEventData eventData);

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInHandState normalState)
		{
			normalState.SetHover();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInHandState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}

	public void CatchMessage(Message m)
	{
		if (m is CardHandPosUpdatedNotice notice)
		{
			handTargetPos = notice.TargetPos;
			handTargetRotation = notice.TargetRotation;
			hoverTargetPos = notice.HoverTargetPos;
			NoticeSystem.Instance.SendSync(m, cardObjectStateMachine);
		}
	}

	protected class CardObjectNormalInHandState : IState, IUpdatable, IMessageReceiver
	{
		public bool IsMoving => owner.handTargetPos != owner.transform.position;
		private BattleCardObjectInHand owner;

		//일단 시간으로
		private const float returnTime = 0.5f;
		private float timePassed = 0f;
		private Vector3 startPos;
		private Quaternion startRotation;
		private AnimationCurve returnAnimationCurve;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Quaternion? targetRotationOverride;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;

		public CardObjectNormalInHandState(BattleCardObjectInHand owner)
		{
			this.owner = owner;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			isHovered = true;
			hoverTarget = originalScale * 1.8f;
			owner.collider.size = GameDataSystem.Instance.GetGameData<Constant>().HandHoverColliderSize;
			targetRotationOverride = Camera.main.transform.localRotation;
			RestartHover();
			//todo: 애니메이션 빼면 순간적으로 마우스 탈출하는 문제
			Restart();
		}

		public void RemoveHover()
		{
			isHovered = false;
			hoverTarget = originalScale;
			targetRotationOverride = null;
			owner.collider.size = GameDataSystem.Instance.GetGameData<Constant>().HandColliderSize;
			RestartHover();
		}

		public void Enter(IState prevState)
		{
			Restart();
			RestartHover();
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
		}

		public void UpdateFrame(float dt)
		{
			UpdateAlignment(dt);
			UpdateScale(dt);
		}

		private void UpdateAlignment(float dt)
		{
			timePassed += dt;
			var progress = returnAnimationCurve.Evaluate(timePassed / returnTime);
			owner.transform.position =
				Vector3.Lerp(startPos, isHovered ? owner.hoverTargetPos : owner.handTargetPos, progress);
			owner.transform.localRotation = Quaternion.Lerp(startRotation,
				targetRotationOverride ?? owner.handTargetRotation, progress);
		}

		private void UpdateScale(float dt)
		{
			hoverTimePassed += dt;
			var progress = returnAnimationCurve.Evaluate(hoverTimePassed / hoverTime);
			owner.transform.localScale = Vector3.Lerp(startScale, hoverTarget, progress);
		}

		public void CatchMessage(Message m)
		{
			if (m is CardHandPosUpdatedNotice)
			{
				Restart();
			}
		}

		private void Restart()
		{
			timePassed = 0f;
			startRotation = owner.transform.localRotation;
			startPos = owner.transform.position;
		}

		private void RestartHover()
		{
			hoverTimePassed = 0f;
			startScale = owner.transform.localScale;
		}
	}
}