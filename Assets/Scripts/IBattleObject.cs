
using UnityEngine;

public interface IBattleObject
{
	public string Name { get; }
	public ObjectType ObjectType { get; }
	public Vector3 Position { get; }
	public Transform Transform { get; }
	public Transform FrameTransform { get; }
	
	public void DestroyObject(IBattleObject destroyer);
	
	
	public IBattleObjectStat UnitCardBattleStat { get; }
	public IDamagedBehaviour DamagedBehaviour { get; }
}

public static class IBattleObjectExtensions
{
	public static void Damage(this IBattleObject bo, DamageInfo dmgInfo)
	{
		bo.DamagedBehaviour?.Damage(dmgInfo);
	}
	
	public static void Heal(this IBattleObject bo, HealInfo healInfo)
	{
		bo.DamagedBehaviour?.Heal(healInfo);
	}
	
	public static bool IsDead(this IBattleObject bo)
	{
		return bo.UnitCardBattleStat.GetValueByValueType(UnitValueType.Hp) == 0;
	}
}