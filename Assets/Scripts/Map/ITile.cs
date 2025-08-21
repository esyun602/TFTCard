
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

public static class ITileExtensions
{
	public static bool HasSameRow(this ITile tile, ITile target)
	{
		var map = Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.Map;
		var (row1, _) = map.GetTileCoord(tile);
		var (row2, _) = map.GetTileCoord(target);

		return row1 == row2;
	}
}