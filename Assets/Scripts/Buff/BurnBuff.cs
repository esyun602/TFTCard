using MessageSystem;

public class BurnBuff : BuffBase
{

	public BurnBuff(int burnLevel)
	{
		Level = burnLevel;
	}

	public override BuffType BuffType => BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.Burn;

	protected override void OnAdd()
	{
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	private void OnTurnEnd(PlayerTurnEndNotice m)
	{
		target.Damage(new DamageInfo()
		{
			Dmg = Level--,
		});
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	public override bool TryStack(IBuff buff)
	{
		var canStack = buff is BurnBuff;
		if (canStack)
		{
			Level += buff.Level;
		}

		return canStack;
	}

	public override string Keyword => "Burn";
}