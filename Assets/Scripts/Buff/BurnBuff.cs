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
		NoticeSystem.Instance.Subscribe<TurnEndNotice>(OnTurnEnd);
	}

	private void OnTurnEnd(TurnEndNotice m)
	{
		if (m.TargetObject == target)
		{
			target.Damage(null, burnLevel--);
		}
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<TurnEndNotice>(OnTurnEnd);
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