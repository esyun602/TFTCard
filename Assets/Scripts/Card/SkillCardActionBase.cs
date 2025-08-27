public abstract class SkillCardActionBase : IAction
{
	protected SkillCardStat Stat { get; private set; }
	protected SkillCardBattleStat BattleStat { get; private set; }
	protected IStat StatFallback => BattleStat != null ? BattleStat : Stat;

	protected IUpdatableRoutine routine;
	public IUpdatableRoutine UpdatableRoutine => routine;

	public virtual bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

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

	public abstract object[] DescParams { get; }

	public virtual void SetCardBattleStat(SkillCardBattleStat stat)
	{
		this.BattleStat = stat;
	}
	
	public virtual void SetCardStat(SkillCardStat stat)
	{
		this.Stat = stat;
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