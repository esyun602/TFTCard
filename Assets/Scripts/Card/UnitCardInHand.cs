using System;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCardInHand : BattleCardObjectInHand
{
	private string cardPrefabPath;
	private UnitCard targetCard;
	private UnitCardBattleStat battleStat;
	protected override ICard TargetCard => targetCard;
	public override IStat Stat => battleStat;
	
	public static UnitCardInHand Instantiate(UnitCard targetUnitCard, UnitCardBattleStat unitCardBattleStat, string cardPrefabPath)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath)).AddComponent<UnitCardInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.targetCard = targetUnitCard;
		cardObject.battleStat = unitCardBattleStat;
		cardObject.cardPrefabPath = cardPrefabPath;

		return cardObject;
	}


	protected override bool CanUse(ITile tile = null)
	{
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		return tile?.TileType == ObjectType.Ally
		       && map.GetBattleObjectOfTile(tile) == null
		       && cardObjectStateMachine.CurrentState is UnitCardSelectedInHandState;
	}
	
	private void SummonCreature(ITile targetTile, string cardPrefabPath)
	{
		//todo: 결합끊기
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerField.AddToField(
			UnitCardInField.Instantiate(targetCard, targetTile, battleStat, ObjectType.Ally, cardPrefabPath));
	}

	protected override void OnPointerClickImpl(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None || !CanSelect()) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInHandState { IsHovered: true } &&
		    eventData.button == PointerEventData.InputButton.Left)
		{
			NoticeSystem.Instance.PublishSync(new HandCardSelectNotice(this));
			ChangeState(new UnitCardSelectedInHandState(this));
		}
	}
	
	
	/// <summary>
	/// 마우스 포인터를 따라가는 상태
	/// </summary>
	private class UnitCardSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private UnitCardInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public UnitCardSelectedInHandState(UnitCardInHand owner)
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
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			owner.transform.position = targetPos.GetX0z(Constant.SelectYPos);;
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
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.battleStat.Cost;
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
			NoticeSystem.Instance.PublishSync(new HandCardSelectCancelNotice(owner));
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
	
	private class CardObjectUsedInHandState : IState
	{
		private UnitCardInHand owner;
		private ITile targetTile;

		public CardObjectUsedInHandState(UnitCardInHand owner, ITile targetTile)
		{
			this.owner = owner;
			this.targetTile = targetTile;
		}

		public void Enter(IState prevState)
		{
			NoticeSystem.Instance.Publish(new HandCardStartUseNotice(owner));
			owner.SummonCreature(targetTile, owner.cardPrefabPath);

			//todo: 결합끊기
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerHand.RemoveCard(owner);

			owner.Deactivate();
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.Publish(new HandCardEndUseNotice(owner));
		}
	}
}