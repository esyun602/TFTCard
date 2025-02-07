
public interface ITurnObject : IUpdatableRoutine
{
	public void StartTurn();
	public float TurnSpeed { get; }
	public void AddChain(IUpdatableRoutine routine);
	public void RemoveChain(IUpdatableRoutine routine);
}