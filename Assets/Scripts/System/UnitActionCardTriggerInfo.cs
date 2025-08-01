public class UnitActionCardTriggerInfo
{
	public UnitActionCardTriggerInfo(IBattleObject owner)
	{
		Owner = owner;
	}

	public IBattleObject Owner { get; private set; }
}