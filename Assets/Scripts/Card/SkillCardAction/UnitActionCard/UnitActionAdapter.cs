public class UnitActionAdapter : UnitSkillCardActionBase
{
	private UnitCardActionBase unitAction;
	public override object[] DescParams => unitAction.DescParams;

	public UnitActionAdapter(UnitCardActionBase unitAction)
	{
		this.unitAction = unitAction;
	}

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		unitAction.UpdatableRoutine.UpdateFrame(dt, out routineDone);
	}

	protected override void OnTrigger(object triggerInfo = null)
	{
		unitAction.SetBattleOwner(battleStat.Owner);
		unitAction.Trigger(triggerInfo);
	}

	protected override void OnCancel()
	{
		unitAction.Cancel();	
	}
}