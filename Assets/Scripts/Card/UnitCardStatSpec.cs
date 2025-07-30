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

	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
				return new int[]{ hp };
			case BattleValueType.Hp:
				return new int[] { hp };
			case BattleValueType.TurnCount:
				return new int[] { turnCount };
			case BattleValueType.MaxTurnCount:
				return new int[] { turnCount };
			case BattleValueType.Attack:
				return new int[] { attack };
			case BattleValueType.Cost:
				return new int[] { cost };
			default:
				return new int[] { };
		}
	}
}