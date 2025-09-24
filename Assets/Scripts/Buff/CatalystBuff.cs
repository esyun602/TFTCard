public class CatalystBuff : BuffBase
{
	public override BuffType BuffType => BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.Catalyst;

	public CatalystBuff(int catalystLevel)
	{
		Level = catalystLevel;
	}
	
	public override bool TryStack(IBuff buff)
	{
		var canStack = buff is CatalystBuff;
		if (canStack)
		{
			Level += buff.Level;
		}

		return canStack;
	}

	public override string Keyword => "Catalyst";
}