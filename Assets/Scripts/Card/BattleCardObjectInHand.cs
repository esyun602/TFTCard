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
public class BattleCardObjectInHand : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IMessageReceiver
{
	//todo: fix how?
	private const string cardPrefabPath = "Card/CardPrefab";
	private SimpleStateMachine cardObjectStateMachine = new();
	private new BoxCollider collider;

	private Card targetCard;

	private Vector3 handTargetPos;
	private Quaternion handTargetRotation;
	private Vector3 hoverTargetPos;

	private InputBlockFlag blockInput;

	//todo: 스탯이 없는 카드
	public BattleStat BattleStat { get; private set; }

	private bool CanSelect()
	{
		//todo: access fix?
		if (Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy < BattleStat.Cost)
		{
			return false;
		}

		return true;
	}
	
	private bool CanUse(ITile tile = null)
	{
		//todo: tile 없이 사용하는 경우?
		
		//todo: cost 방어?
		
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
		return tile?.TileType == ObjectType.Ally
		       && map.GetBattleObjectOfTile(tile) == null
		       && cardObjectStateMachine.CurrentState is CardObjectSelectedInHandState;
	}

	//todo: 풀링으로 수정
	public void Activate()
	{
		if (collider == null)
		{
			collider = GetComponentInChildren<BoxCollider>();
			GetComponentInChildren<CardInfoHandler>().Initialize(targetCard.CardStaticSpec, BattleStat);
		}

		gameObject.SetActive(true);
		transform.forward = Camera.main.transform.forward;
		ChangeState(new CardObjectNormalInHandState(this));
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);

		ChangeState(null);
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

	private void ChangeState(IState nextState)
	{
		cardObjectStateMachine.ChangeState(nextState);
	}

	private void Update()
	{
		cardObjectStateMachine.UpdateFrame(Time.deltaTime);
	}

	public void SummonCreature(ITile targetTile)
	{
		//todo: 결합끊기
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerField.AddToField(
			BattleCardObjectInField.Instantiate(targetCard, targetTile, BattleStat, ObjectType.Ally));
	}

	//todo: field와 같은 리소스 쓰는게 확정되면 리소스 재활용 추가
	public static BattleCardObjectInHand Instantiate(Card targetCard, BattleStat battleStat)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath)).AddComponent<BattleCardObjectInHand>();
		cardObject.gameObject.SetActive(false);
		cardObject.targetCard = targetCard;
		cardObject.BattleStat = battleStat;

		return cardObject;
	}

	public void Dispose()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None || !CanSelect()) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInHandState { IsHovered: true } &&
		    eventData.button == PointerEventData.InputButton.Left)
		{
			NoticeSystem.Instance.PublishSync(new HandCardSelectNotice(this));
			ChangeState(new CardObjectSelectedInHandState(this));
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

	private class CardObjectNormalInHandState : IState, IUpdatable, IMessageReceiver
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
		private Vector3 originalColliderScale = new Vector3(0.7f, 1f, 1f);
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
			owner.collider.size = Vector3.one;
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
			owner.collider.size = originalColliderScale;
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

	/// <summary>
	/// 마우스 포인터를 따라가는 상태
	/// </summary>
	private class CardObjectSelectedInHandState : IState, IUpdatable
	{
		public bool IsMoving => targetPos != owner.transform.position;
		private BattleCardObjectInHand owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public CardObjectSelectedInHandState(BattleCardObjectInHand owner)
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
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.Energy -= owner.BattleStat.Cost;
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
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

			currentTile = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetTileAt(mousePos);
			targetPos = owner.CanUse(currentTile) ? currentTile.GetPosition().GetX0z(Constant.SelectYPos) : mousePos.GetX0z(Constant.SelectYPos);

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
		private BattleCardObjectInHand owner;
		private ITile targetTile;

		public CardObjectUsedInHandState(BattleCardObjectInHand owner, ITile targetTile)
		{
			this.owner = owner;
			this.targetTile = targetTile;
		}

		public void Enter(IState prevState)
		{
			NoticeSystem.Instance.Publish(new HandCardStartUseNotice(owner));
			owner.SummonCreature(targetTile);

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