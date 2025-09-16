using System.Collections.Generic;
using MessageSystem;

public abstract class UnitCardActionBase : IAction
{
	private string descKey;
	protected UnitCardInField owner;

	private IUpdatableRoutine routine;
	protected object triggerInfo;
	public IUpdatableRoutine UpdatableRoutine => routine;

	protected UnitCardActionBase(UnitCardActionSpec spec)
	{
		descKey = spec.DescKey;
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

	public string Desc => GameDataSystem.Instance.GetGameData<GameString>().Format(descKey, DescParams);

	public abstract object[] DescParams { get; }
	public abstract IEnumerable<ITile> Targets { get; }
	public void SetTriggerParam(object triggerInfo)
	{
		this.triggerInfo = triggerInfo;
	}

	public virtual void SetBattleOwner(IBattleObject owner)
	{
		this.owner = (UnitCardInField)owner;
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

	protected abstract void OnTrigger();

	protected abstract void OnCancel();
}