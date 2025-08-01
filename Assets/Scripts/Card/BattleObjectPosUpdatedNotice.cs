using MessageSystem;

public class BattleObjectPosUpdatedNotice : Notice
{
	public BattleObjectPosUpdatedNotice(IBattleObject target, ITile targetTile)
	{
		Target = target;
		TargetTile = targetTile;
	}

	public IBattleObject Target { get; private set; }
	public ITile TargetTile { get; private set; }
}