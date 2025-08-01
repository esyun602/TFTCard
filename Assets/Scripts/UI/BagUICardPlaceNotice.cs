using MessageSystem;

public class BagUICardPlaceNotice : Notice
{
	public BagUICardPlaceNotice(BagUnitCard targetCard, BagUITile targetTile)
	{
		TargetCard = targetCard;
		TargetTile = targetTile;
	}

	public BagUnitCard TargetCard { get; }
	public BagUITile TargetTile { get; }
}