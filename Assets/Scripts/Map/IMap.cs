
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

	public static bool IsInTargetTile(this IMap map, GridSelector gridSelector, IBattleObject owner, ITile tile)
	{
		var (row, col) = map.GetTileCoord(tile);
		return IsInTargetTile(map, gridSelector, owner, row, col);
	}
	
	public static bool IsInTargetTile(this IMap map, GridSelector gridSelector, IBattleObject owner, int row, int col)
	{
		if (row != map.GetTileCoord(map.GetTileOfBattleObject(owner)).Item1)
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
	
	public static List<ITile> GetTargetTiles(this IMap map, GridSelector gridSelector, IBattleObject owner)
	{
		var ret = new List<ITile>();
		var row = map.GetTileCoord(map.GetTileOfBattleObject(owner)).Item1;
		if (!IsInTriggerPos(map, gridSelector, owner))
		{
			return null;
		}
		
		for (var i = 0; i < map.ColumnCnt; i++)
		{
			if (map.IsInTargetTile(gridSelector, owner, row, i))
			{
				ret.Add(map.GetTileAt(row, i));
			}
		}

		return ret;
	}
}