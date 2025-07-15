
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class UnitCardBattleStat : IStat
{
	//                    키워드를 어떻게 추가/제거  -> IOption?,
	//                    체인 -> ITurnObject에서 체이닝 관리
	private UnitCardStat originStat;
	public int Attack { get; }
	private int hp;

	public int Hp
	{
		get => hp;
		set
		{
			var clampedValue = Mathf.Max(value, 0);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(ValueType.Hp, hp, clampedValue, this));
			hp = clampedValue;
		}
	}
	public bool IsDead => Hp == 0;
	public int MaxHp { get; set; }
	public int MaxTurnCount { get; set; }

	public int TurnCount
	{
		get => turnCount;
		set
		{
			var clampedValue = Mathf.Max(value, 0);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(ValueType.TurnCount, turnCount, clampedValue, this));
			turnCount = clampedValue;
		}
	}

	private int turnCount;
	public int Cost { get; set; }
	private List<IOption> optionList;
	//field scope 기믹
	private List<IBuff> buffList;
	private List<Synergy> synergyList;
	private int shield;

	public int Shield
	{
		get => shield;
		set
		{
			var clampedValue = Mathf.Max(value, 0);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(ValueType.Shield, shield, clampedValue, this));
			shield = clampedValue;
		}
	}

	public UnitCardBattleStat(UnitCardStat unitCardStat)
	{
		originStat = unitCardStat;
		Attack = unitCardStat.Attack;
		MaxHp = hp = unitCardStat.MaxHp;
		MaxTurnCount = unitCardStat.MaxTurnCount;
		turnCount = MaxTurnCount;
		Cost = unitCardStat.Cost;
		Shield = 0;
		synergyList = new(unitCardStat.synergyList);
	}

	public List<Synergy> GetSynergyList()
	{
		return new(synergyList);
	}
	
	public int[] GetValuesByValueType(ValueType type)
	{
		switch (type)
		{
			case ValueType.MaxHp:
				return new int[]{ MaxHp };
			case ValueType.Hp:
				return new int[] { Hp };
			case ValueType.TurnCount:
				return new int[] { TurnCount };
			case ValueType.MaxTurnCount:
				return new int[] { MaxTurnCount };
			case ValueType.Attack:
				return new int[] { Attack };
			case ValueType.Cost:
				return new int[] { Cost };
			case ValueType.Shield:
				return new int[] { Shield };
			default:
				return new int[] { };
		}
	}
}