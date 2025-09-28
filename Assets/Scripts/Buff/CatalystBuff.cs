public class CatalystBuff : BuffBase
{
	public override BuffType DefaultType => BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.Catalyst;

	public CatalystBuff(int catalystLevel)
	{
		Level = catalystLevel;
	}

	protected override bool TryStackImpl(IBuff buff)
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