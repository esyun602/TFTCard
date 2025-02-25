using MessageSystem;

public class FieldCardSelectNotice : Notice
{
	public BattleCardObjectInField SelectedCard { get; }
	public FieldCardSelectNotice(BattleCardObjectInField owner)
	{
		SelectedCard = owner;
	}
}