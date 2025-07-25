using System;
using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public class TurnSystem
{
	//todo: to balanced bst?
	private class TurnOrderHandler
	{
		//todo: fix tmp values
		public IEnumerator GetEnumerator()
		{
			var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;
			int[] cols = { 4, 5, 6, 7, 3, 2, 1, 0 };
			for (int row = 2; row >=0; row--)
			{
				foreach (var col in cols)
				{
					var bo = map.GetBattleObjectAt(row, col);
					if(bo is ITurnObject to)
					{
						yield return to;
					}
				}
			}
		}
	}


	private const float MaxTurnGauge = 100;
	private TurnOrderHandler turnOrderHandler;

	private ITurnObject currentObject;

	//todo: 타이 해결
	private Queue<ITurnObject> candidates;
	private Action<float> currentUpdateRoutine;
	private IUpdatableRoutine priorityRoutine;
	private IEnumerator currentTurnEnumerator;
	private PlayerTurn playerTurn;

	public void Initialize()
	{
		//todo: fix subscribe once
		NoticeSystem.Instance.Subscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);

		turnOrderHandler = new();
		candidates = new();

		playerTurn = new PlayerTurn();
		playerTurn.Initialize();
	}

	private void OnBattleStageInitRoutineDone(BattleStageInitRoutineDoneNotice m)
	{
		//todo: 일일히 해줘야하나?
		playerTurn.StartTurn();
		currentUpdateRoutine = UpdatePlayerTurn;
	}

	public void StartAutoTurn()
	{
		// ReSharper disable once NotDisposedResource : No Dispose Needed
		currentTurnEnumerator = turnOrderHandler.GetEnumerator();
		if (!currentTurnEnumerator.MoveNext())
		{
			//todo: fix
			playerTurn.StartTurn();
			currentUpdateRoutine = UpdatePlayerTurn;
			return;
		}

		currentObject = (ITurnObject)currentTurnEnumerator.Current;
		currentObject.StartTurn();
		NoticeSystem.Instance.Publish(new TurnStartNotice(currentObject));
		currentUpdateRoutine = UpdateAutoTurn;
	}

	public void Dispose()
	{
		(currentTurnEnumerator as IDisposable)?.Dispose();
		playerTurn.Dispose();
		NoticeSystem.Instance.Unsubscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);
	}

	public void UpdateTurn(float dt)
	{
		if (priorityRoutine != null)
		{
			priorityRoutine.UpdateFrame(dt, out var done);
			if (done)
			{
				priorityRoutine = null;
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
			StartAutoTurn();
		}
	}

	private void UpdateAutoTurn(float dt)
	{
		//todo: start 전에 update가 불리는 경우 방지
		currentObject.UpdatableRoutine.UpdateFrame(dt, out var routineDone);
		if (routineDone)
		{
			NoticeSystem.Instance.Publish(new TurnEndNotice(currentObject));
			if (currentTurnEnumerator.MoveNext())
			{
				currentObject = (ITurnObject)currentTurnEnumerator.Current;
				currentObject.StartTurn();
				NoticeSystem.Instance.Publish(new TurnStartNotice(currentObject));
			}
			else
			{
				(currentTurnEnumerator as IDisposable)?.Dispose();
				playerTurn.StartTurn();
				currentUpdateRoutine = UpdatePlayerTurn;
			}
		}
	}

//todo: fix?
	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine = routine;
	}
}