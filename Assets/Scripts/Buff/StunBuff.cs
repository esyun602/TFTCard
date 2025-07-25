using MessageSystem;

public class StunBuff : IBuff
{
	private IBattleObject target;
	public BuffType BuffType => BuffType.Negative;
	public ValueType ControlValueType => ValueType.Stun;
	public int Level => 1;
	public void OnAdd(IBattleObject target)
	{
		this.target = target;
		NoticeSystem.Instance.Subscribe<TurnStartNotice>(OnTurnStart);
	}

	public void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<TurnStartNotice>(OnTurnStart);
	}

	private void OnTurnStart(TurnStartNotice m)
	{
		if (m.TargetObject == target)
		{
			target.UnitCardBattleStat.RemoveBuff(this);
		}
	}

	public bool TryStack(IBuff buff)
	{
		return true;
	}
}