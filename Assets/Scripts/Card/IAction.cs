using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
	public IUpdatableRoutine UpdatableRoutine { get; }
	public void Trigger();
	public void Cancel();
	public object[] DescParams { get; }
	public IEnumerable<ITile> Targets { get; }
	public void SetTriggerParam(object triggerInfo);
}