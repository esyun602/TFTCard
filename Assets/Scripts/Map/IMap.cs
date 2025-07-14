
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public interface IMap
{
	public void Load();
	public ITile GetTileAt(Vector3 position);
	public ITile GetTileAt(int row, int col);
	public (int,int) GetTileCoord(ITile tile);
	public ITile[] GetTiles();
	public IBattleObject[] GetBattleObjects();

	public IBattleObject GetBattleObjectOfTile(ITile tile);
	public ITile GetTileOfBattleObject(IBattleObject obj);
	public void SetTile(ITile tile, IBattleObject obj);
	public void SwitchTile(ITile tileA, ITile tileB);
	public void RemoveFromTile(IBattleObject obj);
	
	public int RowCnt { get; }
	public int ColumnCnt { get; }
}

public static class IMapExtensions
{
	public static bool IsInTriggerPos(this IMap map, GridSelector gridSelector, IBattleObject owner)
	{
		if (owner.ObjectType == ObjectType.Enemy)
		{
			return gridSelector.triggerCellList.Contains((map.ColumnCnt - 1 -
			                                       map.GetTileCoordOf(owner).Item2));
		}
		else if(owner.ObjectType == ObjectType.Ally)
		{
			return gridSelector.triggerCellList.Contains((map.GetTileCoordOf(owner).Item2));
		}

		return false;
	}

	public static bool IsInAttackTargetTile(this IMap map, GridSelector gridSelector, IBattleObject owner, ITile tile)
	{
		var (row, col) = map.GetTileCoord(tile);
		return IsInAttackTargetTile(map, gridSelector, owner, row, col);
	}

	public static (int, int) GetTileCoordOf(this IMap map, IBattleObject bo)
	{
		return map.GetTileCoord(map.GetTileOfBattleObject(bo));
	}

	public static IBattleObject GetBattleObjectAt(this IMap map, int row, int col)
	{
		return map.GetBattleObjectOfTile(map.GetTileAt(row, col));
	}
	
	public static IBattleObject GetBattleObjectAt(this IMap map, Vector3 position)
	{
		return map.GetBattleObjectOfTile(map.GetTileAt(position));
	}

	public static int GetAttackTargetRow(this IMap map, IBattleObject owner)
	{
		var (row, _) = map.GetTileCoordOf(owner);

		for (var offset = 0; offset < map.RowCnt; offset++)
		{
			if (IsBattleObjectInRow(row + offset))
			{
				return row + offset;
			}
			else if (IsBattleObjectInRow(row - offset))
			{
				return row - offset;
			}
		}
		
		return -1;

		bool IsBattleObjectInRow(int row)
		{
			var baseCol = owner.ObjectType == ObjectType.Ally ? map.ColumnCnt / 2 : 0;
			for (var col = 0; col < 4; col++)
			{
				if (row>=0 && row < map.RowCnt && map.GetBattleObjectAt(row, baseCol + col) != null)
				{
					return true;
				}
			}

			return false;
		}
	}
	
	public static bool IsInAttackTargetTile(this IMap map, GridSelector gridSelector, IBattleObject owner, int row, int col)
	{
		var targetRow = map.GetAttackTargetRow(owner);
		if (row != targetRow)
		{
			return false;
		}
		
		if (owner.ObjectType == ObjectType.Ally)
		{
			return gridSelector.attackCellList.Contains((map.ColumnCnt - 1 - col));
		}
		else if(owner.ObjectType == ObjectType.Enemy)
		{
			return gridSelector.attackCellList.Contains(col);
		}

		return false;
	}

	//새로운 기획 반영
	public static ITile GetAttackTargetTile(this IMap map, ITile tile)
	{
		var (row, _) = map.GetTileCoord(tile);
		return map.GetFirstTileInRow(row, tile.TileType.GetOpposite());
	}
	
	[Obsolete]
	public static List<ITile> GetAttackTargetTiles(this IMap map, GridSelector gridSelector, IBattleObject owner)
	{
		var ret = new List<ITile>();
		var row = map.GetAttackTargetRow(owner);
		if (!IsInTriggerPos(map, gridSelector, owner))
		{
			return null;
		}
		
		for (var i = 0; i < map.ColumnCnt; i++)
		{
			ITile tile;
			if (map.IsInAttackTargetTile(gridSelector, owner, row, i) && (tile = map.GetTileAt(row, i)) != null)
			{
				ret.Add(tile);
			}
		}

		return ret;
	}
	
	public static ITile GetFirstTileInRow(this IMap map, ITile tile)
	{
		var (row, _) = map.GetTileCoord(tile);
		return map.GetFirstTileInRow(row, tile.TileType);
	}
	
	public static ITile GetFirstTileInRow(this IMap map, int row, ObjectType type)
	{
		if (type == ObjectType.Ally)
		{
			return map.GetTileAt(row, map.ColumnCnt / 2 - 1);
		}
		else
		{
			return map.GetTileAt(row, map.ColumnCnt / 2);
		}
	}

	public static IBattleObject GetFirstObjectInRow(this IMap map, ITile tile)
	{
		var (row, _) = map.GetTileCoord(tile);
		return map.GetFirstObjectInRow(row, tile.TileType);
	}
	
	public static IBattleObject GetFirstObjectInRow(this IMap map, int row, ObjectType type)
	{
		for (var i = 0; i < map.ColumnCnt / 2; i++)
		{
			IBattleObject ret;
			if (type == ObjectType.Ally && (ret = map.GetBattleObjectAt(row, map.ColumnCnt / 2 - 1 - i)) != null)
			{
				return ret;
			}
			if (type == ObjectType.Enemy && (ret = map.GetBattleObjectAt(row, map.ColumnCnt / 2 + i)) != null)
			{
				return ret;
			}
		}

		return null;
	}
}