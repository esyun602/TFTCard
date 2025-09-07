using System.Collections.Generic;

public class UnitActionAdapter : UnitSkillCardActionBase
{
	private UnitCardActionBase unitAction;
	public override object[] DescParams => unitAction.DescParams;
	public override IEnumerable<ITile> Targets => unitAction.Targets;

	public UnitActionAdapter(UnitCardActionBase unitAction)
	{
		this.unitAction = unitAction;
	}

	public override void SetCardBattleStat(SkillCardBattleStat stat)
	{
		base.SetCardBattleStat(stat);
		unitAction.SetBattleOwner(((UnitSkillCardBattleStat)stat).Owner);
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		unitAction.UpdatableRoutine.UpdateFrame(dt, out routineDone);
	}

	protected override void OnTrigger()
	{
		unitAction.SetBattleOwner(BattleStat.Owner);
		unitAction.Trigger();
	}

	protected override void OnCancel()
	{
		unitAction.Cancel();	
	}
}