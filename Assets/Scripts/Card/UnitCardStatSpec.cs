using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UnitCardStatSpec : ScriptableObject, IStat
{
	public int hp;
	public int attack;
	public int turnCount;
	public int cost;
	public SynergyCategory[] synergy;

	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
				return new int[]{ hp };
			case ValueType.Hp:
				return new int[] { hp };
			case ValueType.TurnCount:
				return new int[] { turnCount };
			case ValueType.MaxTurnCount:
				return new int[] { turnCount };
			case ValueType.Attack:
				return new int[] { attack };
			case ValueType.Cost:
				return new int[] { cost };
			default:
				return new int[] { };
		}
	}
}