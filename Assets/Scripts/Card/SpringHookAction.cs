using System;

public class SpringHookAction : UnitSkillCardActionBase
{
	private float timePassed;
	private bool canceled;
	private IBattleObject target;

	public SpringHookAction(SpringHookActionSpec spec)
	{
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
			//todo: owner가 있으면 그냥 스탯에 owner 스탯을 합쳐버리는 방향으로 수정
			target.Damage(stat.Owner, stat.Owner.UnitCardBattleStat.Attack);
			Game.Instance.GetGameMode<BattleStageGameMode>().BattleStage.BattleMap.GrabObject(target);
			
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