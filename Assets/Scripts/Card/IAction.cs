using UnityEngine;

public interface IAction
{
	public IUpdatableRoutine UpdatableRoutine { get; }
	public void Trigger();
	public void Cancel();
	public void SetBattleOwner(IBattleObject owner);
	public GridSelector AttackRangeInfo { get; }
}