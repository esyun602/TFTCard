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
	private Vector3 position;
	private const float tileHeight = 0.5f;
	private ObjectType tileType;
	private Bounds tileBounds;
	public TileBase(Vector3 position, Vector3 tileSize, ObjectType tileType)
	{
		this.position = position;
		this.tileType = tileType;
		tileBounds = new Bounds(position, tileSize);
	}
	public Vector3 GetPosition()
	{
		return position.GetX0z(Constant.FieldYPos);
	}

	public bool Contains(Vector3 position)
	{
		return tileBounds.Contains(position);
	}

	public ObjectType TileType => tileType;
}
