using UnityEngine;

public class UnitCard : ICard
{
	public UnitCardStat Stat { get; }
	public UnitCardActionBase Action { get; }
	public UnitCardSpec UnitCardStaticSpec { get; }
	//todo: 적 / 아군 분리
	public SkillCard UnitSkillCard { get; }

	public UnitCard(UnitCardSpec spec)
	{
		UnitCardStaticSpec = spec;
		var targetSkillSpec = GameDataSystem.Instance.GetGameData<CardData>().GetSkillCardSpecByName(spec.TargetSkillCardSpecName);
		UnitSkillCard = new SkillCard(targetSkillSpec);
		var actionSpec = GameDataSystem.Instance.GetGameData<ActionData>().GetUnitActionByName(spec.ActionSpecName);
		Action = actionSpec.CreateCardAction();
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetUnitStatByName(spec.StatSpecName);
		Stat = new UnitCardStat(statSpec);
	}

	public ICardSpec CardStaticSpec => UnitCardStaticSpec;
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(CardStaticSpec.NameKey);
	//todo: 설명은 액션으로 ?
	public string Desc => GameDataSystem.Instance.GetGameData<GameString>()
		.Format(CardStaticSpec.DescKey, Action.DescParams);
}