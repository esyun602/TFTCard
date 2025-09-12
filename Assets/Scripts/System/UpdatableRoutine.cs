using System;
using System.Collections;
using System.Collections.Generic;

public class UpdatableRoutine : IUpdatableRoutine
{
	public static IUpdatableRoutine CurrentRoutine { get; protected set; }
	private IUpdatableRoutine currentInterruptRoutine;
	public delegate void UpdatableRoutineDelegate(float dt, out bool done);

	private Queue<IUpdatableRoutine> interruptQueue;
	private Queue<IUpdatableRoutine> chainQueue;
	private UpdatableRoutineDelegate baseRoutine;

	private UpdatableRoutineDelegate currentSubRoutine;

	private Action initializeAction;
	private Action completeAction;
	
	public UpdatableRoutine(UpdatableRoutineDelegate routine, Action initializeAction = null, Action completeAction = null)
	{
		this.baseRoutine = routine;
		this.initializeAction = initializeAction;
		this.completeAction = completeAction;
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
			completeAction?.Invoke();
			return;
		}

		CurrentRoutine = this;
		
		routineDone = false;
		
		if (currentInterruptRoutine == null)
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
			currentInterruptRoutine.UpdateFrame(dt, out var subRoutineDone);
			if (subRoutineDone)
			{
				if (interruptQueue.TryDequeue(out var iRoutine))
				{
					currentInterruptRoutine = iRoutine;
					iRoutine.Initialize();
				}
				else
				{
					currentInterruptRoutine = null;
				}
			}
		}
	}

	public void AddChain(IUpdatableRoutine routine)
	{
		chainQueue.Enqueue(routine);
	}

	public void AddInterrupt(IUpdatableRoutine routine)
	{
		if (currentInterruptRoutine == null)
		{
			currentInterruptRoutine = routine;
			routine.Initialize();
		}
		else
		{
			interruptQueue.Enqueue(routine);
		}
	}
}