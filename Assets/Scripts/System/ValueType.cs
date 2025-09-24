//todo: 빌드 시 bake
//todo: 생성자 접근 제한

using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ValueType
{
	private static Dictionary<string, ValueType> typeDict = new();

	public static bool TryParse(string str, out ValueType type)
	{
		return typeDict.TryGetValue(str.ToLower(), out type);
	}

	public string Name { get; }

	protected ValueType(string name)
	{
		Name = name;
		typeDict.Add(name.ToLower(), this);
	}
}

public sealed class CommonValueType : ValueType
{
	public static readonly CommonValueType MaxHpAdd = new("MaxHpAdd");
	public static readonly CommonValueType AttackAdd = new("AttackAdd");
	public static readonly CommonValueType ShieldAdd = new("ShieldAdd");
	public static readonly CommonValueType BurnAdd = new("BurnAdd");
	public static readonly CommonValueType CatalystAdd = new("CatalystAdd");
	public static readonly CommonValueType StunAdd = new("StunAdd");
	public static readonly CommonValueType DodgeAdd = new("DodgeAdd");
	public static readonly CommonValueType HealBanAdd = new("HealBanAdd");
	public static readonly CommonValueType Draw = new("Draw");
	public static readonly CommonValueType Heal = new("Heal");

	public CommonValueType(string name) : base(name)
	{
	}
}

public sealed class UnitValueType : ValueType
{
	private static Dictionary<string, UnitValueType> typeDict = new();

	public static readonly UnitValueType Hp = new("Hp");
	public static readonly UnitValueType MaxHp = new("MaxHp");
	public static readonly UnitValueType Attack = new("Attack", (lv)=>new ValueAddAttackBuff(lv));
	public static readonly UnitValueType Shield = new("Shield");
	public static readonly UnitValueType Burn = new("Burn", (lv)=>new BurnBuff(lv));
	public static readonly UnitValueType Catalyst = new("Catalyst", (lv)=>new CatalystBuff(lv));
	public static readonly UnitValueType Stun = new("Stun");
	public static readonly UnitValueType Dodge = new("Dodge");
	public static readonly UnitValueType HealBan = new("HealBan");

	private Func<int, IBuff> func;
	public IBuff InstantiateBuff(int level)
	{
		return func?.Invoke(level);
	}
	
	public static bool TryParse(string str, out UnitValueType type)
	{
		return typeDict.TryGetValue(str, out type);
	}

	public UnitValueType(string name, Func<int, IBuff> buffCreator = null) : base(name)
	{
		typeDict.Add(name, this);
	}
}

public sealed class SkillValueType : ValueType
{
	private static Dictionary<string, SkillValueType> typeDict = new();

	public static readonly SkillValueType Damage = new("Damage");
	public static readonly SkillValueType Cost = new("Cost");
	public static readonly SkillValueType Exhaustion = new("Exhaustion");

	public static bool TryParse(string str, out SkillValueType type)
	{
		return typeDict.TryGetValue(str, out type);
	}

	public SkillValueType(string name) : base(name)
	{
		typeDict.Add(name, this);
	}
}