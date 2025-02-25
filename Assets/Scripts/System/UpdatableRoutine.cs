using System;
using System.Collections;
using System.Collections.Generic;

public class UpdatableRoutine : IUpdatableRoutine
{
	public static IUpdatableRoutine CurrentRoutine { get; protected set; }
	public delegate void UpdatableRoutineDelegate(float dt, out bool done);

	private Queue<UpdatableRoutineDelegate> interruptQueue;
	private Queue<UpdatableRoutineDelegate> chainQueue;
	private UpdatableRoutineDelegate baseRoutine;

	private UpdatableRoutineDelegate currentSubRoutine;
	
	public UpdatableRoutine(UpdatableRoutineDelegate routine)
	{
		this.baseRoutine = routine;
	}

	public void Initialize()
	{
		currentSubRoutine = baseRoutine;
		interruptQueue = new();
		chainQueue = new();
	}

	public void UpdateFrame(float dt, out bool routineDone)
	{
		if (currentSubRoutine == null && chainQueue.TryDequeue(out var routine))
		{
			currentSubRoutine = routine;
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
				currentSubRoutine = chainedRoutine;
			}
			else if (subRoutineDone)
			{
				currentSubRoutine = null;
			}
		}
		else
		{
			interruptQueue.Peek().Invoke(dt, out var subRoutineDone);
			if (subRoutineDone)
			{
				interruptQueue.Dequeue();
			}
		}
	}

	public void AddChain(IUpdatableRoutine routine)
	{
		chainQueue.Enqueue(routine.UpdateFrame);
	}

	public void AddInterrupt(IUpdatableRoutine routine)
	{
		interruptQueue.Enqueue(routine.UpdateFrame);
	}
}