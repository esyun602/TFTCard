using System.Collections.Generic;
using System.Linq;

public class VacuumTubeLumpAmplifyAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;

	public VacuumTubeLumpAmplifyAction(VacuumTubeLumpAmplifyActionSpec spec) : base(spec)
	{
	}
	public override IEnumerable<ITile> Targets => Enumerable.Empty<ITile>();

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0.2f && timePassed - dt < 0.2f)
		{
			for (var i = 0; i < BattleStat.GetValueByValueType(UnitValueType.Attack); i++)
			{
				Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawEnemyCard();
			}
		}
		else if (timePassed > 1.5f)
		{
			routineDone = true;
		}
	}

	protected override void OnTrigger()
	{
		//todo: 스트링키
		BattleStat.Owner.AnimationController.RunGaugeMotion("적 카드 추가!");
		timePassed = 0f;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}