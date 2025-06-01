using UnityEngine;

public class UnitCard : ICard
{
	public UnitCardStat Stat { get; }
	public IAction Action { get; }
	public UnitCardSpec UnitCardStaticSpec { get; }

	public UnitCard(UnitCardSpec spec)
	{
		UnitCardStaticSpec = spec;
		Action = spec.actionData.CreateCardAction();
		Stat = new UnitCardStat(spec.statSpec);
	}

	public ICardSpec CardStaticSpec => UnitCardStaticSpec;
}