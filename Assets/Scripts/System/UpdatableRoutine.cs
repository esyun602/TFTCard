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
	private Func<bool> triggerCondition;

	private UpdatableRoutineDelegate updateFunc;
	public UpdatableRoutine(UpdatableRoutineDelegate routine, Action initializeAction = null, Action completeAction = null, Func<bool> triggerCondition = null)
	{
		this.baseRoutine = routine;
		this.initializeAction = initializeAction;
		this.completeAction = completeAction;
		this.triggerCondition = triggerCondition;
	}

	public void Initialize()
	{
		currentSubRoutine = baseRoutine;
		interruptQueue = new();
		chainQueue = new();

		if (triggerCondition != null && !triggerCondition.Invoke())
		{
			updateFunc = TriggerFailRoutine;
			return;
		}
		else
		{
			updateFunc = CommonRoutine;
			
		}

		
		initializeAction?.Invoke();
	}

	public void UpdateFrame(float dt, out bool routineDone)
	{
		updateFunc.Invoke(dt, out routineDone);
	}

	private void CommonRoutine(float dt, out bool routineDone)
	{
		if (currentSubRoutine == null && chainQueue.TryDequeue(out var routine))
		{
			routine.Initialize();
			currentSubRoutine = routine.UpdateFrame;
		}
		else if (currentSubRoutine == null && interruptQueue.Count == 0 && currentInterruptRoutine == null)
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

	private void TriggerFailRoutine(float dt, out bool routineDone)
	{
		routineDone = true;
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