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

	private Queue<IUpdatableRoutine> interruptQueueForInitialize = new();
	private Queue<IUpdatableRoutine> chainQueueForInitialize = new();
	
	private UpdatableRoutineDelegate currentSubRoutine;

	private Action initializeAction;
	private Action completeAction;
	private Action disposableFailAction;
	private Action disposableCompleteAction;
	private Func<bool> triggerCondition;

	private bool finished;

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
		(chainQueue, chainQueueForInitialize) = (chainQueueForInitialize, chainQueue);
		(interruptQueue, interruptQueueForInitialize) = (interruptQueueForInitialize, interruptQueue);

		finished = false;
		
		if (triggerCondition != null && !triggerCondition.Invoke())
		{
			chainQueue = new();
			interruptQueue = new();
			disposableCompleteAction = null;
			updateFunc = TriggerFailRoutine;
			return;
		}
		else
		{
			disposableFailAction = null;
			if (interruptQueue.TryDequeue(out var routine))
			{
				currentInterruptRoutine = routine;
				routine.Initialize();
			}
			
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
			finished = routineDone = true;
			CurrentRoutine = null;
			disposableCompleteAction?.Invoke();
			disposableCompleteAction = null;
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
		finished = routineDone = true;
		disposableFailAction?.Invoke();
		disposableFailAction = null;
	}

	public void AddChain(IUpdatableRoutine routine)
	{
		chainQueue.Enqueue(routine);
	}

	public void AddChainAtInitialize(IUpdatableRoutine routine)
	{
		chainQueueForInitialize.Enqueue(routine);
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
	
	public void AddInterruptAtInitialize(IUpdatableRoutine routine)
	{
		interruptQueueForInitialize.Enqueue(routine);
	}

	public void AddOnFailOnce(Action failAction)
	{
		this.disposableFailAction += failAction;
	}

	public void AddOnCompleteOnce(Action completeAction)
	{
		this.disposableCompleteAction += completeAction;
	}

	public int InterruptWaitCount => interruptQueue.Count + (currentInterruptRoutine == null ||
	                                                         ((currentInterruptRoutine as UpdatableRoutine)?.finished ??
	                                                          false) ? 0 : 1);
}