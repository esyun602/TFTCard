
public class CardStat
{
	public float Speed;
	public int MaxHp;
	public int Attack;
	private CardStatData staticStatData;

	public CardStat(CardStatData statData)
	{
		staticStatData = statData;
		Speed = statData.Speed;
	}
}