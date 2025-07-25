using MessageSystem;

public class DamageDodgeNotice : Notice
{
	public DamageDodgeNotice(IBattleObject damageSender, IBattleObject dodgedObject)
	{
		DamageSender = damageSender;
		DodgedObject = dodgedObject;
	}

	public IBattleObject DamageSender { get; }
	public IBattleObject DodgedObject { get; }
	
}