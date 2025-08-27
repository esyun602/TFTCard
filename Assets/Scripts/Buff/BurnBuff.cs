using MessageSystem;

public class BurnBuff : IBuff
{
	private IBattleObject target;
	private int burnLevel;

	public BurnBuff(int burnLevel)
	{
		this.burnLevel = burnLevel;
	}

	public BuffType BuffType => BuffType.Negative;
	public UnitValueType ControlUnitValueType => UnitValueType.Burn;
	public int Level => burnLevel;

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	private void OnTurnEnd(PlayerTurnEndNotice m)
	{
		target.Damage(new DamageInfo()
		{
			Dmg = burnLevel--,
		});
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<PlayerTurnEndNotice>(OnTurnEnd);
	}

	public bool TryStack(IBuff buff)
	{
		var canStack = buff is BurnBuff;
		if (canStack)
		{
			burnLevel += buff.Level;
		}

		return canStack;
	}
}