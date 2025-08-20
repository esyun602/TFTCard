using MessageSystem;

//todo: notice 분리
public class DraftUICardSelectedNotice : Notice
{
	public DraftUICardSelectedNotice(DraftUICard selectedCard)
	{
		SelectedCard = selectedCard;
	}

	public DraftUICard SelectedCard { get; private set; }
	
}