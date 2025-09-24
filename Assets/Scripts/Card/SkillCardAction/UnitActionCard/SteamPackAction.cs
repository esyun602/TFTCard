using System;
using System.Collections.Generic;

public class SteamPackAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public SteamPackAction(SteamPackActionSpec spec) : base(spec)
	{
	}

	public override object[] DescParams => new object[] { 1, 2 };
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
			target.UnitCardBattleStat.AddBuff(new BurnBuff(1));
			
			if (target is ITurnObject to)
			{
				to.StartTurn(2);
				routine.AddChain(to.UpdatableRoutine);
			}
			
			routineDone = true;
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