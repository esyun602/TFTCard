using System;
using System.Collections.Generic;
using DG.Tweening;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitCardInField : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IBattleObject, ITurnObject, IMessageReceiver
{
	private ObjectType objectType;
	private UnitCard targetUnitCard;
	private const string cardPrefabPath = "Card/CardPrefab";

	public ObjectType ObjectType => objectType;
	public Vector3 Position => transform.position;
	private Transform transformCache;
	public Transform Transform => transformCache == null ? transformCache = transform : transformCache;

	public Transform FrameTransform { get; private set; }
	private SimpleStateMachine cardObjectStateMachine = new();
	private Material materialCache;
	private Material Material => materialCache == null ? materialCache = FrameTransform.Find("DamageFx").GetComponent<MeshRenderer>().material : materialCache;
	public IBattleObjectStat UnitCardBattleStat { get; private set; }
	public IDamagedBehaviour DamagedBehaviour { get; private set; }
	
	private void Awake()
	{
		FrameTransform = transform.Find("CardFrame").transform;
	}

	public void CatchMessage(Message m)
	{
		//todo: 분리
		if (m is DamageNotice)
		{
			RunHitAction();
		}
		else if (m is DamageDodgeNotice)
		{
			RunDodgeAction();
		}
		NoticeSystem.Instance.SendSync(m, cardObjectStateMachine);
	}

	//todo: 나중에 그냥 애니메이션 셋으로 통일
	private void RunHitAction()
	{
		var movSeq = DOTween.Sequence();
		movSeq.Append(FrameTransform
			.DOLocalMove((ObjectType == ObjectType.Ally ? -1f : 1f) * 1f * Transform.right,
				0.15f).SetEase(Ease.InQuart));
		movSeq.Append(FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

		Material.DOFade(1, 0.15f).SetLoops(2, LoopType.Yoyo);
		
		//틴트, 데미지 텍스트
		
		movSeq.Play();
	}
	
	private void RunDodgeAction()
	{
		var movSeq = DOTween.Sequence();
		movSeq.Append(FrameTransform
			.DOLocalMove((ObjectType == ObjectType.Ally ? -1f : 1f) * 1f * Transform.right,
				0.15f).SetEase(Ease.InQuart));
		movSeq.Append(FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));
		
		movSeq.Play();
	}

	public void UpdateBlockInput(InputBlockFlag flag)
	{
		blockInput = flag;
		if ((flag & InputBlockFlag.Hover) != InputBlockFlag.None &&
		    cardObjectStateMachine.CurrentState is CardObjectNormalInFieldState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}

	public void DestroyObject(IBattleObject destroyer)
	{
		//todo: pooling
		ChangeState(null);
		
		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GetSkillCardInstance(targetUnitCard.UnitSkillCard)?.SetOwner(null);
		
		DamagedBehaviour.DetachFrom(this);
		DamagedBehaviour = null;
		
		//stat에서 dispose에서
		UnitCardBattleStat.Dispose();
		//UnitCardBattleStat = null;
		NoticeSystem.Instance.Publish(new BattleObjectDestroyedNotice(destroyer, this));
		
		gameObject.SetActive(false);
		//todo: 수정
		//Destroy(this);
	}


	private bool CanMove(ITile target)
	{
		return target is { TileType: ObjectType.Ally } &&
		       (target == Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
			        .GetTileOfBattleObject(this) ||
		        Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.CardMoveCount > 0);
	}

	private void ChangeState(IState targetState)
	{
		cardObjectStateMachine.ChangeState(targetState);
	}

	private IUpdatableRoutine routine;
	private InputBlockFlag blockInput;

	public IUpdatableRoutine UpdatableRoutine => routine;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInFieldState normalState)
		{
			normalState.SetHover();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInFieldState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((blockInput & InputBlockFlag.Select) != InputBlockFlag.None) return;
		if (cardObjectStateMachine.CurrentState is CardObjectNormalInFieldState { IsHovered: true } &&
		    eventData.button == PointerEventData.InputButton.Left)
		{
			ChangeState(new CardObjectSelectedInFieldState(this));
		}
	}

	//todo: hand와 같은 리소스 쓰는게 확정되면 리소스 재활용 추가
	public static UnitCardInField Instantiate(UnitCard targetUnitCard, ITile targetTile, ObjectType objectType)
	{
		//todo: pooling
		var cardObject = GameObject
			.Instantiate(Resources.Load(cardPrefabPath), targetTile.GetPosition(), Camera.main.transform.localRotation)
			.AddComponent<UnitCardInField>();
		cardObject.targetUnitCard = targetUnitCard;
		cardObject.targetUnitCard.Action.SetBattleOwner(cardObject);
		cardObject.DamagedBehaviour = new UnitCardDamagedBehaviour();
		cardObject.DamagedBehaviour.AttachTo(cardObject);

		cardObject.objectType = objectType;
		var unitCardBattleStat = new UnitCardBattleStat(cardObject, targetUnitCard.Stat);
		cardObject.UnitCardBattleStat = unitCardBattleStat;

		//todo: fix
		NoticeSystem.Instance.PublishSync(new BattleObjectGeneratedNotice(cardObject, targetTile));
		NoticeSystem.Instance.PublishSync(new TurnObjectGeneratedNotice(cardObject));

		//기본적으로 선택 불가, 플레이어 카드의 경우 PlayerField의 제어를 받음
		cardObject.UpdateBlockInput(InputBlockFlag.Select);
		cardObject.ChangeState(new CardObjectNormalInFieldState(cardObject));

		cardObject.GetComponentInChildren<UnitCardInfoHandler>().Initialize(targetUnitCard, unitCardBattleStat);
		cardObject.GetComponentInChildren<BoxCollider>().size = Vector3.one;

		Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GetSkillCardInstance(targetUnitCard.UnitSkillCard)?.SetOwner(cardObject);
		
		return cardObject;
	}

	public static UnitCardInField Instantiate(UnitCardSpec unitCardSpec, ITile targetTile, ObjectType objectType)
	{
		var card = new UnitCard(unitCardSpec);
		return Instantiate(card, targetTile, objectType);
	}

	public void StartTurn(int overrideTurnCount)
	{
		if (UnitCardBattleStat.GetValueByValueType(BattleValueType.Stun) > 0)
		{
			NoticeSystem.Instance.Publish(new TurnStartBlockByStunNotice(this));
			return;
		}
		ChangeState(new CardObjectActionState(this, overrideTurnCount));
	}

	public int TurnCount => UnitCardBattleStat.GetValueByValueType(BattleValueType.TurnCount);

	private void Update()
	{
		cardObjectStateMachine.UpdateFrame(Time.deltaTime);
	}

	private class CardObjectNormalInFieldState : IState, IUpdatable, IMessageReceiver
	{
		private UnitCardInField owner;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;
		private AnimationCurve returnAnimationCurve;
		private const float returnTime = 0.5f;
		private float timePassed;
		private Vector3 startPos;
		private IMap map;

		private ITile actOverrideTile;

		public CardObjectNormalInFieldState(UnitCardInField owner)
		{
			this.owner = owner;
			map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			isHovered = true;
			hoverTarget = originalScale * 1.5f;
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
			owner.Transform.up = Camera.main.transform.up;
			Restart();
			RestartHover();
			//todo:fix
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
		}

		public void UpdateFrame(float dt)
		{
			UpdatePos(dt);
			UpdateScale(dt);
		}

		private void UpdatePos(float dt)
		{
			timePassed += dt;
			var progress = returnAnimationCurve.Evaluate(timePassed / returnTime);
			var targetPos = GetTargetPos();
			var isMoving = !targetPos.IsAlmostCloseToXZ(owner.transform.position);
			owner.transform.position = Vector3.Lerp(startPos.GetX0z(GetYValue(isMoving)), targetPos.GetX0z(GetYValue(isMoving)), progress);
		}

		private Vector3 GetTargetPos()
		{
			return actOverrideTile != null
				? actOverrideTile.GetPosition()
				: map.GetTileOfBattleObject(owner).GetPosition();
		}

		private float GetYValue(bool isMoving)
		{
			if (isHovered)
			{
				return Constant.FieldHoverYPos;
			}
			else if (actOverrideTile != null)
			{
				return Constant.FieldSwitchActYPos;
			}
			else if (isMoving)
			{
				return Constant.FieldMoveYPos;
			}
			else
			{
				return Constant.FieldYPos;
			}
		}

		public void CatchMessage(Message m)
		{
			if (m is BattleObjectPosUpdatedNotice)
			{
				Restart();
			}
			//todo: normal 상태에서 처리하지 않고 새로 상태 추가 필요
			else if (m is BattleObjectSwitchActNotice san)
			{
				actOverrideTile = san.TargetTile;
				Restart();
			}
		}

		private void UpdateScale(float dt)
		{
			hoverTimePassed += dt;
			var progress = returnAnimationCurve.Evaluate(hoverTimePassed / hoverTime);
			owner.Transform.localScale = Vector3.Lerp(startScale, hoverTarget, progress);
		}

		private void RestartHover()
		{
			hoverTimePassed = 0f;
			startScale = owner.Transform.localScale;
		}

		private void Restart()
		{
			timePassed = 0f;
			startPos = owner.transform.position;
		}
	}

	/// <summary>
	/// 마우스 포인터를 따라가는 상태
	/// </summary>
	private class CardObjectSelectedInFieldState : IState, IUpdatable
	{
		private UnitCardInField owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public CardObjectSelectedInFieldState(UnitCardInField owner)
		{
			this.owner = owner;
		}

		public void Enter(IState prevState)
		{
			//todo:fix
			NoticeSystem.Instance.PublishSync(new FieldCardSelectNotice(owner));

			InputManager.Instance.InputActions.Player.UseHandCard.Enable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Enable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed += OnTryMoveCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed += OnCancelMoveCard;
			owner.Transform.up = Camera.main.transform.up;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			owner.transform.position = targetPos.GetX0z(Constant.SelectYPos);
		}

		private void OnTryMoveCard(InputAction.CallbackContext obj)
		{
			if (!owner.CanMove(currentTile)) return;
			
			if (currentTile == Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
				    .GetTileOfBattleObject(owner))
			{
				CancelMove();
			}
			else
			{
				owner.ChangeState(new CardObjectMoveState(owner, currentTile));
			}
		}

		public void Exit(IState nextState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Disable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Disable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed -= OnTryMoveCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed -= OnCancelMoveCard;

			if (currentTile != null && Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
					    .GetBattleObjectOfTile(currentTile)
				    is IMessageReceiver receiver)
			{
				NoticeSystem.Instance.Send(new BattleObjectSwitchActNotice(null), receiver);
			}
		}

		private void OnCancelMoveCard(InputAction.CallbackContext obj)
		{
			CancelMove();
		}

		private void CancelMove()
		{
			owner.ChangeState(new CardObjectNormalInFieldState(owner));
			NoticeSystem.Instance.PublishSync(new FieldCardSelectCancelNotice(owner));
		}

		public void UpdateFrame(float dt)
		{
			//todo: optimize and fix - new input mouse pos not working
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos).GetX0z(Constant.SelectYPos);

			var mouseTile = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetTileAt(mousePos);
			if (currentTile != mouseTile)
			{
				if (currentTile != null && Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
						    .GetBattleObjectOfTile(currentTile)
					    is IMessageReceiver receiver)
				{
					NoticeSystem.Instance.Send(new BattleObjectSwitchActNotice(null), receiver);
				}

				currentTile = mouseTile;
			}

			targetPos = mousePos;
			if (owner.CanMove(currentTile))
			{
				var bo = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
					.GetBattleObjectOfTile(currentTile);
				if (bo is IMessageReceiver messageReceiver)
				{
					NoticeSystem.Instance.Send(
						new BattleObjectSwitchActNotice(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map
							.GetTileOfBattleObject(owner)), messageReceiver);
				}

				targetPos = currentTile.GetPosition().GetX0z(Constant.SelectYPos);
			}

			if (Vector3.Distance(targetPos, owner.transform.position) < 0.01f)
			{
				timePassed = 0f;
			}

			timePassed += dt;

			var realSpeed = followAnimationCurve.Evaluate(timePassed) * followSpeed;
			var totalTime = Vector3.Distance(targetPos, owner.transform.position) / realSpeed;
			owner.transform.position = Vector3.Lerp(owner.transform.position, targetPos, dt / totalTime);
			owner.Transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(
					                                Vector3.Distance(targetPos, owner.transform.position) * 50f *
					                                (targetPos.x > owner.transform.position.x ? -1f : 1f), -45f, 45f),
				                                Vector3.Cross(Camera.main.transform.forward,
					                                (targetPos - owner.transform.position).normalized)) *
			                                Camera.main.transform.localRotation;
		}
	}

	private class CardObjectMoveState : IState
	{
		private UnitCardInField owner;
		private ITile targetTile;

		public CardObjectMoveState(UnitCardInField owner, ITile targetTile)
		{
			this.owner = owner;
			this.targetTile = targetTile;
		}

		public void Enter(IState prevState)
		{
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().GetCurrentStage().Map;
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.CardMoveCount -= 1;
			owner.transform.position = targetTile.GetPosition();
			owner.Transform.up = Camera.main.transform.up;

			if (map.GetBattleObjectOfTile(targetTile) != null)
			{
				map.SwitchTile(map.GetTileOfBattleObject(owner), targetTile);
			}
			else
			{
				map.SetTile(targetTile, owner);
			}

			owner.ChangeState(new CardObjectNormalInFieldState(owner));
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.PublishSync(new PlayerFieldCardMoveNotice(owner));
		}
	}

	private class CardObjectActionState : IState
	{
		private UnitCardInField owner;
		private Action currentUpdateAction;
		private int turnCount;

		public CardObjectActionState(UnitCardInField owner, int turnCount)
		{
			this.turnCount = turnCount;
			this.owner = owner;
		}

		public void Enter(IState prevState)
		{
			owner.routine = new UpdatableRoutine(UpdateFrame);
			owner.routine.Initialize();
			owner.UnitCardBattleStat.AddValueByValueType(BattleValueType.TurnCount, -turnCount);
			currentUpdateAction = UpdateTurnCount;
			NoticeSystem.Instance.Publish(new TurnStartNotice(owner));
			owner.transform.position = owner.transform.position.GetX0z(Constant.FieldHoverYPos);
		}

		private void UpdateFrame(float dt, out bool done)
		{
			currentUpdateAction?.Invoke();
			done = currentUpdateAction == null;
			if (done)
			{
				owner.ChangeState(new CardObjectNormalInFieldState(owner));
			}
		}

		private float timePassed = 0f;

		private void UpdateTurnCount()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0.5f)
			{
				timePassed = 0f;
				if (owner.TurnCount == 0)
				{
					owner.targetUnitCard.Action.Trigger();
					currentUpdateAction = UpdateAttack;
				}
				else
				{
					currentUpdateAction = null;
				}
			}
		}

		private void UpdateAttack()
		{
			owner.targetUnitCard.Action.UpdatableRoutine.UpdateFrame(Time.deltaTime, out var routineDone);
			if (routineDone)
			{
				owner.UnitCardBattleStat.SetValueByValueType(BattleValueType.TurnCount, owner.UnitCardBattleStat.GetValueByValueType(BattleValueType.MaxTurnCount));
				currentUpdateAction = UpdateEndAttack;
			}
		}

		private void UpdateEndAttack()
		{
			timePassed += Time.deltaTime;
			if (timePassed > 0.5f)
			{
				timePassed = 0f;
				currentUpdateAction = null;
			}
		}

		public void Exit(IState nextState)
		{
			owner.Transform.localScale = Vector3.one;
			owner.transform.position = owner.transform.position.GetX0z(Constant.FieldYPos);
			//todo: end 날리는 타이밍을 chain 루틴이 다 끝나고 날려야 되는지 고민 필요
			NoticeSystem.Instance.Publish(new TurnEndNotice(owner));
		}
	}

	public void Dispose()
	{
	}
}