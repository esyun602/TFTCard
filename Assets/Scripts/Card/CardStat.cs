
public class CardStat
{
	public int Speed;
	public int MaxHp;
	public int Attack;
	private CardStatSpec staticStatSpec;

	public CardStat(CardStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		Speed = statSpec.speed;
	}
}