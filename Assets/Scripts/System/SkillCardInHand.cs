using System;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//todo: 우선 타게팅만
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
		cardObject.targetCard.Action.SetCardStat(skillCardStat);

		return cardObject;
	}


	protected override bool CanUse(ITile tile = null)
	{
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		if (tile == null)
		{
			return false;
		}
		
		var bo = map.GetBattleObjectOfTile(tile);
		return bo != null
		       && cardObjectStateMachine.CurrentState is SkillCardSelectedInHandState;
	}
	
	protected override void OnPointerClickImpl(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None || !CanSelect()) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInHandState { IsHovered: true } &&
		    eventData.button == PointerEventData.InputButton.Left)
		{
			NoticeSystem.Instance.PublishSync(new SkillHandCardSelectNotice(this));
			ChangeState(new SkillCardSelectedInHandState(this));
		}
	}
	
	
	/// <summary>
	/// 마우스 포인터를 따라가는 상태
	/// </summary>
	private class SkillCardSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private SkillCardInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public SkillCardSelectedInHandState(SkillCardInHand owner)
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
				owner.ChangeState(new CardObjectUsedInHandState(owner, currentTile));
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
			NoticeSystem.Instance.Publish(new SkillHandCardTargetingUpdateNotice(Camera.main.WorldToScreenPoint(targetPos)));
			
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
	
	//todo: run action
	private class CardObjectUsedInHandState : IState, IUpdatable
	{
		private SkillCardInHand owner;
		private IBattleObject targetObject;
		private float timePassed;
		private Action currentUpdateAction;
		
		public CardObjectUsedInHandState(SkillCardInHand owner, ITile targetTile)
		{
			var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
			this.owner = owner;
			this.targetObject = map.GetBattleObjectOfTile(targetTile);;
			
		}

		public void Enter(IState prevState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardStartUseNotice(owner));
			currentUpdateAction = UpdatePreAttack;
			timePassed = 0f;
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.Publish(new SkillHandCardEndUseNotice(owner));
		}
		
		private void UpdatePreAttack()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0f)
			{
				timePassed = 0f;
				owner.targetCard.Action.Trigger(new TargetingActionTriggerInfo()
				{
					Target = targetObject 
				});
				currentUpdateAction = UpdateAttack;
			}
		}

		private void UpdateAttack()
		{
			owner.targetCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
			if (routineDone)
			{
				currentUpdateAction = UpdateEndAttack;
			}
		}

		private void UpdateEndAttack()
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
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCard(owner);
				owner.Deactivate();
			}
		}
	}
}