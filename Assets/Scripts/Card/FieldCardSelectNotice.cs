using MessageSystem;

public class FieldCardSelectNotice : Notice
{
	public UnitCardInField SelectedCard { get; }
	public FieldCardSelectNotice(UnitCardInField owner)
	{
		SelectedCard = owner;
	}
}