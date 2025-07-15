using MessageSystem;

public abstract class UnitCardActionBase : IAction
{
	protected IBattleObject owner;

	private IUpdatableRoutine routine;
	public IUpdatableRoutine UpdatableRoutine => routine;

	protected UnitCardActionBase()
	{
		routine = new UpdatableRoutine(UpdateFrame);
	}
	
	public void Trigger(object triggerInfo = null)
	{
		NoticeSystem.Instance.Publish(new CardActionTriggerNotice(owner, this));
		//todo: updatable routine 내부로?
		routine.Initialize();
		OnTrigger(triggerInfo);
	}

	public void Cancel()
	{
		NoticeSystem.Instance.Publish(new CardActionEndNotice(owner, this));
		OnCancel();
	}

	public virtual void SetBattleOwner(IBattleObject owner)
	{
		this.owner = owner;
	}

	//public abstract GridSelector AttackRangeInfo { get; }

	private void UpdateFrame(float dt, out bool routineDone)
	{
		OnUpdate(dt, out routineDone);
		if (routineDone)
		{
			NoticeSystem.Instance.Publish(new CardActionEndNotice(owner, this));
		}
	}

	protected abstract void OnUpdate(float dt, out bool routineDone);

	protected abstract void OnTrigger(object triggerInfo = null);

	protected abstract void OnCancel();
}