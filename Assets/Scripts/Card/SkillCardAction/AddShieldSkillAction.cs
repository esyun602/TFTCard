using System;
using UnityEngine;

public class AddShieldSkillAction : TacticsCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;
	
	public AddShieldSkillAction(AddShieldSkillActionSpec spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}

	public override object[] DescParams => new object[] { StatFallback.GetValueByValueType(CommonValueType.ShieldAdd) };

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

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
		if (triggerInfo is not TargetingActionTriggerInfo ti)
		{
			throw new ArgumentException();
		}

		target = ti.Target;
	}

	protected override void OnCancel()
	{
		canceled = true;
	}
}