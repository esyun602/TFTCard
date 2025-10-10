using System;
using System.Collections.Generic;
using MessageSystem;

public abstract class SkillCardActionBase : IAction
{
	private string descKey;
	protected SkillCardStat Stat { get; private set; }
	protected SkillCardBattleStat BattleStat { get; private set; }
	protected IStat StatFallback => BattleStat != null ? BattleStat : Stat;

	protected IUpdatableRoutine routine;
	protected object triggerInfo;
	public IUpdatableRoutine UpdatableRoutine => routine;

	public virtual bool CanUse(ITile targetTile)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;

		var bo = map.GetBattleObjectOfTile(targetTile);
		return bo != null;
	}
	
	protected SkillCardActionBase(SkillCardActionSpec spec)
	{
		descKey = spec.DescKey;
		routine = new UpdatableRoutine(UpdateFrame, TriggerRoutine, CompleteRoutine, CanTrigger);
	}

	protected virtual bool CanTrigger()
	{
		return true;
	}
	
	public void Trigger()
	{
		routine.Initialize();
	}

	private void TriggerRoutine()
	{
		NoticeSystem.Instance.Publish(new SkillCardActionTriggerNotice(this));
		OnTrigger();
	}

	private void CompleteRoutine()
	{
		NoticeSystem.Instance.Publish(new SkillCardActionRoutineCompleteNotice(this));
	}

	public void Cancel()
	{
		//todo: publish notice
		OnCancel();
	}

	public virtual string Desc => GameDataSystem.Instance.GetGameData<GameString>().GetStringWithStat(descKey, StatFallback);

	//todo: remove
	public abstract IEnumerable<ITile> Targets { get; }
	public void SetTriggerParam(object triggerInfo)
	{
		this.triggerInfo = triggerInfo;
	}

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
			NoticeSystem.Instance.Publish(new SkillCardActionEndNotice(this));
		}
	}

	protected abstract void OnUpdate(float dt, out bool routineDone);

	protected abstract void OnTrigger();

	protected abstract void OnCancel();
	
	
}

public static class SkillCardActionBaseExtensions
{
}