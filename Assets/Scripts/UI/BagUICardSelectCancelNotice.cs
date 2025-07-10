using MessageSystem;

public class BagUICardSelectCancelNotice : Notice
{
	public BagUICardSelectCancelNotice(BagUICard targetCard)
	{
		TargetCard = targetCard;
	}

	public BagUICard TargetCard { get; }
}