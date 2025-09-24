using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UnitCard : ICard
{
	public UnitCardStat Stat { get; }
	public UnitCardSpec UnitCardStaticSpec { get; }
	//todo: 적 / 아군 분리
	public List<UnitSkillCard> UnitSkillCard { get; }

	public UnitCard(UnitCardSpec spec)
	{
		UnitCardStaticSpec = spec;
		UnitSkillCard = new();
		foreach (var str in spec.TargetSkillCardSpecName)
		{
			var targetSkillSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(str);
			if (targetSkillSpec != null)
			{
				UnitSkillCard.Add(new UnitSkillCard(targetSkillSpec, this));
			}
		}

		if (!String.IsNullOrEmpty(spec.ActionSpecName))
		{
			var actionSpec = GameDataSystem.Instance.GetGameData<ActionData>().GetUnitActionByName(spec.ActionSpecName);
		}
		var statSpec = GameDataSystem.Instance.GetGameData<StatData>().GetUnitStatByName(spec.StatSpecName);
		Stat = new UnitCardStat(statSpec);
	}

	public ICardSpec CardStaticSpec => UnitCardStaticSpec;
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(CardStaticSpec.NameKey);
	//todo: 설명은 액션으로 ?
	//todo: remove test code, 키워드는 나중에
	public string Desc
	{
		get
		{
			var strBuilder = new StringBuilder();
			foreach (var synergy in Stat.synergyList)
			{
				strBuilder.Append(GameDataSystem.Instance.GetGameData<GameString>().GetString(GameDataSystem.Instance
					.GetGameData<SynergyData>().GetSynergySpec(synergy).SynergyNameKey));
				strBuilder.Append('\n');
			}

			return strBuilder.ToString();
		}
	}
}