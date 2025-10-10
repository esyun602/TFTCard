using System;
using UnityEngine;

[Flags]
public enum DamageType
{
	SkillAttack = 0,
	NormalAttack = 1,
	Pierce = 2,
	Bomb = 3,
}

public struct DamageInfo
{
	public IBattleObject Sender { get; set; }
	public DamageType DamageType { get; set; }
	private int dmg;

	public int Dmg
	{
		get => dmg;
		set => dmg = Mathf.Max(0, value);
	}
}

public struct HealInfo
{
	public IBattleObject Sender { get; set; }
	public int HealAmount { get; set; }
	
}

public interface IDamagedBehaviour
{
	public void AttachTo(IBattleObject obj);
	public void DetachFrom(IBattleObject obj);
	public void Damage(DamageInfo damageInfo);
	public void Heal(HealInfo healInfo);
	public void Die(IBattleObject sender);
}