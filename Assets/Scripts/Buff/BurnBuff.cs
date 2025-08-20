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
	public BattleValueType ControlBattleValueType => BattleValueType.Burn;
	public int Level => burnLevel;

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<TurnStartNotice>(OnTurnStart);
	}

	private void OnTurnStart(TurnStartNotice m)
	{
		if (m.TargetObject == target)
		{
			target.Damage(new DamageInfo()
			{
				Dmg = burnLevel--,
			});
		}
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<TurnStartNotice>(OnTurnStart);
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