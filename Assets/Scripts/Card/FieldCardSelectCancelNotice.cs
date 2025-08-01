using MessageSystem;

public class FieldCardSelectCancelNotice : Notice
{
	public UnitCardInField SelectedCard { get; }
	
	public FieldCardSelectCancelNotice(UnitCardInField target)
	{
		this.SelectedCard = target;
	}
}