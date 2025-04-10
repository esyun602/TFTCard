
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
			                                       map.GetTileCoord(map.GetTileOfBattleObject(owner)).Item2));
		}
		else if(owner.ObjectType == ObjectType.Ally)
		{
			return gridSelector.triggerCellList.Contains((map.GetTileCoord(map.GetTileOfBattleObject(owner)).Item2));
		}

		return false;
	}

	public static bool IsInAttackTargetTile(this IMap map, GridSelector gridSelector, IBattleObject owner, ITile tile)
	{
		var (row, col) = map.GetTileCoord(tile);
		return IsInAttackTargetTile(map, gridSelector, owner, row, col);
	}

	public static int GetAttackTargetRow(this IMap map, IBattleObject owner)
	{
		var (row, _) = map.GetTileCoord(map.GetTileOfBattleObject(owner));

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
				if (row>=0 && row < map.RowCnt && map.GetBattleObjectOfTile(map.GetTileAt(row, baseCol + col)) != null)
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
}