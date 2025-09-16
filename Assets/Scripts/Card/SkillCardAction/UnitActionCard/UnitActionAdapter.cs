using System.Collections.Generic;

public class UnitActionAdapter : UnitSkillCardActionBase
{
	private UnitCardActionBase unitAction;
	public override object[] DescParams => unitAction.DescParams;
	public override IEnumerable<ITile> Targets => unitAction.Targets;

	public UnitActionAdapter(UnitActionAdapterSpec spec, UnitCardActionBase unitAction) : base(spec)
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
		//todo: for test, 죽음 관련해서 좀 더 정교하게
		if (BattleStat.Owner == null || BattleStat.Owner.IsDead())
		{
			routineDone = true;
			return;
		}
		
		unitAction.UpdatableRoutine.UpdateFrame(dt, out routineDone);
	}

	protected override void OnTrigger()
	{
		unitAction.Trigger();
	}

	protected override void OnCancel()
	{
		unitAction.Cancel();	
	}
}