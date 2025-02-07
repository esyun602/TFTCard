
using UnityEngine;

public enum ObjectType
{
	Ally,
	Neutral,
	Enemy,
}

public interface ITile
{
	public Vector3 GetPosition();
	public ObjectType TileType { get; }
}