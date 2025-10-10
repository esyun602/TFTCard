//todo: negative를 별도로 둘건지 결정 필요

using UnityEngine;

public class ValueAddAttackBuff : BuffBase
{
	public override BuffType DefaultType => Level > 0 ? BuffType.Positive : BuffType.Negative;
	public override UnitValueType ControlUnitValueType => UnitValueType.Attack;
	

	public ValueAddAttackBuff(int attackAddValue)
	{
		Level = attackAddValue;
	}
	
	protected override void OnAdd()
	{
	}

	protected override void OnRemove()
	{
	}

	protected override bool TryStackImpl(IBuff buff)
	{
		if (buff is ValueAddAttackBuff)
		{
			Level += buff.Level;
			return true;
		}

		return false;
	}

	public override string Keyword => "AttackAdded";
}