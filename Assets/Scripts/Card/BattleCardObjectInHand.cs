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
	TurnEnd = 1 << 2,
	All = Hover | Select | TurnEnd
}

//todo: transform cache?
public abstract class BattleCardObjectInHand : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IMessageReceiver
{
	public bool IsTargeting => TargetCard.SkillCardStaticSpec.CardUseType == UseType.Targeting;
	//todo: fix how?
	public abstract SkillCardBase TargetCard { get; }
	protected SimpleStateMachine cardObjectStateMachine = new();
	private new BoxCollider collider;

	private Vector3 handTargetPos;
	private Quaternion handTargetRotation;
	protected Vector3 hoverTargetPos;

	protected InputBlockFlag blockInput;
	
	public abstract ObjectType CardType { get; }

	//todo: 스탯이 없는 카드
	public abstract IStat Stat { get; }

	protected virtual bool CanSelect()
	{
		//todo: access fix?
		if (Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -
		    Game.Instance.GetPlayer().CurrentPlayInfo.MinEnergy < Stat.GetValueByValueType(SkillValueType.Cost))
		{
			return false;
		}
		
		return true;
	}

	protected virtual bool CanUse(ITile tile = null)
	{
		if (tile == null)
		{
			return false;
		}

		return TargetCard.Action.CanUse(tile)
		       && cardObjectStateMachine.CurrentState is TargetingSkillCardSelectedInHandState;
	}

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
		GetComponentInChildren<ICardInfoHandler>().Initialize(TargetCard, Stat, CanSelect);
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
		GetComponentInChildren<ICardInfoHandler>().Dispose();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None || !CanSelect()) return;
		if (cardObjectStateMachine.CurrentState is not CardObjectNormalInHandState { IsHovered: true } ||
		    eventData.button != PointerEventData.InputButton.Left) return;

		if (IsTargeting)
		{
			ChangeState(new TargetingSkillCardSelectedInHandState(this));
		}
		else
		{
			ChangeState(new GlobalSkillCardSelectedInHandState(this));
		}
	}


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
	
	private void OnUseComplete()
	{
		if (TargetCard.Stat.GetValueByValueType(SkillValueType.Exhaustion) != 0)
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.RemoveCard(this);
					
		}
		else
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCard(this);
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
			NoticeSystem.Instance.Publish(new SkillHandCardHoverNotice(owner));
			isHovered = true;
			hoverTarget = originalScale * 1.1f;
			owner.collider.size = Constant.HandHoverColliderSize;
			targetRotationOverride = Camera.main.transform.localRotation;
			RestartHover();
			//todo: 애니메이션 빼면 순간적으로 마우스 탈출하는 문제
			Restart();
		}

		public void RemoveHover()
		{
			NoticeSystem.Instance.Publish(new SkillHandCardRemoveHoverNotice(owner));
			isHovered = false;
			hoverTarget = originalScale;
			targetRotationOverride = null;
			owner.collider.size = Constant.HandColliderSize;
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
	
	private class TargetingSkillCardSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private BattleCardObjectInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;
		private TargetingActionTriggerInfo currentTriggerInfo;

		public TargetingSkillCardSelectedInHandState(BattleCardObjectInHand owner)
		{
			this.owner = owner;
		}

		public void Enter(IState prevState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Enable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Enable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed += OnTryUseHandCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed += OnCancelHandCard;
			owner.transform.up = Camera.main.transform.up;
			//todo: fix
			owner.transform.localScale = Vector3.one * 1.1f;
			owner.transform.position = owner.hoverTargetPos;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
		
			NoticeSystem.Instance.Publish(new SkillHandCardSelectNotice(owner));
			//owner.transform.position = targetPos.GetX0z(Constant.SelectYPos);;
		}

		private void OnTryUseHandCard(InputAction.CallbackContext obj)
		{
			if (!owner.CanSelect())
			{
				//todo: 이런 상황이 발생하면 안되는데
				throw new Exception();
			}

			//todo: 타일이 필요 없는 카드
			if (owner.CanUse(currentTile))
			{
				//todo: 사용함수를 분리?
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.Stat.GetValueByValueType(SkillValueType.Cost);
				owner.ChangeState(new TargetingCardObjectUsedInHandState(owner, currentTile));
			}
		}

		public void Exit(IState nextState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Disable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Disable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed -= OnTryUseHandCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed -= OnCancelHandCard;
		}

		private void OnCancelHandCard(InputAction.CallbackContext obj)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardSelectCancelNotice(owner));
			owner.ChangeState(new CardObjectNormalInHandState(owner));
			owner.TargetCard.Action.SetTriggerParam(null);
		}

		public void UpdateFrame(float dt)
		{
			//todo: optimize and fix - new input mouse pos not working
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos).GetX0z(Constant.SelectYPos);

			var prevTile = currentTile;
			currentTile = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map.GetTileAt(mousePos);
			if (owner.CanUse(currentTile))
			{
				if (currentTriggerInfo == null)
				{
					currentTriggerInfo = new TargetingActionTriggerInfo()
					{
						Target = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map
							.GetBattleObjectOfTile(currentTile)
					};
					owner.TargetCard.Action.SetTriggerParam(currentTriggerInfo);
					NoticeSystem.Instance.Publish(new TargetingCardAimedNotice(owner));
				}
				targetPos = currentTile.GetPosition().GetX0z(Constant.SelectYPos);
			}
			else
			{
				if (currentTriggerInfo != null)
				{
					currentTriggerInfo = null;
					owner.TargetCard.Action.SetTriggerParam(null);
					NoticeSystem.Instance.Publish(new TargetingCardAimRemovedNotice(owner));
				}
				targetPos = mousePos;
			}
			NoticeSystem.Instance.Publish(
				new SkillHandCardTargetingUpdateNotice(Camera.main.WorldToScreenPoint(targetPos)));

			if (Vector3.Distance(targetPos, owner.transform.position) < 0.01f)
			{
				timePassed = 0f;
			}

			timePassed += dt;

			var realSpeed = followAnimationCurve.Evaluate(timePassed) * followSpeed;
			var totalTime = Vector3.Distance(targetPos, owner.transform.position) / realSpeed;
			//owner.transform.position = Vector3.Lerp(owner.transform.position, targetPos, dt / totalTime);
			/*owner.transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(
					                                Vector3.Distance(targetPos, owner.transform.position) * 50f *
					                                (targetPos.x > owner.transform.position.x ? -1f : 1f), -45f, 45f),
				                                Vector3.Cross(Camera.main.transform.forward,
					                                (targetPos - owner.transform.position).normalized)) *
			                                Camera.main.transform.localRotation;*/
		}
	}

	private class GlobalSkillCardSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private BattleCardObjectInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;

		public GlobalSkillCardSelectedInHandState(BattleCardObjectInHand owner)
		{
			this.owner = owner;
		}

		public void Enter(IState prevState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Enable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Enable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed += OnTryUseHandCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed += OnCancelHandCard;
			owner.transform.up = Camera.main.transform.up;
			//todo: fix
			owner.transform.localScale = Vector3.one * 1.1f;
			owner.transform.position = owner.hoverTargetPos;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			owner.transform.position = targetPos.GetX0z(Constant.SelectYPos);
			NoticeSystem.Instance.Publish(new SkillHandCardSelectNotice(owner));
		}

		private void OnTryUseHandCard(InputAction.CallbackContext obj)
		{
			if (!owner.CanSelect())
			{
				//todo: 이런 상황이 발생하면 안되는데
				throw new Exception();
			}

			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.Stat.GetValueByValueType(SkillValueType.Cost);
			owner.ChangeState(new GlobalCardObjectUsedInHandState(owner));
		}

		public void Exit(IState nextState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Disable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Disable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed -= OnTryUseHandCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed -= OnCancelHandCard;
		}

		private void OnCancelHandCard(InputAction.CallbackContext obj)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardSelectCancelNotice(owner));
			owner.ChangeState(new CardObjectNormalInHandState(owner));
		}

		public void UpdateFrame(float dt)
		{
			//todo: optimize and fix - new input mouse pos not working
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos).GetX0z(Constant.SelectYPos);

			targetPos = mousePos;

			if (Vector3.Distance(targetPos, owner.transform.position) < 0.01f)
			{
				timePassed = 0f;
			}

			timePassed += dt;

			var realSpeed = followAnimationCurve.Evaluate(timePassed) * followSpeed;
			var totalTime = Vector3.Distance(targetPos, owner.transform.position) / realSpeed;
			owner.transform.position = Vector3.Lerp(owner.transform.position, targetPos, dt / totalTime);
			owner.transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(
					                                Vector3.Distance(targetPos, owner.transform.position) * 50f *
					                                (targetPos.x > owner.transform.position.x ? -1f : 1f), -45f, 45f),
				                                Vector3.Cross(Camera.main.transform.forward,
					                                (targetPos - owner.transform.position).normalized)) *
			                                Camera.main.transform.localRotation;
		}
	}

	private class TargetingCardObjectUsedInHandState : IState, IUpdatable
	{
		private BattleCardObjectInHand owner;
		private IBattleObject targetObject;
		private float timePassed;
		private Action currentUpdateAction;

		public TargetingCardObjectUsedInHandState(BattleCardObjectInHand owner, ITile targetTile)
		{
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
			this.owner = owner;
			this.targetObject = map.GetBattleObjectOfTile(targetTile);
		}

		public void Enter(IState prevState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardStartUseNotice(owner));
			currentUpdateAction = UpdatePreAction;
			timePassed = 0f;
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardEndUseNotice(owner));
		}

		private void UpdatePreAction()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0f)
			{
				timePassed = 0f;
				owner.TargetCard.Action.Trigger();
				currentUpdateAction = UpdateAction;
			}
		}

		private void UpdateAction()
		{
			owner.TargetCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
			if (routineDone)
			{
				currentUpdateAction = UpdateEndAction;
			}
		}

		private void UpdateEndAction()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0f)
			{
				timePassed = 0f;
				currentUpdateAction = null;
			}
		}


		public void UpdateFrame(float dt)
		{
			currentUpdateAction?.Invoke();
			if (currentUpdateAction == null)
			{
				owner.OnUseComplete();
			}
		}
	}
	
	private class GlobalCardObjectUsedInHandState : IState, IUpdatable
	{
		private BattleCardObjectInHand owner;
		private IBattleObject targetObject;
		private float timePassed;
		private Action currentUpdateAction;

		public GlobalCardObjectUsedInHandState(BattleCardObjectInHand owner)
		{
			this.owner = owner;
		}

		public void Enter(IState prevState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardStartUseNotice(owner));
			currentUpdateAction = UpdatePreAction;
			timePassed = 0f;
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardEndUseNotice(owner));
		}

		private void UpdatePreAction()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0f)
			{
				timePassed = 0f;
				owner.TargetCard.Action.Trigger();
				currentUpdateAction = UpdateAction;
			}
		}

		private void UpdateAction()
		{
			owner.TargetCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
			if (routineDone)
			{
				currentUpdateAction = UpdateEndAction;
			}
		}

		private void UpdateEndAction()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0f)
			{
				timePassed = 0f;
				currentUpdateAction = null;
			}
		}


		public void UpdateFrame(float dt)
		{
			currentUpdateAction?.Invoke();
			if (currentUpdateAction == null)
			{
				owner.OnUseComplete();
			}
		}
	}
}