using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddShieldSkillAction : TacticsCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;
	
	public AddShieldSkillAction(AddShieldSkillActionSpec spec) : base(spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(CommonValueType.ShieldAdd) };
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
			target.UnitCardBattleStat.AddValueByValueType(UnitValueType.Shield, BattleStat.GetValueByValueType(CommonValueType.ShieldAdd));
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