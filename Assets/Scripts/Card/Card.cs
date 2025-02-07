using UnityEngine;

public class Card
{
	public CardStat Stat { get; }
	public IAction Action { get; }
	public CardData CardStaticData { get; }

	public Card(CardData data)
	{
		CardStaticData = data;
		Action = data.actionData.CreateCardAction();
		Stat = new CardStat(data.statData);
	}
}