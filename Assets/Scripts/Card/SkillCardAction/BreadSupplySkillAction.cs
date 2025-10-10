using System;
using System.Collections.Generic;
using UnityEngine;

public class BreadSupplySkillAction : TacticsCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;

	public BreadSupplySkillAction(BreadSupplySkillActionSpec spec) : base(spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}
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
			//todo: heal?
			target.Heal(new HealInfo()
			{
				HealAmount = BattleStat.GetValueByValueType(SkillValueType.Heal)
			});
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