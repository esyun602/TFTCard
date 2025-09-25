using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using MessageSystem;
using UnityEngine;


public class BattleMap : IMap
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
			//todo: object type 비교 필요?
			if (GetBattleObjectOn(tile) != null)
			{
				//todo: fix
				throw new ArgumentException();
			}

			var originTile = GetTileOf(obj);

			RemoveFromTileImpl(obj);
			SetTileImpl(obj, tile);

			ReAlignRow(originTile);
			ReAlignRow(tile);
		}

		public void SwitchBattleObjectOfTile(ITile tileA, ITile tileB)
		{
			var objA = GetBattleObjectOn(tileA);
			var objB = GetBattleObjectOn(tileB);

			(battleObjectToTile[objA], battleObjectToTile[objB]) = (battleObjectToTile[objB], battleObjectToTile[objA]);
			(tileToBattleObject[tileA], tileToBattleObject[tileB]) =
				(tileToBattleObject[tileB], tileToBattleObject[tileA]);

			var e = new BattleObjectPosUpdatedNotice(objA, tileB);
			NoticeSystem.Instance.PublishSync(e);
			if (objA is IMessageReceiver mra)
			{
				NoticeSystem.Instance.SendSync(e, mra);
			}

			e = new BattleObjectPosUpdatedNotice(objB, tileA);
			NoticeSystem.Instance.PublishSync(e);
			if (objB is IMessageReceiver mrb)
			{
				NoticeSystem.Instance.SendSync(e, mrb);
			}
		}

		public void GrabObject(IBattleObject battleObject)
		{
			var tile = GetTileOf(battleObject);
			if (tile == null) return;

			RemoveFromTileImpl(battleObject);

			var firstTile = owner.GetFirstTileInRow(tile);
			var firstObject = GetBattleObjectOn(firstTile);

			if (firstTile == tile)
			{
				return;
			}

			if (KnockBackObject(firstObject))
			{
				SetTileImpl(battleObject, firstTile);
				var e = new BattleObjectPosUpdatedNotice(battleObject, firstTile);
				NoticeSystem.Instance.PublishSync(e);
				if (battleObject is IMessageReceiver mr)
				{
					NoticeSystem.Instance.SendSync(e, mr);
				}
			}
			else
			{
				SetTileImpl(battleObject, tile);
			}
		}

		private bool KnockBackObject(IBattleObject battleObject)
		{
			var tile = GetTileOf(battleObject);
			if (tile == null) return false;
			var backTile = owner.GetBackwardTile(tile);

			if (backTile == null) return false;

			var backObject = GetBattleObjectOn(backTile);
			if (backObject == null || KnockBackObject(backObject))
			{
				RemoveFromTileImpl(battleObject);
				SetTileImpl(battleObject, backTile);
				var e = new BattleObjectPosUpdatedNotice(battleObject, backTile);
				NoticeSystem.Instance.PublishSync(e);
				if (battleObject is IMessageReceiver mr)
				{
					NoticeSystem.Instance.SendSync(e, mr);
				}
				return true;
			}

			return false;
		}

		public void RemoveFromTile(IBattleObject obj)
		{
			var targetTile = GetTileOf(obj);
			RemoveFromTileImpl(obj);

			ReAlignRow(targetTile);
		}

		public void ReAlignRow(ITile tile)
		{
			if (tile == null) return;

			var firstObj = owner.GetFirstObjectInRow(tile);
			var firstTile = owner.GetFirstTileInRow(tile);
			if (GetBattleObjectOn(firstTile) == firstObj)
			{
				return;
			}

			RemoveFromTileImpl(firstObj);
			SetTileImpl(firstObj, firstTile);

			var e = new BattleObjectPosUpdatedNotice(firstObj, firstTile);
			NoticeSystem.Instance.PublishSync(e);
			if (firstObj is IMessageReceiver mr)
			{
				NoticeSystem.Instance.SendSync(e, mr);
			}
		}

		private void RemoveFromTileImpl(IBattleObject obj)
		{
			var targetTile = GetTileOf(obj);

			if (targetTile == null)
				return;


			tileToBattleObject.Remove(targetTile);
			battleObjectToTile.Remove(obj);
		}

		private void SetTileImpl(IBattleObject obj, ITile tile)
		{
			battleObjectToTile[obj] = tile;
			tileToBattleObject[tile] = obj;
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

	private List<float> colPosList;
	private List<float> rowPosList;

	private Dictionary<(int, int), ITile> tilePosCache;
	private List<ITile> tileList;
	private Vector3 tileSize = new Vector3(1.5f, 100f, 2f);

	public BattleMap(GameObject mapObject)
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
		tilePosCache = new();
		colPosList = new List<float>();
		rowPosList = new List<float>();
		RegisterTiles(mapObject.transform);
	}

	private void RegisterTiles(Transform root)
	{
		tileList = new();
		InitializeTileInfos(root);
		InitializeTileCache();
	}

	private void InitializeTileInfos(Transform root)
	{
		foreach (Transform child in root)
		{
			//todo constant, tileprop serialize
			if (child.CompareTag("Tile"))
			{
				var pos = child.position;

				if (colPosList.Count == 0)
				{
					colPosList.Add(pos.x);
				}
				else
				{
					for (var i = 0; i < colPosList.Count; i++)
					{
						if (colPosList[i].IsAlmostCloseTo(pos.x))
						{
							break;
						}
						else if (colPosList[i] > pos.x)
						{
							colPosList.Insert(i, pos.x);
							break;
						}
						else if (i == colPosList.Count - 1)
						{
							colPosList.Add(pos.x);
						}
					}
				}


				if (rowPosList.Count == 0)
				{
					rowPosList.Add(pos.z);
				}
				else
				{
					for (var i = 0; i < rowPosList.Count; i++)
					{
						if (rowPosList[i].IsAlmostCloseTo(pos.z))
						{
							break;
						}
						else if (rowPosList[i] > pos.z)
						{
							rowPosList.Insert(i, pos.z);
							break;
						}
						else if (i == rowPosList.Count - 1)
						{
							rowPosList.Add(pos.z);
						}
					}
				}


				tileList.Add(new TileBase(pos, tileSize,
					child.parent.name == "AllyLayer" ? ObjectType.Ally :
					child.parent.name == "EnemyLayer" ? ObjectType.Enemy : ObjectType.Neutral));
			}

			InitializeTileInfos(child);
		}
	}

	private void InitializeTileCache()
	{
		foreach (var tile in tileList)
		{
			var (row, col) = (tile.GetPosition().ToRowCol(rowPosList, colPosList));
			tilePosCache[(row, col)] = tile;
		}
	}

	private void LoadBattleObjects()
	{
		battleObjectManager = new(this);
	}

	public ITile GetTileAt(Vector3 position)
	{
		var tile = tilePosCache.GetValueOrDefault(position.ToRowCol(rowPosList, colPosList));
		return tile?.Contains(position) == true ? tile : null;
	}

	/// <summary>
	/// zeroBase Coordinate
	/// </summary>
	public ITile GetTileAt(int row, int col)
	{
		return tilePosCache.GetValueOrDefault((row, col));
	}

	/// <summary>
	/// zeroBase Coordinate
	/// </summary>
	public (int, int) GetTileCoord(ITile tile)
	{
		return (tile.GetPosition().ToRowCol(rowPosList, colPosList));
	}

	public ITile[] GetTiles()
	{
		return tilePosCache.Values.ToArray();
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

	/// <summary>
	/// 같은 행 맨 앞 열로만 가정
	/// todo: 우선 턴 진행 중 사용되지 않을 것을 가정, 사용될 여지가 있으면 턴 시스템 수정 필요
	/// </summary>
	public void GrabObject(IBattleObject obj)
	{
		battleObjectManager.GrabObject(obj);
	}

	public int RowCnt => rowPosList.Count;
	public int ColumnCnt => colPosList.Count;
}