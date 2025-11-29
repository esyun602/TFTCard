using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrassMonsterPrepareAction : UnitSkillCardActionBase
{
	private UnitSkillCardSpec horizontalCardSpec;
	private UnitSkillCardSpec verticalCardSpec;
	private float timePassed = 0f;
	
	public BrassMonsterPrepareAction(BrassMonsterPrepareActionSpec spec) : base(spec)
	{
		horizontalCardSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(spec.HorizontalCardName);
		verticalCardSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(spec.VerticalCardName);
	}

	public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (timePassed == 0)
		{
			BattleStat.Owner.AnimationController.RunGaugeMotion();
		}
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			if (Random.value > 0.5f)
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GenerateEnemySkillCardInstance(
					BattleStat.Owner, new UnitSkillCard(horizontalCardSpec, Stat.Owner));
			}
			else
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GenerateEnemySkillCardInstance(
					BattleStat.Owner, new UnitSkillCard(verticalCardSpec, Stat.Owner));
			}
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
	}
}