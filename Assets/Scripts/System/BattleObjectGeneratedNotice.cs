using MessageSystem;

public class BattleObjectGeneratedNotice : Notice
{
	public BattleObjectGeneratedNotice(IBattleObject targetObject, ITile targetTile)
	{
		TargetObject = targetObject;
		TargetTile = targetTile;
	}

	public IBattleObject TargetObject { get; }
	public ITile TargetTile { get; }
	
}