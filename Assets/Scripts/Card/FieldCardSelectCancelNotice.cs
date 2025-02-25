using MessageSystem;

public class FieldCardSelectCancelNotice : Notice
{
	public BattleCardObjectInField SelectedCard { get; }
	
	public FieldCardSelectCancelNotice(BattleCardObjectInField target)
	{
		this.SelectedCard = target;
	}
}