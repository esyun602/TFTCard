using MessageSystem;

public class BattleObjectSwitchActNotice : Notice
{
	public BattleObjectSwitchActNotice(ITile targetTile)
	{
		TargetTile = targetTile;
	}

	public ITile TargetTile { get; }
}