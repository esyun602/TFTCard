using MessageSystem;

public class BagUICardUnPlaceNotice : Notice
{
	public BagUICardUnPlaceNotice(BagUnitCard targetCard, BagUITile targetTile)
	{
		TargetCard = targetCard;
		TargetTile = targetTile;
	}

	public BagUnitCard TargetCard { get; }
	public BagUITile TargetTile { get; }
}