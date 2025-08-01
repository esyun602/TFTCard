using MessageSystem;

public enum HoverType
{
	Enter = 0,
	Exit = 1,
}

public class BagUITileHoverNotice : Notice
{
	public BagUITileHoverNotice(BagUITile targetTile, HoverType hoverType)
	{
		TargetTile = targetTile;
		HoverType = hoverType;
	}

	public BagUITile TargetTile { get; }
	public HoverType HoverType { get; }
}