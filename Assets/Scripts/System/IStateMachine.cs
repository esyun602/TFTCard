public interface IStateMachine
{
	public void ChangeState(IState nextState);
	public IState CurrentState { get; }
}