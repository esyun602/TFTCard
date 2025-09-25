using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BrassMonsterPrepareAction : UnitSkillCardActionBase
{
	private UnitSkillCardSpec targetCardSpec;
	private float timePassed = 0f;
	
	public BrassMonsterPrepareAction(BrassMonsterPrepareActionSpec spec) : base(spec)
	{
		targetCardSpec = GameDataSystem.Instance.GetGameData<CardData>().GetUnitSkillCardSpecByName(spec.TargetCardName);
	}

	public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.15f && timePassed - dt < 0.15f)
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.GenerateUnitSkillCardInstance(
				BattleStat.Owner, new UnitSkillCard(targetCardSpec, Stat.Owner), true);
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