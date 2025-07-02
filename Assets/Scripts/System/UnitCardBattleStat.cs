
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

public class UnitCardBattleStat : IStat
{
	//todo: actiondata -> action으로 만드는거 전투 중 혹은 게임 중 파라메터가 바뀔 여지는 없나?
	//                    피격 범위를 어떻게 포현할 수 있을까
	//                    키워드를 어떻게 추가/제거할 것인가 -> IOption?,
	//                    키워드가 정확히 어떤 것을 바꾸는가? -> 반격 : 데미지 후 체이닝 등록, 연속공격 : 트리거 시 체이닝 등록, 눈, 암흑 - 그냥 공격 데미지에 추가로 바르는 것, 그랩: 체이닝,   
	//                    체인을 어떻게 구성할 것인가 -> ITurnObject에서 체이닝 관리
	private UnitCardStat originStat;
	public int Attack { get; }
	private int hp;

	public int Hp
	{
		get => hp;
		set
		{
			var clampedValue = Mathf.Max(value, 0);
			NoticeSystem.Instance.Publish(new BattleHpChangeNotice(hp, clampedValue, this));
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
			NoticeSystem.Instance.Publish(new BattleTurnCountChangedNotice(turnCount, clampedValue, this));
			turnCount = clampedValue;
		}
	}

	private int turnCount;
	public int Cost { get; set; }
	private List<IOption> optionList;
	//field scope 기믹
	private List<IBuff> buffList;
	private List<Synergy> synergyList;

	public UnitCardBattleStat(UnitCardStat unitCardStat)
	{
		originStat = unitCardStat;
		Attack = unitCardStat.Attack;
		MaxHp = hp = unitCardStat.MaxHp;
		MaxTurnCount = unitCardStat.MaxTurnCount;
		turnCount = MaxTurnCount;
		Cost = unitCardStat.Cost;
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
			default:
				return new int[] { };
		}
	}
}