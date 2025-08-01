using System;
using UnityEngine;

//todo: dictionary 구현 후 사용하도록 편입하기
public enum TacticsValueType
{
	None = 0,
	Exhaustion = 1,
}

//todo: dictionary 형태 인스펙터에 띄우기
[CreateAssetMenu]
public class SkillCardStatSpec : ScriptableObject, IStat
{
	public int turnCountValue;
	public int hpValue;
	public int attackValue;
	public int costValue;
	public int shieldValue;
	public int catalystValue;
	public bool isExhaustion;

	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
			case BattleValueType.Hp:
				return new int[] { hpValue };
			case BattleValueType.TurnCount:
			case BattleValueType.MaxTurnCount:
				return new int[] { turnCountValue };
			case BattleValueType.Attack:
				return new int[] { attackValue };
			case BattleValueType.Cost:
				return new int[] { costValue };
			case BattleValueType.Shield:
				return new int[] { shieldValue };
			default:
				return new int[] { };
		}
	}

	/// <summary>
	/// 스펙에 값을 덮어씌우는 건 막기
	/// </summary>
	/// <param name="type"></param>
	/// <param name="newValues"></param>
	public void SetValuesByValueType(BattleValueType type, int[] newValues)
	{
#if UNITY_EDITOR
		throw new ArgumentException();
#endif
	}
}