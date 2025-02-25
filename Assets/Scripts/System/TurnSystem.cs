
using System;
using System.Collections.Generic;
using System.Linq;
using MessageSystem;

public class TurnSystem
{
	private const float MaxTurnGauge = 100;
	private Dictionary<ITurnObject, float> turnGaugeDict;
	private ITurnObject currentObject;
	//todo: 타이 해결
	private Queue<ITurnObject> candidates;
	private Action<float> currentUpdateRoutine;
	private IUpdatableRoutine priorityRoutine;
	
	public void Initialize()
	{
		turnGaugeDict = new();
		candidates = new();
		
		var playerTurn = new PlayerTurn();
		playerTurn.Initialize();
		RegisterNewObject(playerTurn, MaxTurnGauge);
		
		currentUpdateRoutine = DetermineCandidates;
	}

	public void Dispose()
	{
		//todo:impl
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

	private void UpdateCurrentObject(float dt)
	{
		//todo: start 전에 update가 불리는 경우 방지
		currentObject.UpdatableRoutine.UpdateFrame(dt, out var routineDone);
		if (routineDone)
		{
			turnGaugeDict[currentObject] = 0f;
			NoticeSystem.Instance.Publish(new TurnEndNotice(currentObject));
			currentObject = null;
			currentUpdateRoutine = DetermineCurrentObject;
		}
	}

	private void DetermineCurrentObject(float dt)
	{
		if (candidates.Count == 0)
		{
			currentUpdateRoutine = DetermineCandidates;
			return;
		}
		
		//todo: fix
		currentObject = candidates.Dequeue();
		currentObject.StartTurn();
		NoticeSystem.Instance.Publish(new TurnStartNotice(currentObject));
		
		currentUpdateRoutine = UpdateCurrentObject;
	}

	private void DetermineCandidates(float dt)
	{
		foreach (var key in turnGaugeDict.Keys.ToArray())
		{
			turnGaugeDict[key] += key.TurnSpeed * dt;
			if (turnGaugeDict[key] >= MaxTurnGauge)
			{
				candidates.Enqueue(key);
				currentUpdateRoutine = DetermineCurrentObject;
			}
		}
		
		NoticeSystem.Instance.Publish(new TurnGaugeUpdateNotice(MaxTurnGauge, turnGaugeDict));
	}
	
	public void RegisterNewObject(ITurnObject obj, float startGauge = 0f)
	{
		if (!turnGaugeDict.TryAdd(obj, startGauge))
		{
			throw new ArgumentException();
		}
		
		NoticeSystem.Instance.Publish(new TurnObjectRegisterNotice(obj));
	}

	public void UnregisterObject(ITurnObject obj)
	{
		turnGaugeDict.Remove(obj);
		
		NoticeSystem.Instance.Publish(new TurnObjectUnregisterNotice(obj));
	}
	
	//todo: fix?
	public void RegisterPriorityRoutine(IUpdatableRoutine routine)
	{
		priorityRoutine = routine;
	}
	
	//턴?
	//이동/소환 -> 턴종 --> 플레이어 턴도 speed를 가지게?
}