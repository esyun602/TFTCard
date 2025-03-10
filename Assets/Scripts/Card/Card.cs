using UnityEngine;

public class Card
{
	public CardStat Stat { get; }
	public IAction Action { get; }
	public CardSpec CardStaticSpec { get; }

	public Card(CardSpec spec)
	{
		CardStaticSpec = spec;
		Action = spec.actionData.CreateCardAction();
		Stat = new CardStat(spec.statSpec);
	}
}