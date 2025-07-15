using UnityEngine;

[CreateAssetMenu]
public class SkillCardStatSpec : ScriptableObject, IStat
{
	public int turnCountValue;
	public int hpValue;
	public int attackValue;
	public int costValue;
	public int shieldValue;

	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
			case ValueType.Hp:
				return new int[] { hpValue };
			case ValueType.TurnCount:
			case ValueType.MaxTurnCount:
				return new int[] { turnCountValue };
			case ValueType.Attack:
				return new int[] { attackValue };
			case ValueType.Cost:
				return new int[] { costValue };
			case ValueType.Shield:
				return new int[] { shieldValue };
			default:
				return new int[] { };
		}
	}
}