using UnityEngine;

public interface IAction : IUpdatableRoutine
{
	public void Trigger();
	public void Cancel();
	public void SetBattleOwner(IBattleObject owner);
}