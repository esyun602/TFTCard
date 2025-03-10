using System;
using System.Collections;
using System.Collections.Generic;

public class UpdatableRoutine : IUpdatableRoutine
{
	public static IUpdatableRoutine CurrentRoutine { get; protected set; }
	public delegate void UpdatableRoutineDelegate(float dt, out bool done);

	private Queue<IUpdatableRoutine> interruptQueue;
	private Queue<IUpdatableRoutine> chainQueue;
	private UpdatableRoutineDelegate baseRoutine;

	private UpdatableRoutineDelegate currentSubRoutine;

	private Action initializeAction;
	
	public UpdatableRoutine(UpdatableRoutineDelegate routine, Action initializeAction = null)
	{
		this.baseRoutine = routine;
		this.initializeAction = initializeAction;
	}

	public void Initialize()
	{
		currentSubRoutine = baseRoutine;
		interruptQueue = new();
		chainQueue = new();
		
		initializeAction?.Invoke();
	}

	public void UpdateFrame(float dt, out bool routineDone)
	{
		if (currentSubRoutine == null && chainQueue.TryDequeue(out var routine))
		{
			routine.Initialize();
			currentSubRoutine = routine.UpdateFrame;
		}
		else if (currentSubRoutine == null && interruptQueue.Count == 0)
		{
			routineDone = true;
			CurrentRoutine = null;
			return;
		}

		CurrentRoutine = this;
		
		routineDone = false;
		
		if (interruptQueue.Count == 0)
		{
			currentSubRoutine.Invoke(dt, out var subRoutineDone);
			if (subRoutineDone && chainQueue.TryDequeue(out var chainedRoutine))
			{
				chainedRoutine.Initialize();
				currentSubRoutine = chainedRoutine.UpdateFrame;
			}
			else if (subRoutineDone)
			{
				currentSubRoutine = null;
			}
		}
		else
		{
			interruptQueue.Peek().UpdateFrame(dt, out var subRoutineDone);
			if (subRoutineDone)
			{
				interruptQueue.Dequeue();
			}
		}
	}

	public void AddChain(IUpdatableRoutine routine)
	{
		chainQueue.Enqueue(routine);
	}

	public void AddInterrupt(IUpdatableRoutine routine)
	{
		interruptQueue.Enqueue(routine);
		routine.Initialize();
	}
}