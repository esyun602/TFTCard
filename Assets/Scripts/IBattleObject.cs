
using UnityEngine;

public interface IBattleObject
{
	public ObjectType ObjectType { get; }
	public Vector3 Position { get; }
	public Transform Transform { get; }
	public UnitCardBattleStat UnitCardBattleStat { get; }
	public void Damage(IBattleObject sender, int dmg);
}