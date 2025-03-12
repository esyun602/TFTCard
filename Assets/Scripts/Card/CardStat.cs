
public class CardStat
{
	public int Speed { get; set; }
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public int Cost { get; set; }
	private CardStatSpec staticStatSpec;

	public CardStat(CardStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		Speed = statSpec.speed;
		MaxHp = statSpec.hp;
		Attack = statSpec.attack;
		Cost = statSpec.cost;
	}
}