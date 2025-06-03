using System;
using UnityEngine;

public class TestSkillAction : SkillCardActionBase
{
	private IUpdatableRoutine routine;
	private float timePassed;
	public IUpdatableRoutine UpdatableRoutine => routine;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;
	
	public TestSkillAction(TestSkillActionData data)
	{
		actionDuration = data.actionDuration;
		fxPrefab = data.fxPrefab;
	}
	
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
			//todo: sender 수정
			target.Damage(null, stat.AttackValue);
			routineDone = true;
		}
	}

	protected override void OnTrigger(object triggerInfo)
	{
		timePassed = 0f;
		if (triggerInfo is not DefaultActionTriggerInfo ti)
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