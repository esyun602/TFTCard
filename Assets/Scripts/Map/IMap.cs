
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
	
	public int RowCnt { get; }
	public int ColumnCnt { get; }
	
}