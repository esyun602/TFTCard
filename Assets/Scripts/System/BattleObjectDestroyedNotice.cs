using MessageSystem;

public class BattleObjectDestroyedNotice : ContextNotice
{
	public IBattleObject Destroyer { get; }
	public IBattleObject Target { get; }

	public BattleObjectDestroyedNotice(IBattleObject destroyer, IBattleObject target)
	{
		Destroyer = destroyer;
		Target = target;
	}
}