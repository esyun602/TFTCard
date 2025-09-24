using MessageSystem;

public class HealBanBuff : BuffBase
{
	public override BuffType BuffType => BuffType.DefiniteNegative;
	public override UnitValueType ControlUnitValueType => UnitValueType.HealBan;
	public int Level => 1;

	public void AddTo(IBattleObject target)
	{
	}

	public void RemoveFromObject()
	{
	}

	public override bool TryStack(IBuff buff)
	{
		return true;
	}

	public override string Keyword => "Healban";
}