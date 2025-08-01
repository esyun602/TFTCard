using System;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//todo: 네이밍 수정 및 이동 관련 분리
public class SkillCardInHand : BattleCardObjectInHand
{
	public bool IsTargeting => targetCard.SkillCardStaticSpec.cardUseType == UseType.Targeting;
	private const string cardPrefabPath = "Card/SkillCardPrefab";
	private SkillCard targetCard;
	private SkillCardBattleStat battleStat;
	protected override ICard TargetCard => targetCard;
	public override IStat Stat => battleStat;

	public static SkillCardInHand Instantiate(SkillCard targetSkillCard, SkillCardBattleStat skillCardStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath)).AddComponent<SkillCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.targetCard = targetSkillCard;
		cardObject.battleStat = skillCardStat;
		cardObject.targetCard.Action.SetCardBattleStat(skillCardStat);

		return cardObject;
	}

	protected override bool CanSelect()
	{
		return base.CanSelect() && (targetCard.Action is not UnitSkillCardActionBase || battleStat.Owner != null);
	}

	public bool IsInstanceOf(SkillCard target)
	{
		return target == targetCard;
	}

	public void SetOwner(IBattleObject bo)
	{
		battleStat.Owner = bo;
	}

	private void OnUseComplete()
	{
		if (battleStat.IsExhaustion)
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.RemoveCard(this);
					
		}
		else
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCard(this);
		}
	}

	protected override bool CanUse(ITile tile = null)
	{
		if (tile == null)
		{
			return false;
		}

		return targetCard.Action.CanUse(tile)
		       && cardObjectStateMachine.CurrentState is TargetingSkillCardSelectedInHandState;
	}

	protected override void OnPointerClickImpl(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None || !CanSelect()) return;
		if (cardObjectStateMachine.CurrentState is not CardObjectNormalInHandState { IsHovered: true } ||
		    eventData.button != PointerEventData.InputButton.Left) return;

		NoticeSystem.Instance.PublishSync(new SkillHandCardSelectNotice(this));

		if (IsTargeting)
		{
			ChangeState(new TargetingSkillCardSelectedInHandState(this));
		}
		else
		{
			ChangeState(new GlobalSkillCardSelectedInHandState(this));
		}
	}

	private class TargetingSkillCardSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private SkillCardInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public TargetingSkillCardSelectedInHandState(SkillCardInHand owner)
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
			owner.transform.localScale = Vector3.one * 1.8f;
			owner.transform.position = owner.hoverTargetPos;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
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
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.battleStat.CostValue;
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
			NoticeSystem.Instance.PublishSync(new SkillHandCardSelectCancelNotice(owner));
			owner.ChangeState(new CardObjectNormalInHandState(owner));
		}

		public void UpdateFrame(float dt)
		{
			//todo: optimize and fix - new input mouse pos not working
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos).GetX0z(Constant.SelectYPos);

			currentTile = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetTileAt(mousePos);
			targetPos = owner.CanUse(currentTile) ? currentTile.GetPosition().GetX0z(Constant.SelectYPos) : mousePos;
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
		private SkillCardInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;

		public GlobalSkillCardSelectedInHandState(SkillCardInHand owner)
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
			owner.transform.localScale = Vector3.one * 1.8f;
			owner.transform.position = owner.hoverTargetPos;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			owner.transform.position = targetPos.GetX0z(Constant.SelectYPos);
		}

		private void OnTryUseHandCard(InputAction.CallbackContext obj)
		{
			if (!owner.CanSelect())
			{
				//todo: 이런 상황이 발생하면 안되는데
				throw new Exception();
			}

			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.battleStat.CostValue;
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
			NoticeSystem.Instance.PublishSync(new SkillHandCardSelectCancelNotice(owner));
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
		private SkillCardInHand owner;
		private IBattleObject targetObject;
		private float timePassed;
		private Action currentUpdateAction;

		public TargetingCardObjectUsedInHandState(SkillCardInHand owner, ITile targetTile)
		{
			var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
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
				owner.targetCard.Action.Trigger(new TargetingActionTriggerInfo()
				{
					Target = targetObject
				});
				currentUpdateAction = UpdateAction;
			}
		}

		private void UpdateAction()
		{
			owner.targetCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
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
		private SkillCardInHand owner;
		private IBattleObject targetObject;
		private float timePassed;
		private Action currentUpdateAction;

		public GlobalCardObjectUsedInHandState(SkillCardInHand owner)
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
				owner.targetCard.Action.Trigger();
				currentUpdateAction = UpdateAction;
			}
		}

		private void UpdateAction()
		{
			owner.targetCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
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