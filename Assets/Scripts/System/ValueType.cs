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

public sealed class SkillValueType : ValueType
{
	public static readonly SkillValueType MaxHpAdd = new("MaxHpAdd");
	public static readonly SkillValueType AttackAdd = new("AttackAdd");
	public static readonly SkillValueType ShieldAdd = new("ShieldAdd");
	public static readonly SkillValueType BurnAdd = new("BurnAdd");
	public static readonly SkillValueType CatalystAdd = new("CatalystAdd");
	public static readonly SkillValueType StunAdd = new("StunAdd");
	public static readonly SkillValueType DodgeAdd = new("DodgeAdd");
	public static readonly SkillValueType HealBanAdd = new("HealBanAdd");
	public static readonly SkillValueType Draw = new("Draw");
	public static readonly SkillValueType Heal = new("Heal");
	public static readonly SkillValueType Damage = new("Damage");
	public static readonly SkillValueType BombDamage = new("BombDamage");
	public static readonly SkillValueType Cost = new("Cost");
	public static readonly SkillValueType Exhaustion = new("Exhaustion");
	public static readonly SkillValueType AttackCount = new("AttackCount");
	public static readonly SkillValueType GoldAdd = new("GoldAdd");

	public SkillValueType(string name) : base(name)
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
	public static readonly UnitValueType BurnImmune = new("BurnImmune", (_) => new BurnImmuneBuff());
	public static readonly UnitValueType Regeneration = new("Regeneration", (lv) => new RegenerationBuff(lv));
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
		func = buffCreator;
		typeDict.Add(name, this);
	}
}