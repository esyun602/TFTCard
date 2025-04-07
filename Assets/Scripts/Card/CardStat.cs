
using System.Collections.Generic;

public class CardStat : IStat
{
	int IStat.Hp => MaxHp;
	public int MaxTurnCount { get; set; }
	public int TurnCount => MaxTurnCount;
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public int Cost { get; set; }
	public List<Synergy> synergyList = new();
	private CardStatSpec staticStatSpec;

	public CardStat(CardStatSpec statSpec)
	{
		staticStatSpec = statSpec;
		MaxTurnCount = statSpec.turnCount;
		MaxHp = statSpec.hp;
		Attack = statSpec.attack;
		Cost = statSpec.cost;
		synergyList = new(statSpec.synergy);
	}
}