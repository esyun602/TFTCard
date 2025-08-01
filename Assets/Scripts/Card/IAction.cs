using UnityEngine;

public interface IAction
{
	public IUpdatableRoutine UpdatableRoutine { get; }
	public void Trigger(object triggerInfo = null);
	public void Cancel();
}