public interface IState
{
	public void Enter(IState prevState);
	public void Exit(IState nextState);
}