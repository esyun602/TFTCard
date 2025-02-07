
using UnityEngine;

public interface IBattleObject
{
	public ObjectType ObjectType { get; }
	public Vector3 Position { get; }
	public BattleStat BattleStat { get; }
}