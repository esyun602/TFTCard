public class CatalystBuff : IBuff
{
	private IBattleObject target;
	public BuffType BuffType => BuffType.Negative;
	public BattleValueType ControlBattleValueType => BattleValueType.Catalyst;
	private int catalystLevel;
	public int Level { get; }

	public void OnAdd(IBattleObject target)
	{
		this.target = target;
	}

	public void OnRemove()
	{
	}

	public bool TryStack(IBuff buff)
	{
		var canStack = buff is CatalystBuff;
		if (canStack)
		{
			catalystLevel += buff.Level;
		}

		return canStack;
	}
}