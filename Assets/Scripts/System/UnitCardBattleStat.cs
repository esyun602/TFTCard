
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
	public int Attack => originStat.Attack + GetValueFromBuffs(BattleValueType.Attack);
	public int MaxHp => originStat.MaxHp + GetValueFromBuffs(BattleValueType.MaxHp);
	public int MaxTurnCount => originStat.MaxTurnCount + GetValueFromBuffs(BattleValueType.MaxTurnCount);
	#endregion
	
	#region BattleValue
	private int hp;
	//todo: public setter 제거
	public int Hp
	{
		get => hp;
		set
		{
			var clampedValue = Mathf.Clamp(value, 0, MaxHp);
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(BattleValueType.Hp, hp, clampedValue, this));
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
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(BattleValueType.TurnCount, turnCount, clampedValue, this));
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
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(BattleValueType.Shield, shield, clampedValue, this));
			shield = clampedValue;
		}
	}
	#endregion
	
	public bool IsDead => Hp == 0;


	private Dictionary<Type, IOption> optionDict = new();
	//field scope 기믹
	private List<IBuff> buffList = new();
	private List<SynergyCategory> synergyList = new();

	public void AddOption(IOption targetOption)
	{
		//stack 루틴 분리 안해도 될 것 같은데, 추후 필요하면 수정
		optionDict.TryGetValue(targetOption.GetType(), out var option);
		if (option != null)
		{
			option.Level = Mathf.Max(option.Level, targetOption.Level);
			return;
		}
			
		optionDict[targetOption.GetType()] = targetOption;
		targetOption.OnAdd(owner);
	}

	public bool RemoveOption<T>() where T : IOption
	{
		var exist = optionDict.TryGetValue(typeof(T), out var option);
		if (exist)
		{
			return RemoveOption(option);
		}

		return false;
	}

	public bool RemoveOption(IOption targetOption)
	{
		var removed = optionDict.Remove(targetOption.GetType());
		if (removed)
		{
			targetOption.OnRemove();
		}

		return removed;
	}

	public void RemoveAllOption()
	{
		foreach (var kvp in optionDict)
		{
			kvp.Value.OnRemove();
		}
		
		optionDict.Clear();
	}
	

	public void AddBuff(IBuff targetBuff)
	{
		foreach (var buff in buffList)
		{
			var done = buff.TryStack(targetBuff);
			if (done) return;
		}

		var prevValue = this.GetValueByValueType(targetBuff.ControlBattleValueType);

		buffList.Add(targetBuff);
		targetBuff.OnAdd(owner);

		var curValue = this.GetValueByValueType(targetBuff.ControlBattleValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(targetBuff.ControlBattleValueType, prevValue, curValue, this));
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
		var prevValue = this.GetValueByValueType(targetBuff.ControlBattleValueType);
		var removed = buffList.Remove(targetBuff);
		if (!removed)
		{
			return false;
		}
		
		targetBuff.OnRemove();
		var curValue = this.GetValueByValueType(targetBuff.ControlBattleValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new BattleValueChangeNotice(targetBuff.ControlBattleValueType, prevValue, curValue, this));
		}

		return true;
	}

	public IBuff GetBuff<T>() where T : IBuff
	{
		return buffList.Find(buff => buff is T);
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
		return synergyList;
	}

	public void AddSynergy(SynergyCategory target)
	{
		synergyList.Add(target);
		NoticeSystem.Instance.Publish(new StatSynergyAddNotice(new List<SynergyCategory>(){ target }, owner));
	}

	public void RemoveSynergy(SynergyCategory target)
	{
		if (synergyList.Remove(target))
		{
			NoticeSystem.Instance.Publish(new StatSynergyRemoveNotice(new List<SynergyCategory>(){ target }, owner));
		}
	}
	
	public int[] GetValuesByValueType(BattleValueType type)
	{
		switch (type)
		{
			case BattleValueType.MaxHp:
				return new int[]{ MaxHp };
			case BattleValueType.Hp:
				return new int[] { Hp };
			case BattleValueType.TurnCount:
				return new int[] { TurnCount };
			case BattleValueType.MaxTurnCount:
				return new int[] { MaxTurnCount };
			case BattleValueType.Attack:
				return new int[] { Attack };
			case BattleValueType.Shield:
				return new int[] { Shield };
			default:
				return new int[] { GetValueFromBuffs(type) };
		}
	}

	//todo: negative / positive 분류 필요할수도
	private int GetValueFromBuffs(BattleValueType type)
	{
		var val = 0;
		foreach (var buff in buffList)
		{
			if (buff.ControlBattleValueType == type)
			{
				val += buff.Level;
			}
		}

		return val;
	}
}