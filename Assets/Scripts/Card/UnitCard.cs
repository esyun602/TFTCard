using UnityEngine;

public class UnitCard : ICard
{
	public UnitCardStat Stat { get; }
	public UnitCardActionBase Action { get; }
	public UnitCardSpec UnitCardStaticSpec { get; }
	public UnitSkillCard UnitSkillCard { get; }

	public UnitCard(UnitCardSpec spec)
	{
		UnitCardStaticSpec = spec;
		UnitSkillCard = new UnitSkillCard(new SkillCard(UnitCardStaticSpec.targetSkillCardSpec));
		Action = spec.actionData.CreateCardAction();
		Stat = new UnitCardStat(spec.statSpec);
	}

	public ICardSpec CardStaticSpec => UnitCardStaticSpec;
}