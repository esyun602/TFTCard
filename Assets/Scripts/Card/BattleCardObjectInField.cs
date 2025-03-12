using System.Collections.Generic;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleCardObjectInField : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBattleObject, ITurnObject
{
	private ObjectType objectType;
	private Card targetCard;
	private const string cardPrefabPath = "Card/CardPrefab";
	
	public ObjectType ObjectType => objectType;
	public Vector3 Position => transform.position;
	public BattleStat BattleStat { get; private set; }
	private SimpleStateMachine cardObjectStateMachine = new();
	//todo: context?
	public void Damage(IBattleObject sender, int dmg)
	{
		BattleStat.Hp = Mathf.Max(BattleStat.Hp - dmg, 0);
		NoticeSystem.Instance.Publish(new DamageNotice(sender, this, dmg));
		if (BattleStat.IsDead)
		{
			Deactivate();
			NoticeSystem.Instance.Publish(new BattleObjectDestroyedNotice(sender, this));
		}
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

	private void Deactivate()
	{
		if (objectType == ObjectType.Ally)
		{
			ChangeState(null);
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.PlayerField.RemoveFromField(this);
		}
		
		Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.RemoveFromTile(this);
		Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.UnregisterObject(this);

		
		gameObject.SetActive(false);
	}
	
	private bool CanMove(ITile target)
	{
		return target != null 
		       && target != Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetTileOfBattleObject(this)
		       && Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetBattleObjectOfTile(target) == null;
	}
	
	private void ChangeState(IState targetState)
	{
		cardObjectStateMachine.ChangeState(targetState);
	}
	
	private Queue<IUpdatableRoutine> currentChain = new();
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
	
	public static BattleCardObjectInField Instantiate(Card targetCard, ITile targetTile, BattleStat battleStat, ObjectType objectType)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath), targetTile.GetPosition(), Camera.main.transform.localRotation).AddComponent<BattleCardObjectInField>();
		cardObject.targetCard = targetCard;
		cardObject.targetCard.Action.SetBattleOwner(cardObject);

		cardObject.objectType = objectType;
		cardObject.BattleStat = battleStat;
		cardObject.routine = cardObject.targetCard.Action.UpdatableRoutine;
		
		//todo: fix
		NoticeSystem.Instance.PublishSync(new BattleObjectGeneratedNotice(cardObject, targetTile));
		NoticeSystem.Instance.PublishSync(new TurnObjectGeneratedNotice(cardObject, cardObject.BattleStat.Speed));

		//기본적으로 선택 불가, 플레이어 카드의 경우 PlayerField의 제어를 받음
		cardObject.UpdateBlockInput(InputBlockFlag.Select);
		cardObject.ChangeState(new CardObjectNormalInFieldState(cardObject));
		
		return cardObject;
	}
		
	public static BattleCardObjectInField Instantiate(CardSpec cardSpec, ITile targetTile, ObjectType objectType)
	{
		var card = new Card(cardSpec);
		return Instantiate(card, targetTile, new BattleStat(card.Stat), objectType);
	}

	public void StartTurn()
	{
		targetCard.Action.Trigger();
	}

	private void Update()
	{
		cardObjectStateMachine.UpdateFrame(Time.deltaTime);
	}
	
	public int TurnSpeed => BattleStat.Speed;
	
	
	private class CardObjectNormalInFieldState : IState, IUpdatable
	{
		private BattleCardObjectInField owner;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = new Vector3(1.8f, 2.7f, 0.01f);
		private AnimationCurve returnAnimationCurve;
		private const float returnTime = 0.5f;
		private float timePassed;
		private Vector3 startPos;
		private IMap map;

		public CardObjectNormalInFieldState(BattleCardObjectInField owner)
		{
			this.owner = owner;
			map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
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
			owner.transform.position = Vector3.Lerp(startPos, map.GetTileOfBattleObject(owner).GetPosition(), progress);
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
		private BattleCardObjectInField owner;
		private Vector3 targetPos;
		private Quaternion targetRotation;
		private const float followSpeed = 400f;
		private AnimationCurve followAnimationCurve;
		private float timePassed = 0f;
		private ITile currentTile;

		public CardObjectSelectedInFieldState(BattleCardObjectInField owner)
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
			owner.transform.up = Camera.main.transform.up;
			followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			targetPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			owner.transform.position = targetPos;
		}

		private void OnTryMoveCard(InputAction.CallbackContext obj)
		{
			if(owner.CanMove(currentTile))
				owner.ChangeState(new CardObjectMoveState(owner, currentTile));
		}

		public void Exit(IState nextState)
		{
			InputManager.Instance.InputActions.Player.UseHandCard.Disable();
			InputManager.Instance.InputActions.Player.CancelHandCard.Disable();
			InputManager.Instance.InputActions.Player.UseHandCard.performed -= OnTryMoveCard;
			InputManager.Instance.InputActions.Player.CancelHandCard.performed -= OnCancelMoveCard;
		}

		private void OnCancelMoveCard(InputAction.CallbackContext obj)
		{
			owner.ChangeState(new CardObjectNormalInFieldState(owner));
			NoticeSystem.Instance.PublishSync(new FieldCardSelectCancelNotice(owner));
		}

		public void UpdateFrame(float dt)
		{
			//todo: optimize and fix - new input mouse pos not working
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 10f;
			var mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

			currentTile = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.GetTileAt(mousePos);
			targetPos = owner.CanMove(currentTile) ? currentTile.GetPosition() : mousePos;

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
			                                Vector3.Cross(Camera.main.transform.forward, (targetPos - owner.transform.position).normalized)) *
			                                Camera.main.transform.localRotation;
		}
	}

	private class CardObjectMoveState : IState
	{
		private BattleCardObjectInField owner;
		private ITile targetTile;

		public CardObjectMoveState(BattleCardObjectInField owner, ITile targetTile)
		{
			this.owner = owner;
			this.targetTile = targetTile;
		}

		public void Enter(IState prevState)
		{
			var map = Game.Instance.GetGameMode<BattleStageGameMode>().GetCurrentStage().Map;
			owner.transform.position = targetTile.GetPosition();
			owner.transform.up = Camera.main.transform.up;
			map.RemoveFromTile(owner);
			map.SetTile(targetTile, owner);
			owner.ChangeState(new CardObjectNormalInFieldState(owner));
		}

		public void Exit(IState nextState)
		{
			NoticeSystem.Instance.PublishSync(new PlayerFieldCardMoveNotice(owner));
		}
	}

	public void Dispose()
	{
	}
}