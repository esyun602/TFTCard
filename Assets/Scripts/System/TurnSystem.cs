using System;
using System.Collections;
using System.Collections.Generic;
using MessageSystem;

public class TurnSystem
{
	private ITurnObject currentObject;

	private Queue<ITurnObject> candidates;
	private Action<float> currentUpdateRoutine;
	private IUpdatableRoutine priorityRoutine;
	private PlayerTurn playerTurn;

	public void Initialize()
	{
		//todo: fix subscribe once
		NoticeSystem.Instance.Subscribe<BattleStageInitRoutineDoneNotice>(OnBattleStageInitRoutineDone);

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
		if (candidates.Count == 0)
		{
			//todo: fix
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

	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine = routine;
	}
}