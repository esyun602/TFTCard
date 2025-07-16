
public interface ITurnObject
{
	//todo: 필요?
	public IUpdatableRoutine UpdatableRoutine { get; }
	public void StartTurn(int overrideTurnCount = 0);
	public int TurnCount { get; }
}