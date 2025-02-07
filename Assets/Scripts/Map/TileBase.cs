using UnityEngine;

public class TileBase : ITile
{
	/*public new Transform transform
	{
		get
		{
			if (transformCache)
			{
				return transformCache;
			}

			transformCache = transform;
			return transformCache;
		}
	}

	public Transform transformCache;

	public Vector2Int GetPosition()
	{
		return new Vector2Int((int)transform.position.x, (int)transform.position.z);
	}*/
	private Vector2Int position;
	private int tileSize;
	private ObjectType tileType;
	public TileBase(Vector2Int position, int tileSize, ObjectType tileType)
	{
		this.position = position;
		this.tileSize = tileSize;
		this.tileType = tileType;
	}
	public Vector3 GetPosition()
	{
		return position.ToVector3XZ(tileSize);
	}

	public ObjectType TileType => tileType;
}
