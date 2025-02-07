using MessageSystem;

public class SimpleStateMachine : IStateMachine, IUpdatable, IMessageReceiver
{
	private IState currentState;

	public void ChangeState(IState nextState)
	{
		currentState?.Exit(nextState);
		var tmp = currentState;
		currentState = nextState;
		currentState?.Enter(tmp);
	}
	public IState CurrentState => currentState;
	public void UpdateFrame(float dt)
	{
		(currentState as IUpdatable)?.UpdateFrame(dt);
	}

	public void CatchMessage(Message m)
	{
		if (currentState is IMessageReceiver receiver)
		{
			NoticeSystem.Instance.SendSync(m, receiver);
		}
	}
}