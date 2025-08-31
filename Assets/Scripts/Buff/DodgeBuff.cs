using MessageSystem;

public class DodgeBuff : BuffBase
{
	public override BuffType BuffType => BuffType.Positive;
	public override UnitValueType ControlUnitValueType => UnitValueType.Dodge;

	protected override void OnAdd()
	{
		NoticeSystem.Instance.Subscribe<DamageDodgeNotice>(OnDodge);
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<DamageDodgeNotice>(OnDodge);
	}

	private void OnDodge(DamageDodgeNotice m)
	{
		if (m.DodgedObject == target)
		{
			target.UnitCardBattleStat.RemoveBuff(this);
		}
	}

	public override bool TryStack(IBuff buff)
	{
		return true;
	}

	public override string Keyword => "Dodge";
}