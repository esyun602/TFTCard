using MessageSystem;

public class BagUICardSelectedNotice : Notice
{
	public BagUICardSelectedNotice(BagUICard targetCard)
	{
		TargetCard = targetCard;
	}

	public BagUICard TargetCard { get; }
}