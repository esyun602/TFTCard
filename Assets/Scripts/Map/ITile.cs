
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
	public bool Contains(Vector3 position);
	public ObjectType TileType { get; }
}