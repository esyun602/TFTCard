public class CatalystBuff : IBuff
{
	private IBattleObject target;
	public BuffType BuffType => BuffType.Negative;
	public UnitValueType ControlUnitValueType => UnitValueType.Catalyst;
	private int catalystLevel;

	public CatalystBuff(int catalystLevel)
	{
		this.catalystLevel = catalystLevel;
	}

	public int Level => catalystLevel;

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