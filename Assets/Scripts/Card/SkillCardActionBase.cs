public abstract class SkillCardActionBase : IAction
{
	protected SkillCardBattleStat stat;

	protected IUpdatableRoutine routine;
	public IUpdatableRoutine UpdatableRoutine => routine;

	public virtual bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().Map;

		var bo = map.GetBattleObjectOfTile(targetTile);
		return bo != null;
	}
	
	protected SkillCardActionBase()
	{
		routine = new UpdatableRoutine(UpdateFrame);
	}
	
	public void Trigger(object triggerInfo = null)
	{
		//todo: publish notice
		
		routine.Initialize();
		OnTrigger(triggerInfo);
	}

	public void Cancel()
	{
		//todo: publish notice
		OnCancel();
	}

	public virtual void SetCardBattleStat(SkillCardBattleStat stat)
	{
		this.stat = stat;
	}

	private void UpdateFrame(float dt, out bool routineDone)
	{
		OnUpdate(dt, out routineDone);
		if (routineDone)
		{
			//todo: publish
		}
	}

	protected abstract void OnUpdate(float dt, out bool routineDone);

	protected abstract void OnTrigger(object triggerInfo = null);

	protected abstract void OnCancel();
	
	
}