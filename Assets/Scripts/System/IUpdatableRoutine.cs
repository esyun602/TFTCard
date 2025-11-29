using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatableRoutine
{
	public void Initialize();
	public void UpdateFrame(float dt, out bool routineDone);
	public void AddChain(IUpdatableRoutine routine);
	public void AddChainAtInitialize(IUpdatableRoutine routine);
	public void AddInterrupt(IUpdatableRoutine routine);
	public void AddInterruptAtInitialize(IUpdatableRoutine routine);
	public void AddOnFailOnce(Action failAction);
	public void AddOnCompleteOnce(Action failAction);
	public int InterruptWaitCount { get; }
}

public static class IUpdatableRoutineExtensions
{
	//todo: 필요하면 join 기능 updatable routine에 편입
	private class RoutineTimeManager
	{
		private IUpdatableRoutine owner;
		public RoutineTimeManager(IUpdatableRoutine owner)
		{
			this.owner = owner;
		}
		
		public float TimePassed { get; private set; } = 0f;
		public float Time { get; private set; } = 0f;
		public float LeftTime => Time - TimePassed;

		private List<(float, Action)> actionList = new();
		
		public void Update(float dt, out bool done)
		{
			TimePassed += dt;
			InvokeAction();
			if (TimePassed > Time && TimePassed - dt < Time)
			{
				done = true;
				routineTimeManagerDict.Remove(owner);
				TimePassed = 0f;
				return;
			}
			

			done = false;
		}

		private void InvokeAction()
		{
			for(var i = actionList.Count - 1; i >= 0; i--)
			{
				var (time, action) = actionList[i];
				if (time <= TimePassed)
				{
					action?.Invoke();
					actionList.RemoveAt(i);
				}
			}
		}

		public void AddCallback(float time, Action action)
		{
			var rTime = TimePassed + time;
			actionList.Add((rTime, action));
			Time = Mathf.Max(Time, rTime);
		}

	}

	private static Dictionary<IUpdatableRoutine, RoutineTimeManager> routineTimeManagerDict = new();
	
	public static void AddInterruptInterval(this IUpdatableRoutine routine, float timeAfter)
	{
		routine.AddInterrupt(GenerateRunAfterTime(timeAfter, null));
	}
	
	public static void AddInterrupt(this IUpdatableRoutine routine, Action action, float timeAfter, bool join = false)
	{
		if (!join)
		{
			routine.AddInterrupt(GenerateRunAfterTime(timeAfter, action));
		}
		else
		{
			AddInterruptJoin(routine, action, timeAfter);
		}
	}
	
	private static void AddInterruptJoin(IUpdatableRoutine routine, Action action, float timeAfter)
	{
		if (!routineTimeManagerDict.TryGetValue(routine, out var manager))
		{
			manager = new RoutineTimeManager(routine);
			routineTimeManagerDict[routine] = manager;
			routine.AddInterrupt(new UpdatableRoutine(manager.Update));
		}

		manager.AddCallback(timeAfter, action);
	}
	
	public static void AddChain(this IUpdatableRoutine routine, Action action, float timeAfter)
	{
		routine.AddChain(GenerateRunAfterTime(timeAfter, action));
	}
	
	public static UpdatableRoutine GenerateRunAfterTime(float time, Action action = null)
	{
		var timePassed = 0f;
		return new UpdatableRoutine((float dt, out bool done) =>
		{
			timePassed += dt;
			if (timePassed > time && timePassed - dt < time)
			{
				action?.Invoke();
				done = true;
				return;
			}
			
			done = false;
		});
	}
}