using MessageSystem;
using UnityEngine;

public struct BagCardPosInfo
{
	public Vector3 TargetPos { get; }
	public BagUITile Tile { get; }

	public BagCardPosInfo(Vector3 targetPos, BagUITile tile)
	{
		TargetPos = targetPos;
		Tile = tile;
	}
}

public class BagCardPosUpdateNotice : Notice
{
	public BagCardPosInfo BagCardPosInfo { get; }
	public BagCardPosUpdateNotice(Vector3 targetPos)
	{
		BagCardPosInfo = new BagCardPosInfo(targetPos, null);
	}
	
	public BagCardPosUpdateNotice(BagUITile tile)
	{
		BagCardPosInfo = new BagCardPosInfo(tile.GetPosition(), tile);
	}

}