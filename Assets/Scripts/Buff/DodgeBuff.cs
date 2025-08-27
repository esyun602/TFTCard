using MessageSystem;

public class DodgeBuff : IBuff
{
	private IBattleObject target;
	public BuffType BuffType => BuffType.Positive;
	public UnitValueType ControlUnitValueType => UnitValueType.Dodge;
	public int Level => 1;

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<DamageDodgeNotice>(OnDodge);
	}

	public void OnRemove()
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

	public bool TryStack(IBuff buff)
	{
		return true;
	}
}