
using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

//todo: rename?
public class UnitCardBattleStat : IStat
{
	private IBattleObject owner;
	private UnitCardStat originStat;
	#region StatValue
	public int Attack => originStat.Attack + GetValueFromBuffs(ValueType.Attack);
	public int MaxHp => originStat.MaxHp + GetValueFromBuffs(ValueType.MaxHp);
	public int MaxTurnCount => originStat.MaxTurnCount + GetValueFromBuffs(ValueType.MaxTurnCount);
	#endregion
	
	#region BattleValue
	private int hp;
	public int Hp
	{
		get => hp;
		set
		{
			var clampedValue = Mathf.Clamp(value, 0, MaxHp);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(ValueType.Hp, hp, clampedValue, this));
			hp = clampedValue;
		}
	}
	private int turnCount;
	public int TurnCount
	{
		get => turnCount;
		set
		{
			var clampedValue = Mathf.Clamp(value, 0, MaxTurnCount);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(ValueType.TurnCount, turnCount, clampedValue, this));
			turnCount = clampedValue;
		}
	}
	
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
	#endregion
	
	public bool IsDead => Hp == 0;


	private List<IOption> optionList;
	//field scope 기믹
	private List<IBuff> buffList;
	private List<SynergyCategory> synergyList;

	public void AddBuff(IBuff targetBuff)
	{
		foreach (var buff in buffList)
		{
			var done = buff.TryStack(targetBuff);
			if (done) return;
		}

		var prevValue = this.GetValueByValueType(targetBuff.ControlValueType);

		buffList.Add(targetBuff);
		targetBuff.OnAdd(owner);

		var curValue = this.GetValueByValueType(targetBuff.ControlValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(targetBuff.ControlValueType, prevValue, curValue, this));
		}
	}
	
	public bool RemoveBuff<T>() where T : IBuff
	{
		for (var i = buffList.Count - 1; i >= 0; i--)
		{
			if (buffList[i] is T)
			{
				return RemoveBuff(buffList[i]);
			}
		}

		return false;
	}
	
	public void RemoveAllBuff()
	{
		for (var i = buffList.Count - 1; i >= 0; i--)
		{
			RemoveBuff(buffList[i]);
		}

		buffList = new();
	}

	public bool RemoveBuff(IBuff targetBuff)
	{
		var prevValue = this.GetValueByValueType(targetBuff.ControlValueType);
		var removed = buffList.Remove(targetBuff);
		if (!removed)
		{
			return false;
		}
		
		var curValue = this.GetValueByValueType(targetBuff.ControlValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(targetBuff.ControlValueType, prevValue, curValue, this));
		}

		return true;
	}
	

	public UnitCardBattleStat(IBattleObject owner, UnitCardStat unitCardStat)
	{
		this.owner = owner;
		originStat = unitCardStat;
		hp = MaxHp;
		turnCount = MaxTurnCount;
		Shield = 0;
		synergyList = new(unitCardStat.synergyList);
	}

	public List<SynergyCategory> GetSynergyList()
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
			case ValueType.Shield:
				return new int[] { Shield };
			default:
				return new int[] { };
		}
	}

	//todo: negative / positive 분류 필요할수도
	private int GetValueFromBuffs(ValueType type)
	{
		var val = 0;
		foreach (var buff in buffList)
		{
			if (buff.ControlValueType == type)
			{
				val += buff.Level;
			}
		}

		return val;
	}
}