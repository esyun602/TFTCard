using MessageSystem;

public abstract class CardActionBase : IAction
{
	protected IBattleObject owner;

	private IUpdatableRoutine routine;
	public IUpdatableRoutine UpdatableRoutine => routine;

	protected CardActionBase()
	{
		routine = new UpdatableRoutine(UpdateFrame);
	}
	
	public void Trigger()
	{
		NoticeSystem.Instance.Publish(new CardActionTriggerNotice(owner, this));
		//todo: updatable routine 내부로?
		routine.Initialize();
		OnTrigger();
	}

	public void Cancel()
	{
		NoticeSystem.Instance.Publish(new CardActionEndNotice(owner, this));
		OnCancel();
	}

	public void SetBattleOwner(IBattleObject owner)
	{
		this.owner = owner;
	}

	public abstract GridSelector AttackRangeInfo { get; }

	private void UpdateFrame(float dt, out bool routineDone)
	{
		OnUpdate(dt, out routineDone);
		if (routineDone)
		{
			NoticeSystem.Instance.Publish(new CardActionEndNotice(owner, this));
		}
	}

	protected abstract void OnUpdate(float dt, out bool routineDone);
	
	protected abstract void OnTrigger();

	protected abstract void OnCancel();
}