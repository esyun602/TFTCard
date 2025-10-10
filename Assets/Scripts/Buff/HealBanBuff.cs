using MessageSystem;

public class HealBanBuff : BuffBase
{
	public override BuffType DefaultType => BuffType.Definite | BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.HealBan;
	public int Level => 1;

	public void AddTo(IBattleObject target)
	{
	}

	public void RemoveFromObject()
	{
	}

	protected override bool TryStackImpl(IBuff buff)
	{
		return true;
	}

	public override string Keyword => "Healban";
}