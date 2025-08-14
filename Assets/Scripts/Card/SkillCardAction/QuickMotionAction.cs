using System;
using UnityEngine;

//todo: 타게팅 조건 추가 필요
public class QuickMotionAction : SkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private float actionDuration;
	private GameObject fxPrefab;
	private IBattleObject target;
	
	public QuickMotionAction(QuickMotionActionSpec spec)
	{
		actionDuration = spec.actionDuration;
		fxPrefab = spec.fxPrefab;
	}

	public override object[] DescParams { get; }

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
			//임시
			if (target is ITurnObject to)
			{
				to.StartTurn(stat.GetValueByValueType(BattleValueType.TurnCount));
				routine.AddChain(to.UpdatableRoutine);
			}
			
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