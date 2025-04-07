
public interface ITurnObject
{
	//todo: 필요?
	public IUpdatableRoutine UpdatableRoutine { get; }
	public void StartTurn();
	public int TurnCount { get; }
}