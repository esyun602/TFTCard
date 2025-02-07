using System.Collections.Generic;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleCardObjectInField : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IMessageReceiver, IBattleObject, ITurnObject
{
	private ObjectType objectType;
	private Card targetCard;
	private const string cardPrefabPath = "Card/CardPrefab";
	
	public ObjectType ObjectType => objectType;
	public Vector3 Position => transform.position;
	public BattleStat BattleStat { get; private set; }

	private Queue<IUpdatableRoutine> currentChain = new();
	private IUpdatableRoutine currentRoutine;
	
	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}

	public void CatchMessage(Message m)
	{
		throw new System.NotImplementedException();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	}
	
	public static BattleCardObjectInField Instantiate(Card targetCard, ITile targetTile, BattleStat battleStat, ObjectType objectType)
	{
		var cardObject = GameObject.Instantiate(Resources.Load(cardPrefabPath), targetTile.GetPosition(), Camera.main.transform.localRotation).AddComponent<BattleCardObjectInField>();
		cardObject.targetCard = targetCard;
		cardObject.targetCard.Action.SetBattleOwner(cardObject);

		cardObject.objectType = objectType;
		cardObject.BattleStat = battleStat;
		
		Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map.SetTile(targetTile, cardObject);
		//todo: 끊기?
		Game.Instance.GetGameMode<BattleStageGameMode>().TurnSystem.RegisterNewObject(cardObject);

		return cardObject;
	}

	public void UpdateFrame(float dt, out bool routineDone)
	{
		if (currentRoutine == null && currentChain.Count == 0)
		{
			routineDone = true;
			return;
		}
		
		currentRoutine.UpdateFrame(dt, out routineDone);
		if (routineDone)
		{
			currentRoutine = currentChain.TryDequeue(out var routine) ? routine : null;
		}
	}

	public void StartTurn()
	{
		targetCard.Action.Trigger();
		currentRoutine = targetCard.Action;
	}

	public float TurnSpeed => BattleStat.Speed;
	public void AddChain(IUpdatableRoutine routine)
	{
		currentChain.Enqueue(routine);
	}

	public void RemoveChain(IUpdatableRoutine routine)
	{
		throw new System.NotImplementedException();
	}
}