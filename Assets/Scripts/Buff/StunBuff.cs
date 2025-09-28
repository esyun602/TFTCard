using MessageSystem;

public class StunBuff : BuffBase
{
	public override BuffType DefaultType => BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.Stun;
	protected override void OnAdd()
	{	
		NoticeSystem.Instance.Subscribe<TurnStartBlockByStunNotice>(OnTurnStart);
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<TurnStartBlockByStunNotice>(OnTurnStart);
	}

	private void OnTurnStart(TurnStartBlockByStunNotice m)
	{
		if (m.TargetObject == target)
		{
			target.UnitCardBattleStat.RemoveBuff<StunBuff>();
		}
	}

	protected override bool TryStackImpl(IBuff buff)
	{
		return true;
	}

	public override string Keyword => "Stun";	
}