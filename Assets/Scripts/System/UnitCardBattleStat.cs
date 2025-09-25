using System;
using System.Collections.Generic;
using MessageSystem;
using UnityEngine;

//todo: 인터페이스 분리
//todo: rename?
public class UnitCardBattleStat : IBattleObjectStat
{
	private IBattleObject owner;
	private UnitCardStat originStat;

	public void Revive()
	{
		RemoveAllBuff();
		Shield = 0;
		Hp = originStat.MaxHp;
	}
	
	#region StatValue

	private int Attack => originStat.Attack + GetValueFromBuffs(UnitValueType.Attack);
	private int MaxHp => originStat.MaxHp + GetValueFromBuffs(UnitValueType.MaxHp);

	#endregion

	#region BattleValue

	private int hp;

	//todo: public setter 제거
	private int Hp
	{
		get => hp;
		set
		{
			var clampedValue = Mathf.Clamp(value, 0, MaxHp);
			NoticeSystem.Instance.Publish(new UnitBattleValueChangeNotice(UnitValueType.Hp, hp, clampedValue, this));
			hp = clampedValue;
		}
	}

	private int shield;

	private int Shield
	{
		get => shield;
		set
		{
			var clampedValue = Mathf.Max(value, 0);
			NoticeSystem.Instance.Publish(new UnitBattleValueChangeNotice(UnitValueType.Shield, shield, clampedValue,
				this));
			shield = clampedValue;
		}
	}

	#endregion

	public bool IsDead => Hp == 0;

	#region Option

	private Dictionary<Type, IOption> optionDict = new();

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

	public IOption GetOption<T>() where T : IOption
	{
		return optionDict.GetValueOrDefault(typeof(T));
	}

	#endregion

	#region Buff

	//field scope 기믹
	private Dictionary<IBuff, object> buffRequester = new();
	private List<IBuff> buffList = new();
	private List<SynergyCategory> synergyList = new();


	public void Purify()
	{
		for (var i = buffList.Count - 1; i >= 0; i--)
		{
			if (!buffList[i].BuffType.IsAny(BuffType.Positive | BuffType.Definite) && buffRequester[buffList[i]] == null)
			{
				RemoveBuff(buffList[i]);
			}
		}
	}


	public void AddBuff(IBuff targetBuff, object requester = null)
	{
		if (targetBuff.BuffType.IsAny(BuffType.BlockOptionAdd) && requester != null) throw new InvalidOperationException();
		
		foreach (var buff in buffList)
		{
			//종류와 requester가 같으면 반드시 스택
			if (buff.GetType() == targetBuff.GetType() && buffRequester[buff] == buffRequester[targetBuff])
			{
				buff.TryStack(targetBuff);
				return;
			}
		}

		buffRequester[targetBuff] = requester;
		var prevValue = this.GetValueByValueType(targetBuff.ControlUnitValueType);

		buffList.Add(targetBuff);
		targetBuff.AddTo(owner);

		var curValue = this.GetValueByValueType(targetBuff.ControlUnitValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new UnitBattleValueChangeNotice(targetBuff.ControlUnitValueType, prevValue,
				curValue, this));
		}
	}

	public void RemoveAllBuff()
	{
		for (var i = buffList.Count - 1; i >= 0; i--)
		{
			RemoveBuff(buffList[i]);
		}

		buffList = new();
	}

	public bool RemoveBuff<T>(object requester = null) where T : IBuff
	{
		var buff = GetBuff<T>(requester);
		if (buff == null) return false;

		return RemoveBuff(buff);
	}

	private bool RemoveBuff(IBuff buff)
	{
		var prevValue = this.GetValueByValueType(buff.ControlUnitValueType);
		buffList.Remove(buff);
		buffRequester.Remove(buff);

		buff.RemoveFromObject();
		var curValue = this.GetValueByValueType(buff.ControlUnitValueType);
		if (prevValue != curValue)
		{
			NoticeSystem.Instance.Publish(new UnitBattleValueChangeNotice(buff.ControlUnitValueType, prevValue,
				curValue, this));
		}

		return true;
	}

	public IBuff GetBuff<T>(object requester) where T : IBuff
	{
		return buffList.Find(buff => buff is T && buffRequester[buff] == requester);
	}

	#endregion

	public List<SynergyCategory> SynergyList => synergyList;

	public UnitCardBattleStat(IBattleObject owner, UnitCardStat unitCardStat)
	{
		this.owner = owner;
		originStat = unitCardStat;
		hp = MaxHp;
		Shield = 0;
		synergyList = new(unitCardStat.synergyList);
	}

	private Dictionary<ValueType, int> skillGlobalValueDict = new();
	public int[] GetValuesByValueType(ValueType type)
	{
		if (!type.IsUnitCompatible()) return new int[] { skillGlobalValueDict.GetValueOrDefault(type) };
		
		if (type == UnitValueType.MaxHp)
		{
			return new int[] { MaxHp };
		}
		else if (type == UnitValueType.Hp)
		{
			return new int[] { Hp };
		}
		else if (type == UnitValueType.Attack)
		{
			return new int[] { Attack };
		}
		else if (type == UnitValueType.Shield)
		{
			return new int[] { Shield };
		}
		else
		{
			return new int[] { GetValueFromBuffs(type) };
		}
	}

	/// <summary>
	/// 스탯 외의 값은 버프로만 추가
	/// </summary>
	/// <param name="type"></param>
	/// <param name="newValues"></param>
	public void SetValuesByValueType(ValueType type, int[] newValues)
	{
		if (type == UnitValueType.Hp)
		{
			Hp = newValues[0];
		}

		if (type == UnitValueType.Shield)
		{
			Shield = newValues[0];
		}

		if (type is UnitValueType utype)
		{
			var buff = utype.InstantiateBuff(newValues[0]);
			if (buff != null)
			{
				AddBuff(buff);
			}
		}
	}

	//todo: negative / positive 분류 필요할수도
	private int GetValueFromBuffs(ValueType type)
	{
		var val = 0;
		foreach (var buff in buffList)
		{
			if (buff.ControlUnitValueType == type)
			{
				val += buff.Level;
			}
		}

		return val;
	}

	public void Dispose()
	{
		RemoveAllBuff();
		RemoveAllOption();
	}
}