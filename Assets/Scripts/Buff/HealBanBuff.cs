using MessageSystem;

public class HealBanBuff : IBuff
{
	public BuffType BuffType => BuffType.DefiniteNegative;
	public BattleValueType ControlBattleValueType => BattleValueType.HealBan;
	public int Level => 1;

	public void OnAdd(IBattleObject target)
	{
	}

	public void OnRemove()
	{
	}

	public bool TryStack(IBuff buff)
	{
		return true;
	}
}