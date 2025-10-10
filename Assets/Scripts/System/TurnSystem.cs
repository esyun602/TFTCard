using System;
using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public class TurnSystem
{
	private ITurnObject currentObject;
	public int CurrentTurnCount => playerTurn.CurrentTurnCount;

	private int phase;
	//todo:fix
	private List<BattleCardObjectInHand> cardList;
	private List<int> currentUsableCosts;
	private List<int> currentCostCumulative;
	private int currentUsedCost;

	public int CurrentUsedCost
	{
		get => currentUsedCost;
		set
		{
			while (phase < currentCostCumulative.Count && currentCostCumulative[phase] <= value)
			{
				//todo: 죽었을 때? 데미지만 먼저 적용? 
				RegisterPlayerTurnRoutine(cardList[phase].TargetCard.Action.UpdatableRoutine);
				//todo: fix
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DropCard(cardList[phase]);
				phase++;
			}

			currentUsedCost = value;
		}
	}

	public int CurrentTotalCost => currentCostCumulative[^1];

	public void RegisterEnemyCard(BattleCardObjectInHand card)
	{
		cardList.Add(card);
		currentUsableCosts.Add(-card.Stat.GetValueByValueType(SkillValueType.Cost));
		currentCostCumulative.Add((currentCostCumulative.Count == 0 ? 0 : currentCostCumulative[^1]) + currentUsableCosts[^1]);
	}

	private Queue<ITurnObject> candidates;
	private Action<float> currentUpdateRoutine;
	private Queue<IUpdatableRoutine> priorityRoutine;
	private PlayerTurn playerTurn;

	public void Initialize()
	{
		//todo: fix subscribe once
		NoticeSystem.Instance.Subscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
		NoticeSystem.Instance.Subscribe<TurnEndClickNotice>(OnTurnEndButtonClick);

		priorityRoutine = new();
		candidates = new();

		currentUsableCosts = new();
		currentCostCumulative = new();
		cardList = new();
		currentUsedCost = 0;

		playerTurn = new PlayerTurn();
		playerTurn.Initialize();
	}
	
	private void OnTurnEndButtonClick(TurnEndClickNotice m)
	{
		CurrentUsedCost = CurrentTotalCost;
		playerTurn.EndTurn();
	}

	private void OnBattleStageInitRoutineDone(BattleStageInitRoutineDoneNotice m)
	{
		//todo: 일일히 해줘야하나?
		playerTurn.StartTurn();
		currentUpdateRoutine = UpdatePlayerTurn;
	}

	public void StartAutoTurn()
	{
		if (candidates.Count == 0)
		{
			//todo: fix
			cardList.Clear();
			currentUsableCosts.Clear();
			currentCostCumulative.Clear();
			phase = 0;
			currentUsedCost = 0;
			
			playerTurn.StartTurn();
			currentUpdateRoutine = UpdatePlayerTurn;
			return;
		}

		currentObject = candidates.Dequeue();
		currentObject.StartTurn();
		currentUpdateRoutine = UpdateAutoTurn;
	}

	public void Dispose()
	{
		playerTurn.Dispose();
		NoticeSystem.Instance.Unsubscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
		NoticeSystem.Instance.Unsubscribe<TurnEndClickNotice>(OnTurnEndButtonClick);
	}

	public void UpdateTurn(float dt)
	{
		if (priorityRoutine.Count != 0)
		{
			priorityRoutine.Peek().UpdateFrame(dt, out var done);
			if (done)
			{
				priorityRoutine.Dequeue();
			}

			return;
		}

		currentUpdateRoutine?.Invoke(dt);
	}

	private void UpdatePlayerTurn(float dt)
	{
		playerTurn.UpdatableCurrentRoutine.UpdateFrame(dt, out var done);
		if (done)
		{
			NoticeSystem.Instance.PublishSync(new PlayerTurnEndNotice(playerTurn));
			StartAutoTurn();
		}
	}

	private void UpdateAutoTurn(float dt)
	{
		//todo: start 전에 update가 불리는 경우 방지
		currentObject.UpdatableRoutine.UpdateFrame(dt, out var routineDone);
		if (routineDone)
		{
			if (candidates.Count > 0)
			{
				currentObject = candidates.Dequeue();
				currentObject.StartTurn();
			}
			else
			{
				playerTurn.StartTurn();
				currentUpdateRoutine = UpdatePlayerTurn;
			}
		}
	}

	public void Register(ITurnObject obj)
	{
		candidates.Enqueue(obj);
	}

	public void RegisterPlayerTurnRoutine(IUpdatableRoutine routine)
	{
		playerTurn.UpdatableCurrentRoutine.AddInterrupt(routine);
	}

	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine.Enqueue(routine);
		routine.Initialize();
	}
}