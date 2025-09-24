using System;
using System.Collections.Generic;

public class PrepareBombingAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public PrepareBombingAction(PrepareBombingActionSpec spec) : base(spec)
	{
	}

	public override object[] DescParams { get; }
	public override IEnumerable<ITile> Targets => ActionUtils.GetTargetTileWithTargetingInfo(triggerInfo);

	protected override void OnUpdate(float dt, out bool routineDone)
	{
		if (canceled)
		{
			routineDone = true;
			return;
		}

		routineDone = false;

		timePassed += dt;
		if (timePassed > 0f)
		{
			Game.Instance.GetGameMode<BattleStageGameMode>().DeckSystem.DrawPlayerCard();
			//todo: 비행선
		}
	}


	protected override void OnTrigger()
	{
		timePassed = 0f;
		target = ActionUtils.GetTargetObjectWithTargetingInfo(triggerInfo);
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}