using System;
using System.Collections.Generic;
using System.Linq;
using MessageSystem;
using UnityEngine;


public class Map : IMap
{
	private class BattleObjectManager
	{
		private IMap owner;
		private Dictionary<ITile, IBattleObject> tileToBattleObject = new();
		private Dictionary<IBattleObject, ITile> battleObjectToTile = new();

		public BattleObjectManager(IMap owner)
		{
			this.owner = owner;
		}

		public void SetTile(ITile tile, IBattleObject obj)
		{
			//todo: rule을 맵마다 다르게 할꺼면 추후 분리
			var targetTile = owner.GetFirstTileInRow(tile);
			if (owner.GetBattleObjectOfTile(targetTile) != null)
			{
				targetTile = tile;
			}
			//
			
			//todo: object type 비교 필요?
			if (GetBattleObjectOn(tile) != null)
			{
				//todo: fix
				throw new ArgumentException();
			}

			RemoveFromTile(obj);
			battleObjectToTile[obj] = targetTile;
			tileToBattleObject[targetTile] = obj;

			if (obj is IMessageReceiver mr)
			{
				NoticeSystem.Instance.SendSync(new BattleObjectPosUpdatedNotice(obj, targetTile), mr);
			}
		}

		public void SwitchBattleObjectOfTile(ITile tileA, ITile tileB)
		{
			var objA = GetBattleObjectOn(tileA);
			var objB = GetBattleObjectOn(tileB);

			(battleObjectToTile[objA], battleObjectToTile[objB]) = (battleObjectToTile[objB], battleObjectToTile[objA]);
			(tileToBattleObject[tileA], tileToBattleObject[tileB]) = (tileToBattleObject[tileB], tileToBattleObject[tileA]);
			
			if (objA is IMessageReceiver mra)
			{
				NoticeSystem.Instance.SendSync(new BattleObjectPosUpdatedNotice(objA, tileB), mra);
			}
			
			if (objB is IMessageReceiver mrb)
			{
				NoticeSystem.Instance.SendSync(new BattleObjectPosUpdatedNotice(objB, tileA), mrb);
			}
		}

		public void RemoveFromTile(IBattleObject obj)
		{
			var targetTile = GetTileOf(obj);
			
			if (targetTile == null)
				return;
			
			
			tileToBattleObject.Remove(targetTile);
			battleObjectToTile.Remove(obj);
			
			//todo: rule을 맵마다 다르게 할꺼면 추후 분리
			if (targetTile == owner.GetFirstTileInRow(targetTile))
			{
				IBattleObject secondObj;
				if ((secondObj = owner.GetFirstObjectInRow(targetTile)) != null)
				{
					SetTile(targetTile, secondObj);
				}
			}
			//
			
		}

		public IBattleObject GetBattleObjectOn(ITile tile)
		{
			return tileToBattleObject.GetValueOrDefault(tile);
		}

		public ITile GetTileOf(IBattleObject obj)
		{
			return battleObjectToTile.GetValueOrDefault(obj);
		}

		public IBattleObject[] GetBattleObjects()
		{
			return tileToBattleObject.Values.ToArray();
		}
	}

	private BattleObjectManager battleObjectManager;
	private GameObject mapObject;
	private Dictionary<Vector2Int, ITile> tileDict;

	private int rowCnt;
	private int colCnt;

	private int xMin;
	private int xMax;
	private int yMin;
	private int yMax;

	//todo: fix
	private const int tileSizeX = 2;
	private const int tileSizeY = 3;
	
	public Map(GameObject mapObject)
	{
		this.mapObject = mapObject;
	}

	public void Load()
	{
		LoadBattleObjects();
		LoadTiles();
	}

	private void LoadTiles()
	{
		tileDict = new();
		xMin = yMin = int.MaxValue;
		xMax = yMax = int.MinValue;
		RegisterTiles(mapObject.transform, new HashSet<int>(), new HashSet<int>());
	}

	private void RegisterTiles(Transform root, HashSet<int> xSet, HashSet<int> ySet)
	{
		foreach (Transform child in root)
		{
			//todo constant, tileprop serialize
			if (child.CompareTag("Tile"))
			{
				var pos = child.position.ToRoundedVector2IntXZ();
				tileDict[pos] = new TileBase(pos, tileSizeX, tileSizeY,
					child.parent.name == "AllyLayer" ? ObjectType.Ally :
					child.parent.name == "EnemyLayer" ? ObjectType.Enemy : ObjectType.Neutral);

				if (xSet.Add(pos.x))
				{
					colCnt++;
				}

				if (pos.x > xMax)
				{
					xMax = pos.x;
				}

				if (pos.x < xMin)
				{
					xMin = pos.x;
				}

				if (ySet.Add(pos.y))
				{
					rowCnt++;
				}

				if (pos.y > yMax)
				{
					yMax = pos.y;
				}

				if (pos.y < yMin)
				{
					yMin = pos.y;
				}
			}

			RegisterTiles(child, xSet, ySet);
		}
	}

	private void LoadBattleObjects()
	{
		battleObjectManager = new(this);
	}

	public ITile GetTileAt(Vector3 position)
	{
		return tileDict.GetValueOrDefault((new Vector2Int(Mathf.RoundToInt(position.x / tileSizeX) * tileSizeX, Mathf.RoundToInt(position.z / tileSizeY) * tileSizeY)));
	}

	/// <summary>
	/// zeroBase Coordinate
	/// </summary>
	public ITile GetTileAt(int row, int col)
	{
		return tileDict.GetValueOrDefault(new Vector2Int(xMin + col * tileSizeX, yMin + row * tileSizeY));
	}

	/// <summary>
	/// zeroBase Coordinate
	/// </summary>
	public (int, int) GetTileCoord(ITile tile)
	{
		var coordVector = (tile.GetPosition().ToRoundedVector2IntXZ() - new Vector2Int(xMin, yMin));
		return ( coordVector.y / tileSizeY, coordVector.x / tileSizeX);
	}

	public ITile[] GetTiles()
	{
		return tileDict.Values.ToArray();
	}

	public IBattleObject[] GetBattleObjects()
	{
		return battleObjectManager.GetBattleObjects();
	}

	public IBattleObject GetBattleObjectOfTile(ITile tile)
	{
		return battleObjectManager.GetBattleObjectOn(tile);
	}

	public ITile GetTileOfBattleObject(IBattleObject obj)
	{
		return battleObjectManager.GetTileOf(obj);
	}

	public void SetTile(ITile tile, IBattleObject obj)
	{
		battleObjectManager.SetTile(tile, obj);
	}

	public void SwitchTile(ITile tileA, ITile tileB)
	{
		battleObjectManager.SwitchBattleObjectOfTile(tileA, tileB);
	}

	public void RemoveFromTile(IBattleObject obj)
	{
		battleObjectManager.RemoveFromTile(obj);
	}

	public int RowCnt => rowCnt;
	public int ColumnCnt => colCnt;
}