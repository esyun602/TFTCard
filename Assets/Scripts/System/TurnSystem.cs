using System;
using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public class TurnSystem
{
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
	
	private Action<float> currentUpdateRoutine;
	private Queue<IUpdatableRoutine> priorityRoutine;
	private PlayerTurn playerTurn;
	private UpdatableRoutine autoTurn;

	private void UpdateFrameForAutoTurn(float dt, out bool done)
	{
		done = true;
	}

	public void Initialize()
	{
		//todo: fix subscribe once
		NoticeSystem.Instance.Subscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
		NoticeSystem.Instance.Subscribe<TurnEndClickNotice>(OnTurnEndButtonClick);

		priorityRoutine = new();

		currentUsableCosts = new();
		currentCostCumulative = new();
		cardList = new();
		currentUsedCost = 0;

		playerTurn = new PlayerTurn();
		playerTurn.Initialize();

		autoTurn = new UpdatableRoutine(UpdateFrameForAutoTurn);
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

	private void StartPlayerTurn()
	{
		//todo: fix
		cardList.Clear();
		currentUsableCosts.Clear();
		currentCostCumulative.Clear();
		phase = 0;
		currentUsedCost = 0;
			
		playerTurn.StartTurn();
		currentUpdateRoutine = UpdatePlayerTurn;
	}
	
	public void StartAutoTurn()
	{
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
			autoTurn.Initialize();
			autoTurn.UpdateFrame(0, out var _);
			NoticeSystem.Instance.PublishSync(new PlayerTurnEndNotice(playerTurn));
			StartAutoTurn();
		}
	}

	private void UpdateAutoTurn(float dt)
	{
		autoTurn.UpdateFrame(dt, out var done);
		if (done)
		{
			StartPlayerTurn();
			playerTurn.UpdatableCurrentRoutine.UpdateFrame(0, out var _);
		}
	}
	
	public void RegisterPlayerTurnRoutine(IUpdatableRoutine routine)
	{
		playerTurn.UpdatableCurrentRoutine.AddInterrupt(routine);
	}

	public void RegisterAutoTurnRoutine(IUpdatableRoutine routine)
	{
		autoTurn.AddInterrupt(routine);
	}
	
	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine.Enqueue(routine);
		routine.Initialize();
	}
}